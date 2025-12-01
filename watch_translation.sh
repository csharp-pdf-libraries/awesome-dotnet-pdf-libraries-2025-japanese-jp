#!/bin/bash
# Translation Progress Monitor

TOTAL=772
START_TIME=$(date +%s)
LAST_COUNT=0

while ps -p 3721 > /dev/null 2>&1; do
    clear
    echo "================================"
    echo "  TRANSLATION PROGRESS MONITOR"
    echo "================================"
    echo ""

    # Get current count from JSON
    COMPLETED=$(cat translation_tracking.json 2>/dev/null | python3 -c "import sys, json; d=json.load(sys.stdin); print(len(d.get('files', {})))" 2>/dev/null || echo "0")

    # Calculate progress
    PERCENT=$((COMPLETED * 100 / TOTAL))
    REMAINING=$((TOTAL - COMPLETED))

    # Calculate rate and ETA
    ELAPSED=$(($(date +%s) - START_TIME))
    if [ $ELAPSED -gt 0 ]; then
        RATE=$(echo "scale=2; ($COMPLETED - $LAST_COUNT) / ($ELAPSED / 60)" | bc 2>/dev/null || echo "0")
        if [ "$(echo "$RATE > 0" | bc)" -eq 1 ]; then
            ETA=$(echo "scale=1; $REMAINING / $RATE" | bc 2>/dev/null || echo "N/A")
        else
            ETA="Calculating..."
        fi
    else
        RATE="0"
        ETA="Calculating..."
    fi

    # Display status
    echo "📊 Progress: $COMPLETED / $TOTAL files ($PERCENT%)"
    echo "⏳ Remaining: $REMAINING files"
    echo "⚡ Rate: $RATE files/min"
    echo "🕒 ETA: $ETA minutes"
    echo ""
    echo "Process PID: 3721"
    echo "Updated: $(date '+%Y-%m-%d %H:%M:%S')"
    echo ""
    echo "Press Ctrl+C to stop monitoring (translation will continue)"
    echo "================================"

    # Wait 2 minutes
    sleep 120
done

echo ""
echo "✅ Translation process has completed or stopped!"
