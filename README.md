# Mimic Party Modding Platform — by arribbaa

This repository contains two separate BepInEx 6 Unity IL2CPP plugins:

1. **Mimic Party Modding Core v1.0.0**
2. **Mimic Party - 10 Player Expansion v1.1.0**

The Core provides reusable Mimic Party-specific runtime services. The 10 Player
Expansion depends on the Core.

## Why this architecture

The previous 10 Player Expansion v1.0.x changed `GameAssembly.dll` on disk and was
build-specific. v1.1 moves the mod to runtime loading:

- no permanent GameAssembly modification
- no BAT/PowerShell installer in the end-user Nexus archives
- BepInEx loads normal plugin DLLs
- structural signatures replace absolute binary offsets for the remaining native
  capacity constants
- Harmony runtime hooks handle the main player count, voice state and rematch logic
- runtime changes disappear automatically when the game process exits

## Build

Requires .NET 8 SDK or newer.

```powershell
./build.ps1
```

Nexus-ready archives are generated in `dist/`.

## End-user dependency chain

```text
BepInEx 6 Unity IL2CPP (Windows x64)
        ↓
Mimic Party Modding Core v1.0.0+
        ↓
Mimic Party - 10 Player Expansion v1.1.0
```

BepInEx is an independent open-source project and is not redistributed by this
repository.

## Author

arribbaa
