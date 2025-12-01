#!/usr/bin/env python3
"""
Restructure library READMEs to include embedded C# code examples and migration summaries.
"""

import os
import re
import glob
from pathlib import Path
from typing import Dict, List, Tuple, Optional

# Operation name to question heading mapping
OPERATION_HEADINGS = {
    "html-to-pdf": "How Do I Convert HTML to PDF in C# with {library}?",
    "url-to-pdf": "How Do I Convert a URL to PDF in .NET?",
    "html-file-to-pdf": "How Do I Convert an HTML File to PDF?",
    "html-string-to-pdf": "How Do I Convert an HTML String to PDF?",
    "html-file-to-pdf-header-footer": "How Do I Convert HTML Files with Headers and Footers?",
    "html-file-custom-pdf": "How Do I Create Custom PDFs from HTML Files?",
    "merge-pdfs": "How Do I Merge Multiple PDFs in C#?",
    "add-text-to-pdf": "How Do I Add Text to an Existing PDF?",
    "create-pdf-with-image": "How Do I Create a PDF with Images?",
    "create-table-pdf": "How Do I Create a PDF with Tables?",
    "create-invoice-pdf": "How Do I Generate Invoice PDFs in C#?",
    "header-footer-pdf": "How Do I Add Headers and Footers to PDFs?",
    "pdf-headers-footers": "How Do I Add Headers and Footers to PDFs?",
    "custom-page-settings": "How Do I Set Custom Page Settings in PDFs?",
    "html-to-pdf-custom-settings": "How Do I Use Custom Rendering Settings?",
    "html-to-pdf-custom-size": "How Do I Create Custom-Sized PDFs?",
}

def get_library_folders() -> List[str]:
    """Get all library folders excluding FAQ and ironpdf."""
    folders = []
    for item in sorted(os.listdir('.')):
        path = Path(item)
        if path.is_dir() and item not in ['faq', 'FAQ', 'ironpdf', '.git', '__pycache__', 'source-material']:
            # Check if it has a README.md
            if (path / 'README.md').exists():
                folders.append(item)
    return folders

def parse_operation_from_filename(filename: str, library_slug: str) -> Optional[str]:
    """Extract operation name from .cs filename."""
    # Remove extension
    name = filename.replace('.cs', '')

    # Remove library suffix or ironpdf suffix
    if name.endswith(f'-{library_slug}'):
        operation = name[:-len(f'-{library_slug}')]
    elif name.endswith('-ironpdf'):
        operation = name[:-len('-ironpdf')]
    else:
        return None

    return operation

def get_cs_file_pairs(library_folder: str, library_slug: str) -> List[Tuple[str, str, str]]:
    """Get pairs of .cs files (operation, library_file, ironpdf_file)."""
    cs_files = glob.glob(f"{library_folder}/*.cs")
    operations = {}

    for cs_file in cs_files:
        filename = os.path.basename(cs_file)
        operation = parse_operation_from_filename(filename, library_slug)

        if operation:
            if operation not in operations:
                operations[operation] = {'library': None, 'ironpdf': None}

            if filename.endswith('-ironpdf.cs'):
                operations[operation]['ironpdf'] = cs_file
            elif filename.endswith(f'-{library_slug}.cs'):
                operations[operation]['library'] = cs_file

    # Return only complete pairs
    pairs = []
    for operation, files in operations.items():
        if files['library'] and files['ironpdf']:
            pairs.append((operation, files['library'], files['ironpdf']))

    return pairs

def read_file_content(filepath: str) -> str:
    """Read file content."""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            return f.read()
    except Exception as e:
        print(f"Error reading {filepath}: {e}")
        return ""

def extract_library_name(readme_content: str, library_slug: str) -> str:
    """Extract library display name from README title or folder name."""
    # Look for first heading
    match = re.search(r'^#\s+(.+?)$', readme_content, re.MULTILINE)
    if match:
        title = match.group(1).strip()

        # Handle "Comparing X and IronPDF" format
        comparing_match = re.search(r'Comparing\s+(.+?)\s+and\s+IronPDF', title, re.IGNORECASE)
        if comparing_match:
            return comparing_match.group(1).strip()

        # Handle "X + C# + PDF" format
        name = re.sub(r'\s+\+\s+C#.*$', '', title)
        name = re.sub(r'\s+vs\.?\s+IronPDF.*$', '', name, flags=re.IGNORECASE)
        name = re.sub(r'\s+for\s+C#.*$', '', name, flags=re.IGNORECASE)

        if name and name.lower() != 'comparing':
            return name.strip()

    # Fallback: use capitalized slug
    return library_slug.replace('-', ' ').title()

def get_operation_heading(operation: str, library_name: str) -> str:
    """Get question-based heading for an operation."""
    if operation in OPERATION_HEADINGS:
        template = OPERATION_HEADINGS[operation]
        return template.format(library=library_name)

    # Fallback: convert operation to title case question
    # Special handling for common patterns
    op_lower = operation.lower()
    if 'html-file' in op_lower and 'custom' in op_lower:
        return "How Do I Convert HTML Files to PDF with Custom Settings?"
    elif 'html-file' in op_lower and ('header' in op_lower or 'footer' in op_lower):
        return "How Do I Convert HTML Files with Headers and Footers?"

    # Generic fallback
    words = operation.replace('-', ' ').title()
    # Fix common awkward phrasings
    words = words.replace('Pdf', 'PDF').replace('Html', 'HTML')
    return f"How Do I {words}?"

def generate_code_comparison_section(operation: str, library_name: str, library_slug: str,
                                     library_code: str, ironpdf_code: str) -> str:
    """Generate a code comparison section for an operation."""
    heading = get_operation_heading(operation, library_name)

    section = f"\n---\n\n## {heading}\n\n"
    section += f"Here's how **{library_name}** handles this:\n\n"
    section += f"```csharp\n{library_code.strip()}\n```\n\n"
    section += f"**With IronPDF**, the same task is simpler and more intuitive:\n\n"
    section += f"```csharp\n{ironpdf_code.strip()}\n```\n\n"

    # Add brief comparison note
    section += f"IronPDF's approach offers cleaner syntax and better integration with modern .NET applications, "
    section += f"making it easier to maintain and scale your PDF generation workflows.\n"

    return section

def extract_migration_summary(migration_file: str, library_name: str) -> Optional[str]:
    """Extract key points from migration guide for summary."""
    if not os.path.exists(migration_file):
        return None

    content = read_file_content(migration_file)
    if not content:
        return None

    summary = f"\n---\n\n## How Can I Migrate from {library_name} to IronPDF?\n\n"

    # Extract "Why Migrate" section
    why_migrate = re.search(r'##\s+Why Migrate[^\n]*\n+(.*?)(?=\n##|\Z)', content, re.DOTALL)
    if why_migrate:
        reason = why_migrate.group(1).strip()
        # Take first 2-3 sentences
        sentences = re.split(r'(?<=[.!?])\s+', reason)
        summary += ' '.join(sentences[:2]) + "\n\n"

    summary += f"**Migrating from {library_name} to IronPDF involves:**\n\n"

    # Extract NuGet package info
    nuget_match = re.search(r'dotnet remove package\s+(\S+)', content)
    if nuget_match:
        old_package = nuget_match.group(1)
        summary += f"1. **NuGet Package Change**: Remove `{old_package}`, add `IronPdf`\n"
    else:
        summary += f"1. **NuGet Package Change**: Install `IronPdf` package\n"

    # Extract namespace info
    namespace_match = re.search(r'\|\s*`([^`]+)`\s*\|\s*`IronPdf`\s*\|', content)
    if namespace_match:
        old_namespace = namespace_match.group(1)
        summary += f"2. **Namespace Update**: Replace `{old_namespace}` with `IronPdf`\n"
    else:
        summary += f"2. **Namespace Update**: Use `IronPdf` namespace\n"

    summary += f"3. **API Adjustments**: Update your code to use IronPDF's modern API patterns\n\n"

    summary += f"**Key Benefits of Migrating:**\n\n"
    summary += f"- Modern Chromium rendering engine with full CSS/JavaScript support\n"
    summary += f"- Active maintenance and security updates\n"
    summary += f"- Better .NET integration and async/await support\n"
    summary += f"- Comprehensive documentation and professional support\n\n"

    migration_link = os.path.basename(migration_file)
    summary += f"For a complete step-by-step migration guide with detailed code examples and common gotchas, see:\n"
    summary += f"**[Complete Migration Guide: {library_name} → IronPDF]({migration_link})**\n"

    return summary

def find_insertion_point(readme_content: str) -> int:
    """Find where to insert new code sections (after weaknesses, before comparison table)."""
    # Look for "Comparison Table" or "C# Code Example with IronPDF"
    patterns = [
        r'\n##\s+Comparison Table',
        r'\n##\s+C#\s+Code\s+Example\s+with\s+IronPDF',
        r'\n##\s+Comparing.*IronPDF',
        r'\n##\s+Further Learning',
    ]

    for pattern in patterns:
        match = re.search(pattern, readme_content, re.IGNORECASE)
        if match:
            return match.start()

    # Fallback: before "## Conclusion" or last section
    conclusion = re.search(r'\n##\s+Conclusion', readme_content, re.IGNORECASE)
    if conclusion:
        return conclusion.start()

    # Last resort: end of content before "Related Tutorials"
    related = re.search(r'\n##\s+Related', readme_content)
    if related:
        return related.start()

    return len(readme_content)

def restructure_readme(library_folder: str, library_slug: str) -> bool:
    """Restructure a single library's README."""
    readme_path = f"{library_folder}/README.md"

    if not os.path.exists(readme_path):
        print(f"  ⚠ No README.md found")
        return False

    # Read existing README
    readme_content = read_file_content(readme_path)
    if not readme_content:
        return False

    # Extract library name
    library_name = extract_library_name(readme_content, library_slug)

    # Get C# file pairs
    cs_pairs = get_cs_file_pairs(library_folder, library_slug)

    if not cs_pairs:
        print(f"  ⚠ No C# file pairs found")
        # Still try to add migration summary

    # Generate code sections
    code_sections = []
    for operation, library_file, ironpdf_file in cs_pairs:
        library_code = read_file_content(library_file)
        ironpdf_code = read_file_content(ironpdf_file)

        if library_code and ironpdf_code:
            section = generate_code_comparison_section(
                operation, library_name, library_slug,
                library_code, ironpdf_code
            )
            code_sections.append(section)

    # Generate migration summary
    migration_file = f"{library_folder}/migrate-from-{library_slug}.md"
    migration_summary = extract_migration_summary(migration_file, library_name)

    if not code_sections and not migration_summary:
        print(f"  ⚠ Nothing to add")
        return False

    # Find insertion point
    insertion_point = find_insertion_point(readme_content)

    # Build new content
    new_sections = ''.join(code_sections)
    if migration_summary:
        new_sections += migration_summary

    # Insert new sections
    new_readme = (
        readme_content[:insertion_point] +
        new_sections +
        "\n" +
        readme_content[insertion_point:]
    )

    # Write updated README
    try:
        with open(readme_path, 'w', encoding='utf-8') as f:
            f.write(new_readme)
        print(f"  ✓ Updated ({len(code_sections)} code sections, {'migration summary' if migration_summary else 'no migration'})")
        return True
    except Exception as e:
        print(f"  ✗ Error writing: {e}")
        return False

def main():
    """Main execution function."""
    print("Restructuring library READMEs...\n")

    # Get all library folders
    folders = get_library_folders()
    print(f"Found {len(folders)} library folders\n")

    success_count = 0
    skip_count = 0

    for folder in folders:
        library_slug = folder
        print(f"Processing {library_slug}...")

        if restructure_readme(folder, library_slug):
            success_count += 1
        else:
            skip_count += 1

    print(f"\n" + "="*50)
    print(f"Complete!")
    print(f"  ✓ Successfully updated: {success_count}")
    print(f"  ⚠ Skipped: {skip_count}")
    print(f"  Total: {len(folders)}")

if __name__ == "__main__":
    main()
