#!/bin/bash
TOTAL=772
START_COUNT=$(cat translation_tracking.json 2>/dev/null | python3 -c "import sys, json; d=json.load(sys.stdin); print(len(d.get('files', {})))" 2>/dev/null || echo "323")
START_TIME=$(date +%s)

while true; do
    sleep 120  # 2 minutes
    
    # Get current progress
    COMPLETED=$(cat translation_tracking.json 2>/dev/null | python3 -c "import sys, json; d=json.load(sys.stdin); print(len(d.get('files', {})))" 2>/dev/null || echo "0")
    PERCENT=$((COMPLETED * 100 / TOTAL))
    REMAINING=$((TOTAL - COMPLETED))
    
    # Calculate rate and ETA
    ELAPSED=$(($(date +%s) - START_TIME))
    TRANSLATED=$((COMPLETED - START_COUNT))
    
    if [ $ELAPSED -gt 0 ] && [ $TRANSLATED -gt 0 ]; then
        RATE=$(echo "scale=2; $TRANSLATED * 60 / $ELAPSED" | bc)
        ETA_MINUTES=$(echo "scale=0; $REMAINING / ($RATE / 60)" | bc 2>/dev/null || echo "0")
        ETA_HOURS=$((ETA_MINUTES / 60))
        ETA_MINS=$((ETA_MINUTES % 60))
        ETA_STR="${ETA_HOURS}h ${ETA_MINS}m"
    else
        RATE="calculating"
        ETA_STR="calculating"
    fi
    
    # Check if still running
    if ! ps -p 3721 > /dev/null 2>&1; then
        echo "$(date '+%H:%M') - COMPLETED/STOPPED | $COMPLETED/$TOTAL ($PERCENT%)" >> translation_monitor.log
        break
    fi
    
    echo "$(date '+%H:%M') - $COMPLETED/$TOTAL ($PERCENT%) | Remaining: $REMAINING | ETA: $ETA_STR" >> translation_monitor.log
done
