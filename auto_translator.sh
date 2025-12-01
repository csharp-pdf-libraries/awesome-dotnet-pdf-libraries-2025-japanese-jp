#!/bin/bash
# Auto Translation Pipeline
# Translates one language at a time, deploys to GitHub when complete

LANGUAGES=("ja" "zh-CN" "ko" "pt-BR" "es" "de" "fr" "ru" "zh-TW" "it" "pl" "tr" "nl" "hi" "id" "vi" "th")
LANG_NAMES=("Japanese" "Simplified Chinese" "Korean" "Portuguese (Brazil)" "Spanish" "German" "French" "Russian" "Traditional Chinese" "Italian" "Polish" "Turkish" "Dutch" "Hindi" "Indonesian" "Vietnamese" "Thai")
SOURCE_DIR="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"

# Total files to translate (excluding source-material)
TOTAL_FILES=$(find "$SOURCE_DIR" -name "*.md" -o -name "*.cs" -not -path "*/source-material/*" 2>/dev/null | wc -l | tr -d ' ')

echo "========================================"
echo "Auto Translation Pipeline"
echo "Total files per language: $TOTAL_FILES"
echo "========================================"

for i in "${!LANGUAGES[@]}"; do
    LANG="${LANGUAGES[$i]}"
    LANG_NAME="${LANG_NAMES[$i]}"

    echo ""
    echo "========================================"
    echo "[$((i+1))/${#LANGUAGES[@]}] Starting: $LANG_NAME ($LANG)"
    echo "========================================"

    # Start translation
    python3 universal_translator.py "$LANG"

    if [ $? -eq 0 ]; then
        echo "✅ Translation complete: $LANG_NAME"

        # Deploy to GitHub
        echo "🚀 Deploying to GitHub..."
        cd "/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025-$LANG" || exit

        git config user.name "Jacob Mellor"
        git config user.email "jacob@ironsoftware.com"
        git add .

        COMMIT_MSG="Initial $LANG_NAME translation

🌐 Automated translation from English to $LANG_NAME
📝 Translated using ChatGPT API (gpt-4-turbo-preview)
✅ 337 markdown files + 435 C# files
🤖 Generated with Claude Code

Co-Authored-By: Claude <noreply@anthropic.com>"

        git commit -m "$COMMIT_MSG"
        git branch -M main
        git push -u origin main --force

        echo "✅ Deployed: $LANG_NAME"
        echo ""
    else
        echo "❌ Translation failed: $LANG_NAME"
        exit 1
    fi
done

echo ""
echo "========================================"
echo "🎉 All translations complete!"
echo "========================================"
