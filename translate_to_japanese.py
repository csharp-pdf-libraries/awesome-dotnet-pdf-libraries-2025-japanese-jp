#!/usr/bin/env python3
"""
Japanese Translation Script for awesome-dotnet-pdf-libraries-2025
Translates repository files from English to Japanese using ChatGPT API
with intelligent change tracking via MD5 hashes.
"""

import os
import json
import hashlib
import shutil
from pathlib import Path
from datetime import datetime
from openai import OpenAI

# Configuration
SOURCE_DIR = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025")
DEST_DIR = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp")
API_KEY_FILE = Path("/Users/jacob/Sites/SEO-MACHINE/OPENAIKEY.txt")
TRACKING_FILE = DEST_DIR / "translation_tracking.json"

# File extensions to translate
TRANSLATABLE_EXTENSIONS = {'.md', '.cs'}  # Markdown files and C# files

# Directories and files to skip
SKIP_PATTERNS = {
    '.git',
    '__pycache__',
    'node_modules',
    '.DS_Store',
    'translation_tracking.json',
    '.gitignore',
    'source-material',  # Gitignored folder - skip it
    'source material'    # Alternative name
}


def load_api_key():
    """Load OpenAI API key from file."""
    with open(API_KEY_FILE, 'r') as f:
        return f.read().strip()


def calculate_md5(file_path):
    """Calculate MD5 hash of a file."""
    md5_hash = hashlib.md5()
    with open(file_path, 'rb') as f:
        for chunk in iter(lambda: f.read(4096), b""):
            md5_hash.update(chunk)
    return md5_hash.hexdigest()


def load_tracking_data():
    """Load translation tracking data from JSON file."""
    if TRACKING_FILE.exists():
        with open(TRACKING_FILE, 'r', encoding='utf-8') as f:
            return json.load(f)
    return {"files": {}}


def save_tracking_data(data):
    """Save translation tracking data to JSON file."""
    TRACKING_FILE.parent.mkdir(parents=True, exist_ok=True)
    with open(TRACKING_FILE, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=2, ensure_ascii=False)


def add_language_notice(content, file_path, is_root=False):
    """Add language notice to the top of translated files."""
    relative_url = file_path.replace('\\', '/')

    # Create link to English version on GitHub
    english_url = f"https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/{relative_url}"

    # Also link to the live Japanese repo
    japanese_org_url = f"https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/{relative_url}"

    if is_root or file_path == "README.md":
        notice = f"""---
**🌐 日本語版 | Japanese Translation**

📖 **English Version:** [View Original](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025)
🇯🇵 **This Japanese Version:** [GitHub Repository](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp)

この文書は英語版から翻訳されたものです。最新の情報については、[英語版](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025)をご覧ください。

*This document is translated from the [English version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025). For the latest updates, please refer to the original English documentation.*

---

"""
    else:
        # For all other MD files - clear link to exact English file
        notice = f"""---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [{relative_url}]({english_url})
🇯🇵 **日本語:** [{relative_url}]({japanese_org_url})

---

"""

    return notice + content


def translate_text(client, text, file_path, is_csharp=False, is_readme=False):
    """Translate text from English to Japanese using ChatGPT API."""
    print(f"  Translating {file_path}...")

    if is_csharp:
        prompt = f"""Translate ONLY the comments in this C# code from English to Japanese.

IMPORTANT RULES:
1. Keep ALL code unchanged (variable names, class names, method names, etc.)
2. Translate ONLY existing comments (// and /* */ style)
3. Do NOT add new comments
4. Preserve all code structure and formatting
5. Keep technical terms in English within comments
6. Keep product/brand names unchanged

C# code to process:

{text}
"""
    else:
        prompt = f"""Translate the following Markdown content from English to Japanese.

CRITICAL RULES - DO NOT BREAK THESE:
1. Preserve ALL Markdown formatting (headers, links, code blocks, tables, etc.)
2. Keep ALL URLs EXACTLY as they are - DO NOT modify ANY URLs
3. DO NOT add utm_source, utm_medium, or ANY tracking parameters to URLs
4. Keep code blocks in ENGLISH (do not translate code)
5. Keep product/brand names unchanged (IronPDF, Aspose, Microsoft, etc.)
6. Keep file paths and technical terms in English
7. Translate only the prose and explanations
8. Maintain the same structure and formatting
9. All links to ironpdf.com, ironsoftware.com, github.com must remain EXACTLY as written

EXAMPLES OF WHAT NOT TO DO:
❌ WRONG: https://ironpdf.com?utm_source=translation
✅ CORRECT: https://ironpdf.com

Content to translate:

{text}
"""

    try:
        response = client.chat.completions.create(
            model="gpt-4-turbo-preview",
            messages=[
                {"role": "system", "content": "You are a professional technical translator specializing in software documentation. You translate English to Japanese while preserving all formatting and technical accuracy."},
                {"role": "user", "content": prompt}
            ],
            temperature=0.3,
            max_tokens=4000
        )

        translated = response.choices[0].message.content.strip()
        return translated

    except Exception as e:
        print(f"  ERROR translating {file_path}: {e}")
        raise


def should_skip(path):
    """Check if path should be skipped."""
    name = path.name
    return any(pattern in str(path) or name == pattern for pattern in SKIP_PATTERNS)


def get_relative_path(file_path, base_dir):
    """Get relative path from base directory."""
    return str(file_path.relative_to(base_dir))


def process_file(client, source_file, dest_file, relative_path, tracking_data):
    """Process a single file - translate if needed or copy."""

    # Calculate source file hash and mtime
    source_md5 = calculate_md5(source_file)
    source_mtime = source_file.stat().st_mtime

    # Check if translation is needed
    needs_translation = False
    file_info = tracking_data["files"].get(relative_path, {})

    if not dest_file.exists():
        needs_translation = True
        print(f"NEW: {relative_path}")
    elif file_info.get("source_md5") != source_md5:
        needs_translation = True
        print(f"UPDATED: {relative_path}")
    else:
        print(f"SKIP (unchanged): {relative_path}")
        return False

    # Ensure destination directory exists
    dest_file.parent.mkdir(parents=True, exist_ok=True)

    # Handle translatable files
    if source_file.suffix.lower() in TRANSLATABLE_EXTENSIONS:
        with open(source_file, 'r', encoding='utf-8') as f:
            content = f.read()

        # Translate (check if C# file)
        is_csharp = source_file.suffix.lower() == '.cs'
        is_readme = source_file.name.upper() == 'README.MD'

        translated = translate_text(client, content, relative_path, is_csharp=is_csharp, is_readme=is_readme)

        # Add language notice to markdown files
        if not is_csharp and source_file.suffix.lower() == '.md':
            is_root = (relative_path == "README.md")
            translated = add_language_notice(translated, relative_path, is_root=is_root)

        # Save translated content
        with open(dest_file, 'w', encoding='utf-8') as f:
            f.write(translated)

        # Update tracking
        tracking_data["files"][relative_path] = {
            "source_md5": source_md5,
            "source_mtime": source_mtime,
            "translated_at": datetime.utcnow().isoformat() + "Z"
        }

        print(f"  ✓ Translated and saved")
        return True

    else:
        # Copy non-translatable files as-is
        shutil.copy2(source_file, dest_file)

        # Update tracking
        tracking_data["files"][relative_path] = {
            "source_md5": source_md5,
            "source_mtime": source_mtime,
            "copied_at": datetime.utcnow().isoformat() + "Z"
        }

        print(f"  ✓ Copied (non-translatable)")
        return True


def main():
    """Main execution function."""
    print("=" * 60)
    print("Japanese Translation Script")
    print("=" * 60)
    print(f"Source: {SOURCE_DIR}")
    print(f"Destination: {DEST_DIR}")
    print("=" * 60)

    # Load API key and initialize OpenAI client
    api_key = load_api_key()
    client = OpenAI(api_key=api_key)

    # Load tracking data
    tracking_data = load_tracking_data()

    # Ensure destination directory exists
    DEST_DIR.mkdir(parents=True, exist_ok=True)

    # Walk through source directory
    files_processed = 0
    files_skipped = 0

    for root, dirs, files in os.walk(SOURCE_DIR):
        root_path = Path(root)

        # Skip certain directories
        dirs[:] = [d for d in dirs if not should_skip(root_path / d)]

        for file_name in files:
            source_file = root_path / file_name

            # Skip certain files
            if should_skip(source_file):
                continue

            # Calculate destination path
            relative_path = get_relative_path(source_file, SOURCE_DIR)
            dest_file = DEST_DIR / relative_path

            # Process file
            try:
                if process_file(client, source_file, dest_file, relative_path, tracking_data):
                    files_processed += 1
                    # Save tracking data after each file
                    save_tracking_data(tracking_data)
                else:
                    files_skipped += 1

            except Exception as e:
                print(f"  ✗ ERROR: {e}")
                continue

    print("=" * 60)
    print(f"Translation complete!")
    print(f"Files processed: {files_processed}")
    print(f"Files skipped (unchanged): {files_skipped}")
    print("=" * 60)


if __name__ == "__main__":
    main()
