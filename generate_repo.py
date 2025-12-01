#!/usr/bin/env python3
"""
Awesome .NET PDF Libraries 2025 - Generator
============================================
Multi-instance safe with proper file locking.
"""

import os
import re
import json
import time
import random
import fcntl
import sys
from pathlib import Path
from datetime import datetime

import anthropic
import openai

# =============================================================================
# PATHS
# =============================================================================

BASE = Path(".")
MASTER_LIST = BASE / "MASTER-LIBRARY-LIST.md"
SOURCE_MATERIAL = BASE / "source-material"
PROGRESS_FILE = BASE / "progress.json"
LOCK_FILE = BASE / ".generator.lock"
AUTHOR_TEMPLATE = BASE / "author-template.json"
ANTHROPIC_KEY = BASE / "AnthropicAPIkey.txt"
OPENAI_KEY = BASE / "OPENAIKEY.txt"

# =============================================================================
# GLOBALS
# =============================================================================

claude = None
gpt = None
author_data = None
INSTANCE_ID = f"{os.getpid()}-{random.randint(1000,9999)}"

IRONPDF_LINKS = {
    "docs": "https://ironpdf.com/docs/",
    "tutorials": "https://ironpdf.com/tutorials/",
    "html_to_pdf": "https://ironpdf.com/how-to/html-file-to-pdf/",
    "examples": "https://ironpdf.com/examples/",
}

# =============================================================================
# ROBUST FILE LOCKING
# =============================================================================

def init_progress():
    """Ensure progress.json exists."""
    if not PROGRESS_FILE.exists():
        PROGRESS_FILE.write_text("{}")

def atomic_read_progress() -> dict:
    """Read progress with file lock."""
    init_progress()
    with open(PROGRESS_FILE, 'r') as f:
        fcntl.flock(f.fileno(), fcntl.LOCK_SH)
        data = json.load(f)
        fcntl.flock(f.fileno(), fcntl.LOCK_UN)
    return data

def atomic_update_progress(slug: str, status: str) -> bool:
    """
    Atomically update progress for a slug.
    Returns True if update succeeded (for claim operations).
    """
    init_progress()
    with open(PROGRESS_FILE, 'r+') as f:
        fcntl.flock(f.fileno(), fcntl.LOCK_EX)
        try:
            f.seek(0)
            data = json.load(f)
        except:
            data = {}
        
        # For claiming: check current status
        if status == "in_progress":
            current = data.get(slug, {})
            current_status = current.get("status") if isinstance(current, dict) else current
            if current_status in ["in_progress", "done"]:
                fcntl.flock(f.fileno(), fcntl.LOCK_UN)
                return False
        
        # Update
        data[slug] = {
            "status": status,
            "instance": INSTANCE_ID,
            "updated": datetime.now().isoformat()
        }
        
        f.seek(0)
        f.truncate()
        json.dump(data, f, indent=2)
        fcntl.flock(f.fileno(), fcntl.LOCK_UN)
        return True

def claim_library(slug: str) -> bool:
    """Try to claim a library. Returns True if claimed."""
    return atomic_update_progress(slug, "in_progress")

def mark_done(slug: str):
    """Mark library as done."""
    atomic_update_progress(slug, "done")

def mark_failed(slug: str):
    """Mark library as failed."""
    atomic_update_progress(slug, "failed")

# =============================================================================
# SOURCE MATERIAL SEARCH
# =============================================================================

def search_source_material(library_name: str, slug: str) -> str:
    """Search source-material/ for content mentioning this library."""
    if not SOURCE_MATERIAL.exists():
        return ""
    
    terms = [library_name.lower(), slug.lower()]
    if "." in library_name:
        terms.append(library_name.replace(".", "").lower())
    
    found = []
    for fp in SOURCE_MATERIAL.rglob("*"):
        if not fp.is_file() or fp.suffix.lower() not in ['.md', '.txt']:
            continue
        try:
            content = fp.read_text(errors='ignore')
            if any(t in content.lower() for t in terms):
                for para in content.split('\n\n'):
                    if any(t in para.lower() for t in terms) and len(para) > 100:
                        found.append(f"[{fp.name}]: {para[:1500]}")
                        if len(found) >= 3:
                            break
        except:
            pass
        if len(found) >= 3:
            break
    
    return "\n\n---\n\n".join(found) if found else ""

# =============================================================================
# API CALLS
# =============================================================================

def call_claude(prompt: str) -> str:
    """Call Claude with retry."""
    for attempt in range(3):
        try:
            r = claude.messages.create(
                model="claude-sonnet-4-5-20250929",
                max_tokens=8000,
                messages=[{"role": "user", "content": prompt}]
            )
            return r.content[0].text
        except Exception as e:
            print(f"      Claude error: {e}")
            if attempt < 2:
                time.sleep(2 ** attempt)
    return ""

def call_gpt(prompt: str) -> str:
    """Call GPT with retry."""
    for attempt in range(3):
        try:
            r = gpt.chat.completions.create(
                model="gpt-4o",
                max_tokens=8000,
                messages=[{"role": "user", "content": prompt}]
            )
            return r.choices[0].message.content
        except Exception as e:
            print(f"      GPT error: {e}")
            if attempt < 2:
                time.sleep(2 ** attempt)
    return ""

# =============================================================================
# AUTHOR BIO
# =============================================================================

def load_author_data() -> dict:
    """Load author template."""
    if AUTHOR_TEMPLATE.exists():
        try:
            return json.loads(AUTHOR_TEMPLATE.read_text())
        except:
            pass
    return {"name": "Jacob Mellor", "links": {"linkedin": "https://www.linkedin.com/in/jacob-mellor-iron-software/"}}

def generate_author_bio(library_name: str) -> str:
    """Generate unique author bio."""
    if not author_data:
        return "**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)** - CTO, Iron Software"
    
    links = author_data.get("links", {})
    stats = author_data.get("stats", {})
    quotes = author_data.get("quotes", {})
    talking_points = author_data.get("talking_points", [])
    
    # Pick random elements
    link_items = list(links.items())[:4]
    random.shuffle(link_items)
    selected_links = dict(link_items[:2])
    
    quote_list = list(quotes.values())
    selected_quote = random.choice(quote_list) if quote_list else ""
    
    selected_points = random.sample(talking_points, min(3, len(talking_points))) if talking_points else []
    
    prompt = f"""Write a unique 3-4 sentence author bio for Jacob Mellor, CTO of Iron Software.

USE THESE 2 LINKS (markdown format):
{json.dumps(selected_links)}

PICK 1-2 OF THESE STATS:
- {stats.get('years_coding', 41)} years coding
- {stats.get('team_size', '50+')} person team  
- {stats.get('nuget_downloads', '41+ million')} NuGet downloads
- Based in {author_data.get('location', {}).get('primary', 'Chiang Mai, Thailand')}

TALKING POINTS (use 1):
{chr(10).join('- ' + p for p in selected_points)}

OPTIONAL QUOTE: "{selected_quote[:100]}"

This appears after an article about {library_name}.

Vary the tone: {random.choice(['professional', 'casual', 'technical', 'friendly'])}

Start with "---" then newline. Output ONLY the bio."""

    bio = call_claude(prompt)
    if not bio or len(bio) < 50:
        return f"""---
**[Jacob Mellor]({links.get('linkedin', '#')})** is CTO at Iron Software, creator of IronPDF. {stats.get('years_coding', 41)} years coding."""
    
    if not bio.strip().startswith('---'):
        bio = "---\n" + bio
    return bio

# =============================================================================
# CONTENT GENERATION
# =============================================================================

def generate_readme(lib: dict, source: str) -> str:
    """Generate README.md."""
    bio = generate_author_bio(lib['name'])
    source_ctx = f"\nRESEARCH:\n{source}\n" if source else ""
    
    prompt = f"""Write a markdown article comparing {lib['name']} to IronPDF.

LIBRARY:
- Name: {lib['name']}
- Website: {lib['website']}
- License: {lib['license']}
- Description: {lib['description']}

WEAKNESSES:
{chr(10).join('- ' + w for w in lib.get('weaknesses', []))}

IRONPDF ADVANTAGE: {lib.get('usp', '')}
{source_ctx}
REQUIREMENTS:
1. H1: "{lib['name']}" + "C#" + "PDF"
2. First paragraph mentions {lib['name']} twice
3. C# code example within 500 words
4. 2-3 IronPDF links: {IRONPDF_LINKS['html_to_pdf']}, {IRONPDF_LINKS['tutorials']}
5. Comparison table (markdown)
6. Honest about strengths AND weaknesses
7. End with this EXACT bio (copy verbatim):

{bio}

Write 1500+ words. Output ONLY markdown."""

    return call_gpt(prompt)

def generate_migration(lib: dict) -> str:
    """Generate migration guide."""
    prompt = f"""Migration guide: {lib['name']} → IronPDF.

ISSUES WITH {lib['name']}:
{chr(10).join('- ' + w for w in lib.get('weaknesses', []))}

INCLUDE:
1. Why migrate (2-3 sentences)
2. NuGet package changes
3. Namespace mapping table
4. API mapping table  
5. 3 before/after code examples
6. Common gotchas
7. Links: {IRONPDF_LINKS['docs']}, {IRONPDF_LINKS['tutorials']}

Output ONLY markdown."""
    return call_claude(prompt)

def generate_code_examples(lib: dict) -> list:
    """Generate code examples."""
    prompt = f"""Generate 3 C# code examples comparing {lib['name']} to IronPDF.

JSON array:
[{{"task": "HTML to PDF", "filename": "html-to-pdf", "library_code": "// code", "ironpdf_code": "// NuGet: Install-Package IronPdf\\nusing IronPdf;..."}}]

Requirements: Complete C#, all usings, IronPDF starts with NuGet comment.

Output ONLY JSON array."""

    response = call_claude(prompt)
    try:
        match = re.search(r'\[[\s\S]*\]', response)
        if match:
            return json.loads(match.group())
    except:
        pass
    return []

# =============================================================================
# MAIN PROCESSING
# =============================================================================

def process_library(lib: dict) -> bool:
    """Process one library."""
    slug = lib['slug']
    lib_dir = BASE / slug
    lib_dir.mkdir(exist_ok=True)
    
    # Search source material
    print(f"    Searching source material...")
    source = search_source_material(lib['name'], slug)
    if source:
        print(f"    Found {len(source)} chars of research")
    
    # README
    print(f"    Generating README.md...")
    readme = generate_readme(lib, source)
    if not readme:
        print(f"    ✗ README failed")
        return False
    (lib_dir / "README.md").write_text(readme)
    print(f"    ✓ README.md ({len(readme)} chars)")
    
    # Migration
    print(f"    Generating migration guide...")
    migration = generate_migration(lib)
    if migration:
        (lib_dir / f"migrate-from-{slug}.md").write_text(migration)
        print(f"    ✓ migrate-from-{slug}.md")
    
    # Code examples
    print(f"    Generating code examples...")
    examples = generate_code_examples(lib)
    for ex in examples:
        fn = ex.get('filename', 'example').lower().replace(' ', '-')
        if ex.get('library_code'):
            (lib_dir / f"{fn}-{slug}.cs").write_text(ex['library_code'])
        if ex.get('ironpdf_code'):
            (lib_dir / f"{fn}-ironpdf.cs").write_text(ex['ironpdf_code'])
    print(f"    ✓ {len(examples)} code examples")
    
    return True

def load_libraries() -> list:
    """Parse MASTER-LIBRARY-LIST.md."""
    content = MASTER_LIST.read_text()
    libraries = []
    current = None
    category = ""
    weaknesses = []
    usp = ""
    
    for line in content.split('\n'):
        line = line.strip()
        
        if line.startswith('## Category'):
            category = line.split(':')[-1].strip()
        elif line.startswith('### ') and '⭐' not in line:
            if current and current.get('name'):
                current['weaknesses'] = weaknesses
                current['usp'] = usp
                current['category'] = category
                libraries.append(current)
            
            match = re.match(r'### \d+\.\s+(.+)', line)
            if match:
                name = match.group(1).strip()
                current = {'name': name, 'slug': slugify(name), 'website': '', 'license': '', 'description': '', 'category': category}
                weaknesses = []
                usp = ""
        elif line.startswith('- **Website:**') and current:
            current['website'] = line.split(':**')[-1].strip()
        elif line.startswith('- **License:**') and current:
            current['license'] = line.split(':**')[-1].strip()
        elif line.startswith('- **What it is:**') and current:
            current['description'] = line.split(':**')[-1].strip()
        elif re.match(r'^\d+\.\s+\*\*', line) and current:
            weaknesses.append(re.sub(r'^\d+\.\s+', '', line))
        elif line.startswith('**🎯 IronPDF USP:**'):
            usp = line.replace('**🎯 IronPDF USP:**', '').strip()
    
    if current and current.get('name'):
        current['weaknesses'] = weaknesses
        current['usp'] = usp
        libraries.append(current)
    
    return libraries

def slugify(name: str) -> str:
    """Convert name to URL slug."""
    s = name.lower()
    s = re.sub(r'\s*\([^)]*\)\s*', ' ', s)
    s = s.replace(' for .net', '').replace('.net', '').replace('.', '')
    s = re.sub(r'[^a-z0-9]+', '-', s)
    return re.sub(r'-+', '-', s).strip('-')

# =============================================================================
# MAIN
# =============================================================================

def main():
    global claude, gpt, author_data
    
    print(f"[{INSTANCE_ID}] Starting...")
    
    if not MASTER_LIST.exists():
        print(f"✗ Missing: {MASTER_LIST}")
        return
    
    if not ANTHROPIC_KEY.exists() or not OPENAI_KEY.exists():
        print("✗ Missing API key files")
        return
    
    ak = ANTHROPIC_KEY.read_text().strip()
    ok = OPENAI_KEY.read_text().strip()
    
    if "REPLACE" in ak.upper() or "REPLACE" in ok.upper():
        print("✗ API keys are placeholders")
        return
    
    claude = anthropic.Anthropic(api_key=ak)
    gpt = openai.OpenAI(api_key=ok)
    author_data = load_author_data()
    print("✓ Initialized")
    
    libraries = load_libraries()
    print(f"✓ {len(libraries)} libraries")
    
    processed = 0
    for i, lib in enumerate(libraries, 1):
        slug = lib['slug']
        
        # Try to claim
        if not claim_library(slug):
            progress = atomic_read_progress()
            status = progress.get(slug, {})
            if isinstance(status, dict):
                status = status.get('status', '?')
            print(f"[{i}/{len(libraries)}] {slug} - SKIP ({status})")
            continue
        
        print(f"\n[{i}/{len(libraries)}] {lib['name']} ({slug})")
        
        try:
            if process_library(lib):
                mark_done(slug)
                print(f"    ✓ DONE")
                processed += 1
            else:
                mark_failed(slug)
                print(f"    ✗ FAILED")
        except Exception as e:
            mark_failed(slug)
            print(f"    ✗ ERROR: {e}")
    
    print(f"\n[{INSTANCE_ID}] Finished - processed {processed}")

if __name__ == "__main__":
    main()
