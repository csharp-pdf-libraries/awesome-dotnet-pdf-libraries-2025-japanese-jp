#!/usr/bin/env python3
"""
Universal Translation Script
Translates the awesome-dotnet-pdf-libraries-2025 repository to any language
"""

import os
import json
import hashlib
import shutil
import sys
from pathlib import Path
from datetime import datetime
from openai import OpenAI

# Language display names and their native forms
LANGUAGE_NAMES = {
    'ja': {'english': 'Japanese', 'native': '日本語', 'flag': '🇯🇵'},
    'zh-CN': {'english': 'Simplified Chinese', 'native': '简体中文', 'flag': '🇨🇳'},
    'zh-TW': {'english': 'Traditional Chinese', 'native': '繁體中文', 'flag': '🇹🇼'},
    'ko': {'english': 'Korean', 'native': '한국어', 'flag': '🇰🇷'},
    'pt-BR': {'english': 'Portuguese (Brazil)', 'native': 'Português (Brasil)', 'flag': '🇧🇷'},
    'es': {'english': 'Spanish', 'native': 'Español', 'flag': '🇪🇸'},
    'de': {'english': 'German', 'native': 'Deutsch', 'flag': '🇩🇪'},
    'fr': {'english': 'French', 'native': 'Français', 'flag': '🇫🇷'},
    'ru': {'english': 'Russian', 'native': 'Русский', 'flag': '🇷🇺'},
    'it': {'english': 'Italian', 'native': 'Italiano', 'flag': '🇮🇹'},
    'pl': {'english': 'Polish', 'native': 'Polski', 'flag': '🇵🇱'},
    'tr': {'english': 'Turkish', 'native': 'Türkçe', 'flag': '🇹🇷'},
    'nl': {'english': 'Dutch', 'native': 'Nederlands', 'flag': '🇳🇱'},
    'hi': {'english': 'Hindi', 'native': 'हिन्दी', 'flag': '🇮🇳'},
    'id': {'english': 'Indonesian', 'native': 'Bahasa Indonesia', 'flag': '🇮🇩'},
    'vi': {'english': 'Vietnamese', 'native': 'Tiếng Việt', 'flag': '🇻🇳'},
    'th': {'english': 'Thai', 'native': 'ไทย', 'flag': '🇹🇭'},
}


class UniversalTranslator:
    def __init__(self, target_lang, source_dir, dest_dir, api_key_file):
        self.target_lang = target_lang
        self.lang_info = LANGUAGE_NAMES.get(target_lang, {'english': target_lang, 'native': target_lang, 'flag': '🌐'})
        self.source_dir = Path(source_dir)
        self.dest_dir = Path(dest_dir)
        self.tracking_file = dest_dir / "translation_tracking.json"
        self.client = OpenAI(api_key=Path(api_key_file).read_text().strip())
        self.translatable_extensions = {'.md', '.cs'}
        self.skip_patterns = {'.git', '__pycache__', 'node_modules', '.DS_Store',
                              'translation_tracking.json', '.gitignore',
                              'source-material', 'source material'}

    def calculate_md5(self, file_path):
        """Calculate MD5 hash of a file."""
        md5_hash = hashlib.md5()
        with open(file_path, 'rb') as f:
            for chunk in iter(lambda: f.read(4096), b""):
                md5_hash.update(chunk)
        return md5_hash.hexdigest()

    def load_tracking_data(self):
        """Load translation tracking data."""
        if self.tracking_file.exists():
            with open(self.tracking_file, 'r', encoding='utf-8') as f:
                return json.load(f)
        return {"files": {}}

    def save_tracking_data(self, data):
        """Save translation tracking data."""
        self.tracking_file.parent.mkdir(parents=True, exist_ok=True)
        with open(self.tracking_file, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, ensure_ascii=False)

    def add_language_notice(self, content, file_path, is_root=False):
        """Add language notice to translated files."""
        relative_url = file_path.replace('\\', '/')
        flag = self.lang_info['flag']
        native_name = self.lang_info['native']

        if is_root or file_path == "README.md":
            notice = f"""---
**{flag} {native_name} Version | {self.lang_info['english']} Translation**

この文書は英語版から翻訳されたものです。最新の情報については、[英語版をご覧ください](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025)。

*This document has been translated from English to {self.lang_info['english']}. For the latest information, please refer to the [English version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025).*

---

"""
        else:
            notice = f"""---
**{flag} {native_name}** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/{relative_url})

---

"""
        return notice + content

    def translate_text(self, text, file_path, is_csharp=False):
        """Translate text using ChatGPT API."""
        print(f"  Translating to {self.lang_info['english']}: {file_path}...")

        if is_csharp:
            prompt = f"""Translate ONLY the comments in this C# code from English to {self.lang_info['english']}.

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
            prompt = f"""Translate the following Markdown content from English to {self.lang_info['english']}.

IMPORTANT RULES:
1. Preserve ALL Markdown formatting (headers, links, code blocks, tables, etc.)
2. Keep code blocks in ENGLISH (do not translate code)
3. Keep URLs unchanged
4. Keep product/brand names unchanged (IronPDF, Aspose, Microsoft, etc.)
5. Keep file paths and technical terms in English
6. Translate only the prose and explanations
7. Maintain the same structure and formatting

Content to translate:

{text}
"""

        try:
            response = self.client.chat.completions.create(
                model="gpt-4-turbo-preview",
                messages=[
                    {"role": "system", "content": f"You are a professional technical translator specializing in software documentation. You translate English to {self.lang_info['english']} while preserving all formatting and technical accuracy."},
                    {"role": "user", "content": prompt}
                ],
                temperature=0.3,
                max_tokens=4000
            )

            return response.choices[0].message.content.strip()

        except Exception as e:
            print(f"  ERROR: {e}")
            raise

    def should_skip(self, path):
        """Check if path should be skipped."""
        return any(pattern in str(path) or path.name == pattern for pattern in self.skip_patterns)

    def process_file(self, source_file, dest_file, relative_path, tracking_data):
        """Process a single file."""
        source_md5 = self.calculate_md5(source_file)
        source_mtime = source_file.stat().st_mtime

        # Check if translation needed
        file_info = tracking_data["files"].get(relative_path, {})
        if dest_file.exists() and file_info.get("source_md5") == source_md5:
            print(f"SKIP (unchanged): {relative_path}")
            return False

        print(f"{'NEW' if not dest_file.exists() else 'UPDATED'}: {relative_path}")
        dest_file.parent.mkdir(parents=True, exist_ok=True)

        # Handle translatable files
        if source_file.suffix.lower() in self.translatable_extensions:
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

            tracking_data["files"][relative_path] = {
                "source_md5": source_md5,
                "source_mtime": source_mtime,
                "translated_at": datetime.utcnow().isoformat() + "Z"
            }
            print(f"  ✓ Translated")
            return True
        else:
            # Copy non-translatable files
            shutil.copy2(source_file, dest_file)
            tracking_data["files"][relative_path] = {
                "source_md5": source_md5,
                "source_mtime": source_mtime,
                "copied_at": datetime.utcnow().isoformat() + "Z"
            }
            print(f"  ✓ Copied")
            return True

    def run(self):
        """Run the translation process."""
        print("=" * 80)
        print(f"Universal Translator - {self.lang_info['flag']} {self.lang_info['english']}")
        print("=" * 80)
        print(f"Source: {self.source_dir}")
        print(f"Destination: {self.dest_dir}")
        print("=" * 80)

        tracking_data = self.load_tracking_data()
        self.dest_dir.mkdir(parents=True, exist_ok=True)

        files_processed = 0
        files_skipped = 0

        for root, dirs, files in os.walk(self.source_dir):
            root_path = Path(root)
            dirs[:] = [d for d in dirs if not self.should_skip(root_path / d)]

            for file_name in files:
                source_file = root_path / file_name
                if self.should_skip(source_file):
                    continue

                relative_path = str(source_file.relative_to(self.source_dir))
                dest_file = self.dest_dir / relative_path

                try:
                    if self.process_file(source_file, dest_file, relative_path, tracking_data):
                        files_processed += 1
                        self.save_tracking_data(tracking_data)
                    else:
                        files_skipped += 1
                except Exception as e:
                    print(f"  ✗ ERROR: {e}")
                    continue

        print("=" * 80)
        print(f"Translation complete!")
        print(f"Processed: {files_processed} | Skipped: {files_skipped}")
        print("=" * 80)


def main():
    if len(sys.argv) < 2:
        print("Usage: python universal_translator.py <lang_code> [source_dir] [dest_dir]")
        print("\nAvailable languages:")
        for code, info in LANGUAGE_NAMES.items():
            print(f"  {code:8} - {info['flag']} {info['english']} ({info['native']})")
        sys.exit(1)

    target_lang = sys.argv[1]
    source_dir = sys.argv[2] if len(sys.argv) > 2 else "/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"
    dest_dir = sys.argv[3] if len(sys.argv) > 3 else f"/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-{target_lang}"
    api_key_file = "/Users/jacob/Sites/SEO-MACHINE/OPENAIKEY.txt"

    translator = UniversalTranslator(target_lang, source_dir, dest_dir, api_key_file)
    translator.run()


if __name__ == "__main__":
    main()
