#!/usr/bin/env python3
"""
Translation Monitor - Every 5 minutes
Checks progress, errors, broken links, and API status
"""

import subprocess
import time
import re
from pathlib import Path
from datetime import datetime

def get_progress():
    """Get current translation progress."""
    source_dir = "/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"
    dest_dir = "/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp"

    total_md = int(subprocess.check_output(
        f'find {source_dir} -name "*.md" -type f -not -path "*/source-material/*" | wc -l',
        shell=True
    ).decode().strip())

    done_md = int(subprocess.check_output(
        f'find {dest_dir} -name "*.md" -type f | wc -l',
        shell=True
    ).decode().strip())

    total_cs = int(subprocess.check_output(
        f'find {source_dir} -name "*.cs" -type f -not -path "*/source-material/*" | wc -l',
        shell=True
    ).decode().strip())

    done_cs = int(subprocess.check_output(
        f'find {dest_dir} -name "*.cs" -type f | wc -l',
        shell=True
    ).decode().strip())

    return done_md, total_md, done_cs, total_cs


def check_for_errors():
    """Check translation log for errors."""
    log_file = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp/translation.log")

    if not log_file.exists():
        return []

    errors = []
    with open(log_file, 'r') as f:
        for line in f.readlines()[-100:]:  # Check last 100 lines
            if 'ERROR' in line or 'error' in line.lower():
                errors.append(line.strip())
            if '429' in line or 'rate limit' in line.lower():
                errors.append("⚠️  RATE LIMIT: " + line.strip())
            if 'quota' in line.lower():
                errors.append("⚠️  QUOTA: " + line.strip())

    return errors[-5:] if errors else []  # Return last 5 errors


def check_broken_links():
    """Check for broken or modified links in translated files."""
    dest_dir = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp")

    issues = []
    md_files = list(dest_dir.rglob("*.md"))[-10:]  # Check last 10 translated files

    for md_file in md_files:
        with open(md_file, 'r', encoding='utf-8') as f:
            content = f.read()

        # Check for UTM parameters (should not exist)
        if 'utm_' in content:
            issues.append(f"❌ UTM params found in: {md_file.name}")

        # Check for broken English links
        if 'github.com/iron-software/awesome-dotnet-pdf-libraries-2025' not in content:
            if md_file.name == 'README.md':
                issues.append(f"⚠️  Missing English link in: {md_file.name}")

        # Check for malformed URLs
        urls = re.findall(r'https://[^\s\)]+', content)
        for url in urls:
            if ' ' in url or url.endswith(','):
                issues.append(f"⚠️  Malformed URL in {md_file.name}: {url[:50]}")

    return issues[:5] if issues else []  # Return first 5 issues


def check_script_running():
    """Check if translation script is still running."""
    try:
        result = subprocess.check_output(
            "ps aux | grep 'translate_to_japanese.py' | grep -v grep",
            shell=True
        ).decode().strip()
        return True if result else False
    except:
        return False


def main():
    """Main monitoring loop."""
    print("🔍 Translation Monitor Started")
    print("=" * 80)
    print("Checking every 5 minutes for:")
    print("  - Translation progress")
    print("  - API errors / rate limits")
    print("  - Broken or modified links")
    print("  - Script health")
    print("=" * 80)
    print()

    while True:
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")

        print(f"\n{'='*80}")
        print(f"📊 STATUS UPDATE - {timestamp}")
        print(f"{'='*80}")

        # Check progress
        done_md, total_md, done_cs, total_cs = get_progress()
        total = total_md + total_cs
        done = done_md + done_cs
        percent = (done * 100) // total if total > 0 else 0
        remaining = total - done

        print(f"\n🇯🇵 TRANSLATION PROGRESS")
        print(f"  Total:       {done}/{total} files ({percent}%)")
        print(f"  Remaining:   {remaining} files")
        print(f"  Markdown:    {done_md}/{total_md} ({done_md*100//total_md if total_md > 0 else 0}%)")
        print(f"  C# files:    {done_cs}/{total_cs} ({done_cs*100//total_cs if total_cs > 0 else 0}%)")

        # Estimate completion
        if done > 0:
            # Rough estimate: ~30 seconds per file average
            est_minutes = remaining * 0.5
            est_hours = est_minutes / 60
            print(f"  Estimated:   ~{est_hours:.1f} hours remaining")

        # Check script health
        script_running = check_script_running()
        print(f"\n🔧 SCRIPT STATUS")
        print(f"  Running:     {'✅ YES' if script_running else '❌ NO - STOPPED!'}")

        if not script_running:
            print(f"\n⚠️  WARNING: Translation script is not running!")
            print(f"   Restart with: python3 translate_to_japanese.py > translation.log 2>&1 &")

        # Check for errors
        errors = check_for_errors()
        if errors:
            print(f"\n❌ ERRORS DETECTED ({len(errors)})")
            for error in errors:
                print(f"  {error}")
        else:
            print(f"\n✅ NO ERRORS")

        # Check links
        link_issues = check_broken_links()
        if link_issues:
            print(f"\n⚠️  LINK ISSUES ({len(link_issues)})")
            for issue in link_issues:
                print(f"  {issue}")
        else:
            print(f"\n✅ LINKS OK")

        # Cost estimate
        cost_so_far = done_md * 0.08
        cost_remaining = (total_md - done_md) * 0.08
        print(f"\n💰 ESTIMATED COST")
        print(f"  So far:      ~${cost_so_far:.2f}")
        print(f"  Remaining:   ~${cost_remaining:.2f}")
        print(f"  Total:       ~${cost_so_far + cost_remaining:.2f}")

        print(f"\n{'='*80}")
        print(f"⏰ Next update in 5 minutes...")
        print(f"{'='*80}\n")

        # Wait 5 minutes
        time.sleep(300)


if __name__ == "__main__":
    main()
