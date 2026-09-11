#!/usr/bin/env python3
import sys
from pathlib import Path

SIGNATURES = {
    "HostCapacityGate": (
        "48 8B 43 40 48 85 C0 0F 84 ?? ?? ?? ?? 83 78 18 05 "
        "0F 8D ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 83 B9 E4 00 00 00 00",
        16,
    ),
    "ConnectStatusMax": (
        "48 8D 54 24 30 48 8B D8 C7 44 24 30 05 00 00 00 "
        "E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 45 33 C9 4C 8B C0 48 8B D3",
        12,
    ),
    "FullRoomMax": (
        "48 8D 54 24 30 48 8B D8 C7 44 24 30 05 00 00 00 "
        "E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 45 33 C0 48 8B D0",
        12,
    ),
    "RoomListMax": (
        "48 8D 4C 24 60 48 89 44 24 38 0F 57 C0 8B 44 24 5C 4C 8B CD "
        "44 88 64 24 30 4D 8B C5 C7 44 24 28 05 00 00 00 48 8B D7 89 44 24 20",
        32,
    ),
}

def parse(sig):
    return [None if t in ("?", "??") else int(t, 16) for t in sig.split()]

def find_all(data, sig):
    p = parse(sig)
    out = []
    for i in range(len(data) - len(p) + 1):
        if all(v is None or data[i+j] == v for j, v in enumerate(p)):
            out.append(i)
    return out

def main(paths):
    ok = True
    for path in paths:
        data = Path(path).read_bytes()
        print(f"\n{path}")
        for name, (sig, patch_offset) in SIGNATURES.items():
            hits = find_all(data, sig)
            good = len(hits) == 1 and data[hits[0] + patch_offset] == 0x05
            print(f"  {name}: matches={len(hits)}", end="")
            if hits:
                print(f" file_offset=0x{hits[0]:X} patch=0x{hits[0]+patch_offset:X}", end="")
            print(" PASS" if good else " FAIL")
            ok &= good
    raise SystemExit(0 if ok else 1)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("usage: validate_native_signatures.py GameAssembly.dll [GameAssembly2.dll ...]")
        raise SystemExit(2)
    main(sys.argv[1:])
