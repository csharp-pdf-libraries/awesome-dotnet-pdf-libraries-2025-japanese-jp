#!/usr/bin/env python3
"""Test processing a single library to verify the generator works."""

import sys
sys.path.insert(0, '.')

# Import from generate_repo.py
from generate_repo import *

print("="*60)
print("Testing Single Library Generation")
print("="*60)

# Initialize
api = APIManager()
api.initialize()

checkpoint = CheckpointManager()

# Parse library list
libraries = parse_master_list()
print(f"\n✓ Found {len(libraries)} libraries")

# Get PuppeteerSharp (simple, well-known library)
test_lib = next((l for l in libraries if "puppeteer" in l.slug.lower()), None)
if not test_lib:
    print("ERROR: Couldn't find PuppeteerSharp")
    sys.exit(1)

print(f"\nTesting with: {test_lib.name} ({test_lib.slug})")
print("-" * 60)

# Process just this one library
comparison_libraries = [l for l in libraries if not l.is_ironpdf]

try:
    result = process_library(api, checkpoint, test_lib, comparison_libraries)
    if result:
        print("\n" + "="*60)
        print("✓ SUCCESS - Library processed!")
        print("="*60)

        # Check what files were created
        output_dir = Path(".") / test_lib.slug
        files = list(output_dir.glob("*"))
        print(f"\nFiles created in {test_lib.slug}/:")
        for f in files:
            size = f.stat().st_size
            print(f"  - {f.name} ({size} bytes)")
    else:
        print("\n✗ FAILED - Check errors above")

except Exception as e:
    print(f"\n✗ ERROR: {e}")
    import traceback
    traceback.print_exc()
