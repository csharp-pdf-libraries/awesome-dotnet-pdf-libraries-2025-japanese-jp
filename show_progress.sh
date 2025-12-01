#!/bin/bash
SOURCE="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"
DEST="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-japanese-jp"

TOTAL_MD=$(find "$SOURCE" -name "*.md" -type f 2>/dev/null | wc -l | tr -d ' ')
MD_PROCESSED=$(grep "\.md" "$DEST/translation_tracking.json" 2>/dev/null | wc -l | tr -d ' ')

echo "✓ $MD_PROCESSED / $TOTAL_MD markdown files translated"
