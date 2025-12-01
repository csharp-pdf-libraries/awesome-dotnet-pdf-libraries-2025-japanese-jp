#!/usr/bin/env python3
"""
Master Translation Manager
Manages translation and GitHub deployment for all language variants
"""

import os
import json
import subprocess
from pathlib import Path

# Language configurations
LANGUAGES = {
    # Tier 1 (Highest Priority)
    'ja': {'name': 'Japanese', 'suffix': 'jp', 'tier': 1, 'locale': 'ja_JP'},
    'zh-CN': {'name': 'Simplified Chinese', 'suffix': 'zh-cn', 'tier': 1, 'locale': 'zh_CN'},
    'ko': {'name': 'Korean', 'suffix': 'kr', 'tier': 1, 'locale': 'ko_KR'},
    'pt-BR': {'name': 'Portuguese (Brazil)', 'suffix': 'pt-br', 'tier': 1, 'locale': 'pt_BR'},
    'es': {'name': 'Spanish', 'suffix': 'es', 'tier': 1, 'locale': 'es_ES'},

    # Tier 2 (High Priority)
    'de': {'name': 'German', 'suffix': 'de', 'tier': 2, 'locale': 'de_DE'},
    'fr': {'name': 'French', 'suffix': 'fr', 'tier': 2, 'locale': 'fr_FR'},
    'ru': {'name': 'Russian', 'suffix': 'ru', 'tier': 2, 'locale': 'ru_RU'},
    'zh-TW': {'name': 'Traditional Chinese', 'suffix': 'zh-tw', 'tier': 2, 'locale': 'zh_TW'},
    'it': {'name': 'Italian', 'suffix': 'it', 'tier': 2, 'locale': 'it_IT'},

    # Tier 3 (Growing Markets)
    'pl': {'name': 'Polish', 'suffix': 'pl', 'tier': 3, 'locale': 'pl_PL'},
    'tr': {'name': 'Turkish', 'suffix': 'tr', 'tier': 3, 'locale': 'tr_TR'},
    'nl': {'name': 'Dutch', 'suffix': 'nl', 'tier': 3, 'locale': 'nl_NL'},
    'hi': {'name': 'Hindi', 'suffix': 'in', 'tier': 3, 'locale': 'hi_IN'},
    'id': {'name': 'Indonesian', 'suffix': 'id', 'tier': 3, 'locale': 'id_ID'},
    'vi': {'name': 'Vietnamese', 'suffix': 'vn', 'tier': 3, 'locale': 'vi_VN'},
    'th': {'name': 'Thai', 'suffix': 'th', 'tier': 3, 'locale': 'th_TH'},
}

# Paths
BASE_DIR = Path("/Users/jacob/Sites")
SOURCE_REPO = BASE_DIR / "awesome-dotnet-pdf-libraries-2025"
GITHUB_TOKEN_FILE = Path("/Users/jacob/Sites/SEO-MACHINE/GITHUB_TOKEN.txt")
OPENAI_KEY_FILE = Path("/Users/jacob/Sites/SEO-MACHINE/OPENAIKEY.txt")

# GitHub configuration
GITHUB_ORG = "csharp-pdf-libraries"
GITHUB_USER = "jacob-mellor"  # Committing as Jacob Mellor


def load_github_token():
    """Load GitHub personal access token."""
    if GITHUB_TOKEN_FILE.exists():
        return GITHUB_TOKEN_FILE.read_text().strip()
    else:
        print(f"⚠️  GitHub token not found at {GITHUB_TOKEN_FILE}")
        print("Please create a GitHub Personal Access Token with 'repo' permissions")
        print("and save it to this file.")
        return None


def create_github_repo(lang_code, lang_info, github_token):
    """Create a GitHub repository for a language variant."""
    repo_name = f"awesome-dotnet-pdf-libraries-2025-{lang_info['suffix']}"
    description = f"🌐 {lang_info['name']} translation - Comprehensive comparison of C#/.NET PDF libraries"

    print(f"\n📦 Creating GitHub repository: {repo_name}")

    # Create repository using GitHub API
    import requests

    headers = {
        'Authorization': f'token {github_token}',
        'Accept': 'application/vnd.github.v3+json'
    }

    data = {
        'name': repo_name,
        'description': description,
        'homepage': 'https://ironpdf.com',
        'private': False,
        'has_issues': True,
        'has_projects': False,
        'has_wiki': False,
        'auto_init': False
    }

    # Try organization first, fall back to user
    url = f'https://api.github.com/orgs/{GITHUB_ORG}/repos'
    response = requests.post(url, headers=headers, json=data)

    if response.status_code == 404:
        # Try user repos instead
        url = 'https://api.github.com/user/repos'
        response = requests.post(url, headers=headers, json=data)

    if response.status_code == 201:
        repo_url = response.json()['html_url']
        print(f"  ✅ Repository created: {repo_url}")
        return repo_url
    elif response.status_code == 422 and 'already exists' in response.text:
        print(f"  ℹ️  Repository already exists")
        return f"https://github.com/{GITHUB_ORG}/{repo_name}"
    else:
        print(f"  ❌ Failed to create repository: {response.status_code}")
        print(f"     {response.text}")
        return None


def initialize_local_repo(lang_code, lang_info, repo_url):
    """Initialize local git repository."""
    repo_dir = BASE_DIR / f"awesome-dotnet-pdf-libraries-2025-{lang_info['suffix']}"

    if not repo_dir.exists():
        repo_dir.mkdir(parents=True)

    os.chdir(repo_dir)

    # Initialize git if needed
    if not (repo_dir / '.git').exists():
        subprocess.run(['git', 'init'], check=True)
        subprocess.run(['git', 'remote', 'add', 'origin', repo_url.replace('https://', f'https://{load_github_token()}@')], check=True)

    print(f"  ✅ Local repository initialized at {repo_dir}")
    return repo_dir


def deploy_to_github(repo_dir, lang_info):
    """Deploy completed translation to GitHub."""
    os.chdir(repo_dir)

    print(f"\n🚀 Deploying {lang_info['name']} to GitHub...")

    # Configure git user (Jacob Mellor)
    subprocess.run(['git', 'config', 'user.name', 'Jacob Mellor'], check=True)
    subprocess.run(['git', 'config', 'user.email', 'jacob@ironsoftware.com'], check=True)

    # Add all files
    subprocess.run(['git', 'add', '.'], check=True)

    # Check if there are changes
    result = subprocess.run(['git', 'status', '--porcelain'], capture_output=True, text=True)
    if not result.stdout.strip():
        print("  ℹ️  No changes to commit")
        return

    # Commit
    commit_message = f"""Update {lang_info['name']} translation - {subprocess.check_output(['date', '+%Y-%m-%d']).decode().strip()}

🌐 Automated translation to {lang_info['name']}
📝 Translated by Claude AI with ChatGPT API
✅ Generated with Claude Code

Co-Authored-By: Claude <noreply@anthropic.com>"""

    subprocess.run(['git', 'commit', '-m', commit_message], check=True)

    # Push
    subprocess.run(['git', 'push', '-u', 'origin', 'main'], check=True)

    print(f"  ✅ Deployed successfully!")


def check_translation_progress(repo_dir):
    """Check if translation is complete."""
    tracking_file = repo_dir / "translation_tracking.json"

    if not tracking_file.exists():
        return 0, 0

    with open(tracking_file) as f:
        tracking = json.load(f)

    total_files = len([f for f in SOURCE_REPO.rglob('*') if f.is_file() and f.suffix in ['.md', '.cs'] and 'source-material' not in str(f)])
    completed = len(tracking.get('files', {}))

    return completed, total_files


def main():
    """Main execution."""
    print("=" * 80)
    print("Master Translation Manager")
    print("Managing translations across all language variants")
    print("=" * 80)

    github_token = load_github_token()
    if not github_token:
        print("\n❌ Cannot proceed without GitHub token")
        return

    print(f"\nLanguages to process: {len(LANGUAGES)}")

    # Group by tier
    for tier in [1, 2, 3]:
        tier_langs = {k: v for k, v in LANGUAGES.items() if v['tier'] == tier}
        print(f"\n{'='*80}")
        print(f"Tier {tier}: {len(tier_langs)} languages")
        print('='*80)

        for lang_code, lang_info in tier_langs.items():
            print(f"\n🌐 Processing: {lang_info['name']} ({lang_code})")

            # Create GitHub repository
            repo_url = create_github_repo(lang_code, lang_info, github_token)
            if not repo_url:
                print(f"  ⚠️  Skipping {lang_info['name']} due to repository creation failure")
                continue

            # Initialize local repository
            repo_dir = initialize_local_repo(lang_code, lang_info, repo_url)

            # Check translation status
            completed, total = check_translation_progress(repo_dir)

            if completed >= total and total > 0:
                print(f"  ✅ Translation complete ({completed}/{total} files)")

                # Deploy to GitHub
                deploy_to_github(repo_dir, lang_info)
            else:
                print(f"  ⏳ Translation in progress: {completed}/{total} files ({completed*100//total if total > 0 else 0}%)")
                print(f"     Repository ready, translation script needs to be started")

    print("\n" + "="*80)
    print("Summary complete!")
    print("="*80)


if __name__ == "__main__":
    main()
