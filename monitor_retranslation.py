#!/usr/bin/env python3
import json
import time
from pathlib import Path
from hashlib import md5
from datetime import datetime

def check_retranslation_progress():
    tracking_file = Path("translation_tracking.json")
    tracking_data = json.load(open(tracking_file))
    source_dir = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025")
    
    needs_update = 0
    updated = 0
    total = 0
    
    for relative_path, file_info in tracking_data["files"].items():
        source_file = source_dir / relative_path
        if source_file.exists():
            total += 1
            current_md5 = md5(source_file.read_bytes()).hexdigest()
            stored_md5 = file_info.get("source_md5", "")
            if current_md5 != stored_md5:
                needs_update += 1
            else:
                updated += 1
    
    percent = (updated * 100 // total) if total > 0 else 0
    cost = needs_update * 0.10
    
    # Calculate ETA (assume 1 file per 20 seconds with 4 workers)
    if needs_update > 0:
        eta_seconds = (needs_update * 20) // 4
        eta_minutes = eta_seconds // 60
        eta_str = f"{eta_minutes}m" if eta_minutes > 0 else f"{eta_seconds}s"
    else:
        eta_str = "Complete"
    
    timestamp = datetime.now().strftime('%I:%M %p')
    
    print(f"\n{'='*50}")
    print(f"📊 Retranslation Progress - {timestamp}")
    print(f"{'='*50}")
    print(f"✅ Up-to-date:     {updated}/{total} ({percent}%)")
    print(f"⏳ Remaining:      {needs_update} files")
    print(f"💰 Cost:           ${cost:.2f}")
    print(f"⏱️  ETA:            {eta_str}")
    print(f"🔧 Workers:        4 threads")
    print(f"{'='*50}\n")
    
    return needs_update == 0

while True:
    try:
        complete = check_retranslation_progress()
        if complete:
            print("✨ All markdown files from the last 30 minutes have been translated!")
            break
        time.sleep(120)  # Check every 2 minutes
    except KeyboardInterrupt:
        print("\n\nMonitoring stopped.")
        break
    except Exception as e:
        print(f"Error: {e}")
        time.sleep(120)
