# Japanese Translation Guide

## Quick Start

```bash
# Check status
./translate.sh status

# Start translation (if stopped)
./translate.sh start

# Watch live progress
./translate.sh watch

# Stop translation
./translate.sh stop
```

## Current Status

- **Source**: `/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025` (English)
- **Destination**: Current directory (Japanese)
- **Workers**: 4 parallel threads
- **Progress tracking**: `translation_tracking.json`
- **Total files**: ~772 files (.md and .cs)

## What's Running

The `parallel_translator.py` script uses OpenAI GPT-4 to translate:
- **Markdown files** (.md): Full content translation with formatting preserved
- **C# files** (.cs): Only translates comments, code stays in English

## Files

- `translate.sh` - Main control script (use this!)
- `parallel_translator.py` - Translation engine
- `translation_tracking.json` - Progress tracking
- `translation_output.log` - Detailed logs

## Troubleshooting

If translation seems stuck:

```bash
# Check if running
ps aux | grep parallel_translator

# View logs
tail -f translation_output.log

# Restart
./translate.sh stop
./translate.sh start
```

## Progress Monitoring

The script automatically:
- Skips already-translated files (checks MD5 hash)
- Tracks progress in JSON
- Handles failures gracefully
- Can be stopped and resumed anytime

## Completion

When done (772/772 files), you can:
1. Review the translations
2. Commit to git
3. Push to GitHub repository
