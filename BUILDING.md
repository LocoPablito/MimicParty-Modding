# Building Mimic Party Modding Core

by arribbaa

## Requirements

- .NET 8 SDK or newer
- Internet access for NuGet restore

The Core targets BepInEx 6 Unity IL2CPP build #788 (`6.0.0-be.788`).

## Build

```powershell
./build.ps1
```

The script restores and builds:

```text
src/MimicParty.ModdingCore/MimicParty.ModdingCore.csproj
```

Then it creates the Nexus-ready archive in `dist/`.

## Output

```text
dist/
├─ MimicParty_Modding_Core_v1.0.0_by_arribbaa_NEXUS.zip
└─ SHA256SUMS.txt
```

The public Core archive contains only:

```text
BepInEx/plugins/MimicPartyModdingCore.dll
README.txt
CHANGELOG.txt
LICENSE.txt
```

It does not bundle BepInEx, feature mods, HarmonyX, or original Mimic Party binaries.

## Feature mods

Feature mods are maintained in separate repositories. The 10 Player Expansion lives at:

https://github.com/LocoPablito/Mimic-Party---10-Player-Expansion
