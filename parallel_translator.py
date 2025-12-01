#!/usr/bin/env python3
"""
Parallel Translation Engine
Uses multiple workers to translate files simultaneously with chunk-based locking
"""

import os
import json
import hashlib
import shutil
import threading
import queue
from pathlib import Path
from datetime import datetime
from openai import OpenAI
import time

# Configuration
SOURCE_DIR = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025")
DEST_DIR = Path("/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp")
API_KEY_FILE = Path("/Users/jacob/Sites/SEO-MACHINE/OPENAIKEY.txt")
TRACKING_FILE = DEST_DIR / "translation_tracking.json"
LOCK_FILE = DEST_DIR / "translation.lock"

# Parallel settings
NUM_WORKERS = 4  # Number of parallel translation threads

# File extensions to translate
TRANSLATABLE_EXTENSIONS = {'.md', '.cs'}

# Skip patterns
SKIP_PATTERNS = {'.git', '__pycache__', 'node_modules', '.DS_Store',
                 'translation_tracking.json', '.gitignore',
                 'source-material', 'source material', 'translation.lock'}


class TranslationWorker(threading.Thread):
    """Worker thread for parallel translation."""

    def __init__(self, worker_id, file_queue, api_key, results_queue, lock):
        super().__init__()
        self.worker_id = worker_id
        self.file_queue = file_queue
        self.client = OpenAI(api_key=api_key)
        self.results_queue = results_queue
        self.lock = lock
        self.files_processed = 0

    def calculate_md5(self, file_path):
        """Calculate MD5 hash."""
        md5_hash = hashlib.md5()
        with open(file_path, 'rb') as f:
            for chunk in iter(lambda: f.read(4096), b""):
                md5_hash.update(chunk)
        return md5_hash.hexdigest()

    def add_language_notice(self, content, file_path, is_root=False):
        """Add language notice."""
        relative_url = file_path.replace('\\', '/')
        english_url = f"https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/{relative_url}"
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
            notice = f"""---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [{relative_url}]({english_url})
🇯🇵 **日本語:** [{relative_url}]({japanese_org_url})

---

"""
        return notice + content

    def translate_text(self, text, file_path, is_csharp=False):
        """Translate text using ChatGPT API."""
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
            response = self.client.chat.completions.create(
                model="gpt-4-turbo-preview",
                messages=[
                    {"role": "system", "content": "You are a professional technical translator specializing in software documentation. You translate English to Japanese while preserving all formatting and technical accuracy."},
                    {"role": "user", "content": prompt}
                ],
                temperature=0.3,
                max_tokens=4000
            )
            return response.choices[0].message.content.strip()
        except Exception as e:
            print(f"[Worker {self.worker_id}] ERROR translating {file_path}: {e}")
            raise

    def process_file(self, source_file, dest_file, relative_path, tracking_data):
        """Process a single file."""
        source_md5 = self.calculate_md5(source_file)
        source_mtime = source_file.stat().st_mtime

        # Check if translation needed
        file_info = tracking_data["files"].get(relative_path, {})
        if dest_file.exists() and file_info.get("source_md5") == source_md5:
            return False  # Skip unchanged

        print(f"[Worker {self.worker_id}] Translating: {relative_path}")

        dest_file.parent.mkdir(parents=True, exist_ok=True)

        # Handle translatable files
        if source_file.suffix.lower() in TRANSLATABLE_EXTENSIONS:
            with open(source_file, 'r', encoding='utf-8') as f:
                content = f.read()

            is_csharp = source_file.suffix.lower() == '.cs'
            translated = self.translate_text(content, relative_path, is_csharp=is_csharp)

            # Add language notice to markdown files
            if not is_csharp and source_file.suffix.lower() == '.md':
                is_root = (relative_path == "README.md")
                translated = self.add_language_notice(translated, relative_path, is_root=is_root)

            with open(dest_file, 'w', encoding='utf-8') as f:
                f.write(translated)

            return {
                "path": relative_path,
                "md5": source_md5,
                "mtime": source_mtime,
                "type": "translated"
            }
        else:
            # Copy non-translatable files
            shutil.copy2(source_file, dest_file)
            return {
                "path": relative_path,
                "md5": source_md5,
                "mtime": source_mtime,
                "type": "copied"
            }

    def run(self):
        """Worker main loop."""
        print(f"[Worker {self.worker_id}] Started")

        while True:
            try:
                # Get file from queue (timeout after 1 second)
                item = self.file_queue.get(timeout=1)
                if item is None:  # Poison pill to stop worker
                    break

                source_file, dest_file, relative_path = item

                # Load tracking data (thread-safe)
                with self.lock:
                    if TRACKING_FILE.exists():
                        with open(TRACKING_FILE, 'r', encoding='utf-8') as f:
                            tracking_data = json.load(f)
                    else:
                        tracking_data = {"files": {}}

                # Process file
                result = self.process_file(source_file, dest_file, relative_path, tracking_data)

                if result:
                    # Update tracking (thread-safe)
                    with self.lock:
                        if TRACKING_FILE.exists():
                            with open(TRACKING_FILE, 'r', encoding='utf-8') as f:
                                tracking_data = json.load(f)
                        else:
                            tracking_data = {"files": {}}

                        tracking_data["files"][result["path"]] = {
                            "source_md5": result["md5"],
                            "source_mtime": result["mtime"],
                            f"{result['type']}_at": datetime.utcnow().isoformat() + "Z"
                        }

                        TRACKING_FILE.parent.mkdir(parents=True, exist_ok=True)
                        with open(TRACKING_FILE, 'w', encoding='utf-8') as f:
                            json.dump(tracking_data, f, indent=2, ensure_ascii=False)

                    self.files_processed += 1
                    self.results_queue.put(result)

                self.file_queue.task_done()

            except queue.Empty:
                continue
            except Exception as e:
                print(f"[Worker {self.worker_id}] ERROR: {e}")
                self.file_queue.task_done()
                continue

        print(f"[Worker {self.worker_id}] Finished - Processed {self.files_processed} files")


def should_skip(path):
    """Check if path should be skipped."""
    return any(pattern in str(path) or path.name == pattern for pattern in SKIP_PATTERNS)


def main():
    """Main parallel translation orchestrator."""
    print("="*80)
    print(f"Parallel Translation Engine - {NUM_WORKERS} Workers")
    print("="*80)
    print(f"Source: {SOURCE_DIR}")
    print(f"Destination: {DEST_DIR}")
    print("="*80)

    # Load API key
    api_key = API_KEY_FILE.read_text().strip()

    # Create work queue
    file_queue = queue.Queue()
    results_queue = queue.Queue()
    lock = threading.Lock()

    # Discover all files to translate
    print("\n📂 Discovering files...")
    file_count = 0

    for root, dirs, files in os.walk(SOURCE_DIR):
        root_path = Path(root)
        dirs[:] = [d for d in dirs if not should_skip(root_path / d)]

        for file_name in files:
            source_file = root_path / file_name
            if should_skip(source_file):
                continue

            relative_path = str(source_file.relative_to(SOURCE_DIR))
            dest_file = DEST_DIR / relative_path

            file_queue.put((source_file, dest_file, relative_path))
            file_count += 1

    print(f"✅ Found {file_count} files to process")

    # Start workers
    print(f"\n🚀 Starting {NUM_WORKERS} translation workers...")
    workers = []
    for i in range(NUM_WORKERS):
        worker = TranslationWorker(i+1, file_queue, api_key, results_queue, lock)
        worker.start()
        workers.append(worker)

    # Monitor progress
    start_time = time.time()
    last_report = 0

    while any(w.is_alive() for w in workers) or not file_queue.empty():
        time.sleep(5)

        # Report every minute
        elapsed = time.time() - start_time
        if elapsed - last_report >= 60:
            completed = sum(w.files_processed for w in workers)
            rate = completed / (elapsed / 60) if elapsed > 0 else 0
            remaining = file_count - completed
            eta_minutes = remaining / rate if rate > 0 else 0

            print(f"\n⏱️  Progress: {completed}/{file_count} files ({completed*100//file_count}%) - "
                  f"Rate: {rate:.1f} files/min - ETA: {eta_minutes:.1f} min")

            last_report = elapsed

    # Stop workers
    for _ in range(NUM_WORKERS):
        file_queue.put(None)

    # Wait for completion
    for worker in workers:
        worker.join()

    elapsed = time.time() - start_time
    total_processed = sum(w.files_processed for w in workers)

    print("\n" + "="*80)
    print("🎉 Translation Complete!")
    print(f"   Processed: {total_processed} files")
    print(f"   Time: {elapsed/60:.1f} minutes")
    print(f"   Rate: {total_processed/(elapsed/60):.1f} files/minute")
    print("="*80)


if __name__ == "__main__":
    main()
