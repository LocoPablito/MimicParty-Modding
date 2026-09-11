# Mimic Party Modding Platform

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

## Build

Requires .NET 8 SDK or newer.

```powershell
./build.ps1
```

Nexus-ready archives are generated in `dist/`.

## Author

arribbaa
