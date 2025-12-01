#!/bin/bash
################################################################################
# JAPANESE TRANSLATION CONTROL SCRIPT
# Run this script to manage the English → Japanese translation
################################################################################

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Configuration
SOURCE_DIR="/Users/jacob/Sites/awesome-dotnet-pdf-libraries-2025"
DEST_DIR="$SCRIPT_DIR"
TRACKING_FILE="$DEST_DIR/translation_tracking.json"
PID_FILE="$DEST_DIR/translation.pid"
LOG_FILE="$DEST_DIR/translation_output.log"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

################################################################################
# Functions
################################################################################

print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}  JAPANESE TRANSLATION MANAGER${NC}"
    echo -e "${BLUE}================================${NC}"
    echo ""
}

get_status() {
    # Check if process is running
    if [ -f "$PID_FILE" ]; then
        PID=$(cat "$PID_FILE")
        if ps -p $PID > /dev/null 2>&1; then
            echo "running"
            return
        fi
    fi

    # Check if completed
    if [ -f "$TRACKING_FILE" ]; then
        COMPLETED=$(cat "$TRACKING_FILE" | python3 -c "import sys, json; d=json.load(sys.stdin); print(len(d.get('files', {})))" 2>/dev/null || echo "0")
        TOTAL=$(find "$SOURCE_DIR" -type f \( -name "*.md" -o -name "*.cs" \) ! -path "*/source-material/*" ! -path "*/.git/*" ! -path "*/node_modules/*" 2>/dev/null | wc -l)

        if [ "$COMPLETED" -ge "$TOTAL" ] && [ "$TOTAL" -gt "0" ]; then
            echo "completed"
            return
        fi
    fi

    echo "stopped"
}

show_progress() {
    if [ ! -f "$TRACKING_FILE" ]; then
        echo -e "${YELLOW}No progress data available${NC}"
        return
    fi

    COMPLETED=$(cat "$TRACKING_FILE" | python3 -c "import sys, json; d=json.load(sys.stdin); print(len(d.get('files', {})))" 2>/dev/null || echo "0")
    TOTAL=$(find "$SOURCE_DIR" -type f \( -name "*.md" -o -name "*.cs" \) ! -path "*/source-material/*" ! -path "*/.git/*" ! -path "*/node_modules/*" 2>/dev/null | wc -l)

    PERCENT=$((COMPLETED * 100 / TOTAL))
    REMAINING=$((TOTAL - COMPLETED))

    echo -e "${GREEN}📊 Progress: $COMPLETED / $TOTAL files ($PERCENT%)${NC}"
    echo -e "${GREEN}⏳ Remaining: $REMAINING files${NC}"
}

start_translation() {
    STATUS=$(get_status)

    if [ "$STATUS" == "running" ]; then
        echo -e "${YELLOW}⚠️  Translation is already running!${NC}"
        PID=$(cat "$PID_FILE")
        echo -e "   PID: $PID"
        show_progress
        return
    fi

    if [ "$STATUS" == "completed" ]; then
        echo -e "${GREEN}✅ Translation already completed!${NC}"
        show_progress
        echo ""
        read -p "Restart translation? (y/N): " RESTART
        if [ "$RESTART" != "y" ] && [ "$RESTART" != "Y" ]; then
            return
        fi
        rm -f "$TRACKING_FILE"
    fi

    echo -e "${BLUE}🚀 Starting translation with 4 parallel workers...${NC}"
    echo ""

    python3 parallel_translator.py > "$LOG_FILE" 2>&1 &
    PID=$!
    echo $PID > "$PID_FILE"

    sleep 2

    if ps -p $PID > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Translation started successfully!${NC}"
        echo -e "   PID: $PID"
        echo -e "   Log: $LOG_FILE"
        echo ""
        echo -e "Run ${YELLOW}./translate.sh status${NC} to check progress"
    else
        echo -e "${RED}❌ Failed to start translation${NC}"
        rm -f "$PID_FILE"
        return 1
    fi
}

stop_translation() {
    if [ ! -f "$PID_FILE" ]; then
        echo -e "${YELLOW}No translation process found${NC}"
        return
    fi

    PID=$(cat "$PID_FILE")

    if ! ps -p $PID > /dev/null 2>&1; then
        echo -e "${YELLOW}Translation process not running${NC}"
        rm -f "$PID_FILE"
        return
    fi

    echo -e "${YELLOW}Stopping translation (PID: $PID)...${NC}"
    kill $PID 2>/dev/null || kill -9 $PID 2>/dev/null
    rm -f "$PID_FILE"
    echo -e "${GREEN}✅ Translation stopped${NC}"
}

show_status() {
    STATUS=$(get_status)

    case $STATUS in
        running)
            echo -e "${GREEN}Status: RUNNING ✅${NC}"
            PID=$(cat "$PID_FILE")
            echo -e "PID: $PID"
            echo ""
            show_progress
            echo ""
            echo -e "Tail log: ${YELLOW}tail -f $LOG_FILE${NC}"
            ;;
        completed)
            echo -e "${GREEN}Status: COMPLETED ✅${NC}"
            echo ""
            show_progress
            ;;
        stopped)
            echo -e "${YELLOW}Status: STOPPED${NC}"
            echo ""
            show_progress
            echo ""
            echo -e "Run ${YELLOW}./translate.sh start${NC} to continue"
            ;;
    esac
}

watch_progress() {
    echo -e "${BLUE}Starting live progress monitor...${NC}"
    echo -e "${YELLOW}Press Ctrl+C to stop monitoring (translation will continue)${NC}"
    echo ""
    sleep 2

    while true; do
        clear
        print_header

        STATUS=$(get_status)
        if [ "$STATUS" != "running" ]; then
            echo -e "${YELLOW}Translation is not running${NC}"
            break
        fi

        show_status
        sleep 30  # Update every 30 seconds
    done
}

show_help() {
    print_header
    echo "Usage: ./translate.sh [command]"
    echo ""
    echo "Commands:"
    echo "  start    - Start the translation process"
    echo "  stop     - Stop the translation process"
    echo "  status   - Show current status and progress"
    echo "  watch    - Watch live progress (updates every 30s)"
    echo "  help     - Show this help message"
    echo ""
    echo "Files:"
    echo "  Source:   $SOURCE_DIR"
    echo "  Dest:     $DEST_DIR"
    echo "  Tracking: $TRACKING_FILE"
    echo "  Log:      $LOG_FILE"
    echo ""
}

################################################################################
# Main
################################################################################

print_header

case "${1:-status}" in
    start)
        start_translation
        ;;
    stop)
        stop_translation
        ;;
    status)
        show_status
        ;;
    watch)
        watch_progress
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        echo -e "${RED}Unknown command: $1${NC}"
        echo ""
        show_help
        exit 1
        ;;
esac
