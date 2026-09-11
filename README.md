# Mimic Party Modding Core — by arribbaa

Shared BepInEx 6 Unity IL2CPP runtime foundation for Mimic Party mods.

This repository now contains **only the Core**. Feature mods live in separate repositories.

## Architecture

```text
BepInEx 6 Unity IL2CPP x64
        ↓
Mimic Party Modding Core v1.0.0+
        ↓
Feature mods
```

Current feature mod:

- **Mimic Party - 10 Player Expansion**  
  https://github.com/LocoPablito/Mimic-Party---10-Player-Expansion

## What the Core provides

- Mimic Party build fingerprinting
- GameAssembly executable-section scanning
- structural native signature resolution
- transactional runtime patching with rollback
- shared type/method reflection helpers
- common mod registry/API
- no permanent `GameAssembly.dll` modification on disk
- no network communication or auto-updater

## Requirements

- Mimic Party on Windows / Steam
- BepInEx 6 Unity IL2CPP x64 build #788 (`6.0.0-be.788`)

Official BepInEx builds:
https://builds.bepinex.dev/projects/bepinex_be

## Build

Requires .NET 8 SDK or newer.

```powershell
./build.ps1
```

The Nexus-ready Core archive and `SHA256SUMS.txt` are generated in `dist/`.

## Public API

See:

`docs/CORE_API_FOR_MOD_AUTHORS.md`

Dependent mods should reference the Core as an external dependency. Do not bundle the Core DLL inside feature-mod downloads.

## Release status

Compilation and packaging checks pass. Runtime validation with BepInEx/Mimic Party remains a release gate before the first Nexus Core release.

## Source / author

Repository: https://github.com/LocoPablito/MimicParty-Modding

Author: **arribbaa**
