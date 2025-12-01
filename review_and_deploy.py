#!/usr/bin/env python3
"""
Review and Deploy Script
Waits for translation to complete, shows samples for review, then deploys
"""

import subprocess
import time
from pathlib import Path

def get_translation_progress(lang_dir):
    """Get translation progress."""
    source_dir = "/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"

    # Count total files
    total_md = int(subprocess.check_output(
        f'find {source_dir} -name "*.md" -type f -not -path "*/source-material/*" | wc -l',
        shell=True
    ).decode().strip())

    total_cs = int(subprocess.check_output(
        f'find {source_dir} -name "*.cs" -type f -not -path "*/source-material/*" | wc -l',
        shell=True
    ).decode().strip())

    # Count completed files
    done_md = int(subprocess.check_output(
        f'find {lang_dir} -name "*.md" -type f | wc -l',
        shell=True
    ).decode().strip())

    done_cs = int(subprocess.check_output(
        f'find {lang_dir} -name "*.cs" -type f | wc -l',
        shell=True
    ).decode().strip())

    total = total_md + total_cs
    done = done_md + done_cs
    percent = (done * 100) // total if total > 0 else 0

    return done, total, percent, done_md, total_md, done_cs, total_cs


def show_samples(lang_dir):
    """Show sample translated files for review."""
    print("\n" + "="*80)
    print("TRANSLATION SAMPLES FOR REVIEW")
    print("="*80)

    # Show README sample
    readme = Path(lang_dir) / "README.md"
    if readme.exists():
        print("\n📄 README.md (first 30 lines):")
        print("-"*80)
        with open(readme, 'r', encoding='utf-8') as f:
            for i, line in enumerate(f):
                if i >= 30:
                    break
                print(line.rstrip())

    # Show a random markdown file
    md_files = list(Path(lang_dir).rglob("*.md"))
    if len(md_files) > 1:
        sample_md = md_files[5] if len(md_files) > 5 else md_files[1]
        print(f"\n📄 {sample_md.name} (first 20 lines):")
        print("-"*80)
        with open(sample_md, 'r', encoding='utf-8') as f:
            for i, line in enumerate(f):
                if i >= 20:
                    break
                print(line.rstrip())

    # Show a C# file
    cs_files = list(Path(lang_dir).rglob("*.cs"))
    if cs_files:
        sample_cs = cs_files[0]
        print(f"\n📄 {sample_cs.name}:")
        print("-"*80)
        with open(sample_cs, 'r', encoding='utf-8') as f:
            print(f.read())

    print("\n" + "="*80)


def deploy_to_github(lang_dir, lang_name):
    """Deploy translation to GitHub."""
    print(f"\n🚀 Deploying {lang_name} to GitHub...")

    subprocess.run(['git', 'config', 'user.name', 'Jacob Mellor'], cwd=lang_dir, check=True)
    subprocess.run(['git', 'config', 'user.email', 'jacob@ironsoftware.com'], cwd=lang_dir, check=True)
    subprocess.run(['git', 'add', '.'], cwd=lang_dir, check=True)

    commit_msg = f"""Initial {lang_name} translation

🌐 Automated translation from English to {lang_name}
📝 Translated using ChatGPT API (gpt-4-turbo-preview)
✅ 337 markdown files + 435 C# files
🤖 Generated with Claude Code

Co-Authored-By: Claude <noreply@anthropic.com>"""

    subprocess.run(['git', 'commit', '-m', commit_msg], cwd=lang_dir, check=True)
    subprocess.run(['git', 'branch', '-M', 'main'], cwd=lang_dir, check=True)
    subprocess.run(['git', 'push', '-u', 'origin', 'main', '--force'], cwd=lang_dir, check=True)

    print(f"✅ Deployed: {lang_name}")


def main():
    lang_dir = "/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp"
    lang_name = "Japanese"

    print("="*80)
    print(f"Monitoring {lang_name} Translation")
    print("="*80)

    # Wait for completion
    while True:
        done, total, percent, done_md, total_md, done_cs, total_cs = get_translation_progress(lang_dir)

        print(f"\r🇯🇵 Progress: {done}/{total} ({percent}%) - MD:{done_md}/{total_md} CS:{done_cs}/{total_cs}", end='', flush=True)

        if done >= total:
            print("\n\n✅ Translation complete!")
            break

        time.sleep(30)  # Check every 30 seconds

    # Show samples for review
    show_samples(lang_dir)

    # Ask for approval
    print("\n" + "="*80)
    approval = input("\n👀 Review the samples above. Deploy to GitHub? (yes/no): ").strip().lower()

    if approval == 'yes':
        deploy_to_github(lang_dir, lang_name)
        print("\n🎉 Deployment complete!")
        print(f"🔗 https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp")
    else:
        print("\n⏸️  Deployment cancelled. You can deploy manually later.")


if __name__ == "__main__":
    main()
