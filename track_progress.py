#!/usr/bin/env python3
"""Track translation progress with ETA and cost estimates"""
import json
import time
from datetime import datetime, timedelta
from pathlib import Path

TRACKING_FILE = Path("translation_tracking.json")
PROGRESS_LOG = Path("progress_history.json")
TOTAL_FILES = 772
COST_PER_FILE = 0.10  # GPT-4-turbo estimate

def load_tracking():
    if TRACKING_FILE.exists():
        with open(TRACKING_FILE) as f:
            return json.load(f)
    return {"files": {}}

def load_history():
    if PROGRESS_LOG.exists():
        with open(PROGRESS_LOG) as f:
            return json.load(f)
    return []

def save_history(history):
    with open(PROGRESS_LOG, 'w') as f:
        json.dump(history, f, indent=2)

def calculate_eta(history):
    if len(history) < 2:
        return None

    # Get last two data points
    recent = history[-2:]
    time_diff = recent[1]['timestamp'] - recent[0]['timestamp']
    files_diff = recent[1]['completed'] - recent[0]['completed']

    if time_diff <= 0 or files_diff <= 0:
        return None

    # Calculate rate (files per second)
    rate = files_diff / time_diff

    # Calculate remaining time
    remaining_files = TOTAL_FILES - recent[1]['completed']
    eta_seconds = remaining_files / rate

    return eta_seconds

def format_eta(seconds):
    if seconds is None:
        return "Calculating..."

    hours = int(seconds // 3600)
    minutes = int((seconds % 3600) // 60)

    if hours > 0:
        return f"{hours}h {minutes}m"
    else:
        return f"{minutes}m"

def get_status():
    tracking = load_tracking()
    history = load_history()

    completed = len(tracking.get('files', {}))
    remaining = TOTAL_FILES - completed
    percent = int(completed * 100 / TOTAL_FILES)

    # Add current snapshot to history
    current = {
        'timestamp': time.time(),
        'completed': completed,
        'datetime': datetime.now().isoformat()
    }
    history.append(current)

    # Keep last 10 snapshots
    history = history[-10:]
    save_history(history)

    # Calculate ETA
    eta_seconds = calculate_eta(history)
    eta_str = format_eta(eta_seconds)

    # Calculate costs
    cost_so_far = completed * COST_PER_FILE
    remaining_cost = remaining * COST_PER_FILE
    total_cost = cost_so_far + remaining_cost

    # Calculate rate
    if len(history) >= 2:
        time_diff = history[-1]['timestamp'] - history[-2]['timestamp']
        files_diff = history[-1]['completed'] - history[-2]['completed']
        rate = files_diff / (time_diff / 60) if time_diff > 0 else 0
    else:
        rate = 0

    return {
        'completed': completed,
        'total': TOTAL_FILES,
        'percent': percent,
        'remaining': remaining,
        'cost_so_far': cost_so_far,
        'remaining_cost': remaining_cost,
        'total_cost': total_cost,
        'eta': eta_str,
        'rate': rate,
        'timestamp': datetime.now().strftime('%H:%M')
    }

if __name__ == "__main__":
    status = get_status()

    print(f"📊 Progress: {status['completed']} / {status['total']} files ({status['percent']}%)")
    print(f"⏳ Remaining: {status['remaining']} files")
    print(f"⚡ Rate: {status['rate']:.1f} files/min")
    print(f"🕒 ETA: {status['eta']}")
    print(f"💰 Cost so far: ${status['cost_so_far']:.2f}")
    print(f"💰 Remaining: ${status['remaining_cost']:.2f}")
    print(f"💰 Total estimate: ${status['total_cost']:.2f}")
