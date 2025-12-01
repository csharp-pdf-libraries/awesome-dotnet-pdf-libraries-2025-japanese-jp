#!/usr/bin/env python3
"""
Cleanup Script - Delete premature language repositories
Keep only: English (iron-software) and Japanese (csharp-pdf-libraries)
"""

import requests
import subprocess
from pathlib import Path

# GitHub configuration
GITHUB_TOKEN_FILE = Path("/Users/jacob/Sites/SEO-MACHINE/GITHUB_TOKEN.txt")
GITHUB_ORG = "csharp-pdf-libraries"

# Repositories to DELETE (all except Japanese)
REPOS_TO_DELETE = [
    "awesome-dotnet-pdf-libraries-2025-zh-cn",
    "awesome-dotnet-pdf-libraries-2025-kr",
    "awesome-dotnet-pdf-libraries-2025-pt-br",
    "awesome-dotnet-pdf-libraries-2025-es",
    "awesome-dotnet-pdf-libraries-2025-de",
    "awesome-dotnet-pdf-libraries-2025-fr",
    "awesome-dotnet-pdf-libraries-2025-ru",
    "awesome-dotnet-pdf-libraries-2025-zh-tw",
    "awesome-dotnet-pdf-libraries-2025-it",
    "awesome-dotnet-pdf-libraries-2025-pl",
    "awesome-dotnet-pdf-libraries-2025-tr",
    "awesome-dotnet-pdf-libraries-2025-nl",
    "awesome-dotnet-pdf-libraries-2025-in",
    "awesome-dotnet-pdf-libraries-2025-id",
    "awesome-dotnet-pdf-libraries-2025-vn",
    "awesome-dotnet-pdf-libraries-2025-th",
]

def load_github_token():
    """Load GitHub token."""
    return GITHUB_TOKEN_FILE.read_text().strip()

def delete_github_repo(repo_name, token):
    """Delete a GitHub repository."""
    headers = {
        'Authorization': f'token {token}',
        'Accept': 'application/vnd.github.v3+json'
    }

    url = f'https://api.github.com/repos/{GITHUB_ORG}/{repo_name}'

    print(f"🗑️  Deleting: {repo_name}")
    response = requests.delete(url, headers=headers)

    if response.status_code == 204:
        print(f"   ✅ Deleted successfully")
        return True
    elif response.status_code == 404:
        print(f"   ℹ️  Repository doesn't exist (already deleted?)")
        return True
    else:
        print(f"   ❌ Failed: {response.status_code} - {response.text}")
        return False

def delete_local_directory(repo_name):
    """Delete local directory."""
    local_path = Path(f"/Users/jacob/Sites/{repo_name}")

    if local_path.exists():
        print(f"🗑️  Deleting local: {local_path}")
        subprocess.run(['rm', '-rf', str(local_path)], check=True)
        print(f"   ✅ Deleted")
    else:
        print(f"   ℹ️  Local directory doesn't exist")

def main():
    """Main cleanup."""
    print("="*80)
    print("GitHub Repository Cleanup")
    print("="*80)
    print("Keeping only:")
    print("  - awesome-dotnet-pdf-libraries-2025 (English - iron-software org)")
    print("  - awesome-dotnet-pdf-libraries-2025-jp (Japanese - csharp-pdf-libraries)")
    print()
    print(f"Deleting {len(REPOS_TO_DELETE)} premature repositories...")
    print("="*80)

    token = load_github_token()

    for repo_name in REPOS_TO_DELETE:
        print()

        # Delete from GitHub
        delete_github_repo(repo_name, token)

        # Delete local directory
        delete_local_directory(repo_name)

    print()
    print("="*80)
    print("✅ Cleanup Complete!")
    print("="*80)
    print()
    print("Remaining repositories:")
    print("  1. awesome-dotnet-pdf-libraries-2025 (English)")
    print("  2. awesome-dotnet-pdf-libraries-2025-jp (Japanese)")
    print()
    print("Other languages will be created one at a time as translations complete.")
    print("="*80)

if __name__ == "__main__":
    main()
