#!/bin/bash
# Auto-update script - Reports progress every 5 minutes

SOURCE_DIR="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"
DEST_DIR="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp"

while true; do
    python3 -c "
import subprocess
from datetime import datetime

timestamp = datetime.now().strftime('%Y-%m-%d %H:%M:%S')

source_dir = '$SOURCE_DIR'
dest_dir = '$DEST_DIR'

total_md = int(subprocess.check_output(f'find {source_dir} -name \"*.md\" -type f -not -path \"*/source-material/*\" | wc -l', shell=True).decode().strip())
done_md = int(subprocess.check_output(f'find {dest_dir} -name \"*.md\" -type f | wc -l', shell=True).decode().strip())
total_cs = int(subprocess.check_output(f'find {source_dir} -name \"*.cs\" -type f -not -path \"*/source-material/*\" | wc -l', shell=True).decode().strip())
done_cs = int(subprocess.check_output(f'find {dest_dir} -name \"*.cs\" -type f | wc -l', shell=True).decode().strip())

total = total_md + total_cs
done = done_md + done_cs
percent = (done * 100) // total if total > 0 else 0
remaining = total - done

md_percent = (done_md * 100) // total_md if total_md > 0 else 0
cs_percent = (done_cs * 100) // total_cs if total_cs > 0 else 0

cost = done_md * 0.08
estimated_total = total_md * 0.08

print()
print('='*75)
print(f'🇯🇵 TRANSLATION STATUS - {timestamp}')
print('='*75)
print(f'⚡ Mode:        4 Parallel Workers')
print(f'📊 Progress:    {done}/{total} files ({percent}%)')
print(f'⏳ Remaining:   {remaining} files')
print()
print(f'📝 Markdown:    {done_md}/{total_md} ({md_percent}%)')
print(f'💻 C# Files:    {done_cs}/{total_cs} ({cs_percent}%)')
print()
print(f'💰 Cost:        \${cost:.2f} / \${estimated_total:.2f}')
print(f'📈 Progress:    {cost*100//estimated_total if estimated_total > 0 else 0}% of budget spent')
print('='*75)
print('✅ Next update in 5 minutes...')
print()
"
    sleep 300  # 5 minutes
done
