#!/usr/bin/env python3
"""Quick API test to verify Anthropic and OpenAI are working."""

import os
from pathlib import Path
import anthropic
import openai
import time

# Load API keys
BASE_DIR = Path(".")
ANTHROPIC_KEY = (BASE_DIR / "AnthropicAPIkey.txt").read_text().strip()
OPENAI_KEY = (BASE_DIR / "OPENAIKEY.txt").read_text().strip()

print("Testing APIs...")
print(f"Anthropic key: {ANTHROPIC_KEY[:15]}...")
print(f"OpenAI key: {OPENAI_KEY[:15]}...")

# Test Anthropic with timeout
print("\n1. Testing Anthropic API...")
try:
    client = anthropic.Anthropic(api_key=ANTHROPIC_KEY, timeout=30.0)
    start = time.time()
    response = client.messages.create(
        model="claude-sonnet-4-5-20250929",
        max_tokens=100,
        messages=[{"role": "user", "content": "Say 'Hello, test successful!' and nothing else."}]
    )
    elapsed = time.time() - start
    result = response.content[0].text
    print(f"✓ Anthropic works! ({elapsed:.2f}s)")
    print(f"  Response: {result}")
except Exception as e:
    print(f"✗ Anthropic failed: {e}")

# Test OpenAI with timeout
print("\n2. Testing OpenAI API...")
try:
    client = openai.OpenAI(api_key=OPENAI_KEY, timeout=30.0)
    start = time.time()
    response = client.chat.completions.create(
        model="gpt-4o-mini",
        max_tokens=100,
        messages=[{"role": "user", "content": "Say 'Hello, test successful!' and nothing else."}]
    )
    elapsed = time.time() - start
    result = response.choices[0].message.content
    print(f"✓ OpenAI works! ({elapsed:.2f}s)")
    print(f"  Response: {result}")
except Exception as e:
    print(f"✗ OpenAI failed: {e}")

print("\nAPI test complete!")
