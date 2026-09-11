# Mimic Party Modding Platform — by arribbaa

This repository contains the source for:

- **Mimic Party Modding Core v1.0.0**
- **Mimic Party - 10 Player Expansion v1.1.0**

The Core provides reusable Mimic Party-specific runtime services. The 10 Player Expansion depends on the Core.

## Architecture

```text
BepInEx 6 Unity IL2CPP (Windows x64)
        ↓
Mimic Party Modding Core v1.0.0+
        ↓
Mimic Party - 10 Player Expansion v1.1.0
```

The v1.1 runtime architecture avoids permanent `GameAssembly.dll` modification. BepInEx is an independent open-source project and is not redistributed by this repository.

## Release test target

The compile/runtime dependency is pinned to **BepInEx 6 Unity IL2CPP build #788 (`6.0.0-be.788`)**.

Official BepInEx build server:
https://builds.bepinex.dev/projects/bepinex_be

## Build

Requires .NET 8 SDK or newer.

```powershell
./build.ps1
```

Nexus-ready archives and `SHA256SUMS.txt` are generated in `dist/`.

CI also validates the public archive layout and rejects unexpected executables/scripts, nested archives, original Mimic Party binaries, or missing author attribution.

## Release status

Compilation and static compatibility checks pass. **Runtime/in-game testing remains a hard release gate.** See `PRE_RELEASE_CHECKLIST.md` and `RELEASE_AUDIT_NOTES.md`.

Do not replace the public v1.0.1 10 Player Expansion release until BepInEx/Core/v1.1 has passed the full local multiplayer test matrix.

## Source / author

Repository: https://github.com/LocoPablito/MimicParty-Modding

Author: **arribbaa**
