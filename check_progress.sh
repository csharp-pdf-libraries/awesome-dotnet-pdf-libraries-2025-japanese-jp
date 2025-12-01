#!/bin/bash
# Check translation progress

SOURCE="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"
DEST="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp"

echo "========================================"
echo "Translation Progress Monitor"
echo "========================================"
echo ""

# Count total files
TOTAL_MD=$(find "$SOURCE" -name "*.md" -type f 2>/dev/null | wc -l | tr -d ' ')
TOTAL_CS=$(find "$SOURCE" -name "*.cs" -type f 2>/dev/null | wc -l | tr -d ' ')

echo "Source repository:"
echo "  - Markdown files: $TOTAL_MD"
echo "  - C# files: $TOTAL_CS"
echo ""

# Count processed files from tracking JSON
if [ -f "$DEST/translation_tracking.json" ]; then
    PROCESSED=$(grep -c "source_md5" "$DEST/translation_tracking.json")
    MD_PROCESSED=$(grep "\.md" "$DEST/translation_tracking.json" | wc -l | tr -d ' ')
    CS_PROCESSED=$(grep "\.cs" "$DEST/translation_tracking.json" | wc -l | tr -d ' ')

    echo "Files processed:"
    echo "  - Total files: $PROCESSED"
    echo "  - Markdown files: $MD_PROCESSED / $TOTAL_MD"
    echo "  - C# files: $CS_PROCESSED / $TOTAL_CS"
    echo ""

    # Calculate percentage
    if [ "$TOTAL_MD" -gt 0 ]; then
        PERCENT=$((MD_PROCESSED * 100 / TOTAL_MD))
        echo "Progress: $PERCENT% of markdown files"
    fi
else
    echo "No tracking file found yet."
fi

echo ""
echo "========================================"

# Check if script is still running
if pgrep -f "translate_to_japanese.py" > /dev/null; then
    echo "Status: Translation script is RUNNING"
else
    echo "Status: Translation script is NOT running"
fi

echo "========================================"
