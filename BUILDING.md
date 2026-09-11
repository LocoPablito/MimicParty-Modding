# Building the Mimic Party Modding Platform

**by arribbaa**

This repository builds two managed BepInEx 6 Unity IL2CPP plugins:

- `MimicPartyModdingCore.dll`
- `MimicParty10PlayerExpansion.dll`

No Mimic Party game binaries are required to compile the source.

## Requirements

- Windows, Linux, or macOS build host with the .NET 8 SDK
- The release CI is pinned to **.NET SDK 8.0.425**
- Internet access to restore NuGet packages

The projects target `.NET 6.0` and pin the BepInEx compile dependency to `BepInEx.Unity.IL2CPP 6.0.0-be.788`. HarmonyX is pinned to `2.10.2`, matching the BepInEx runtime line used for release validation.

Package restore uses the public NuGet, BepInEx, and Samboy package feeds declared by the project files so BepInEx's Cpp2IL dependency resolves to its requested build instead of silently falling back to another version.

## One-command build

From the repository root:

```powershell
./build.ps1
```

The script performs restore, Release compilation, packaging, and creates the public archives in `dist/`.

Expected outputs:

```text
dist/
├── MimicParty_Modding_Core_v1.0.0_by_arribbaa_NEXUS.zip
├── MimicParty_10_Player_Expansion_v1.1.0_by_arribbaa_NEXUS.zip
└── SHA256SUMS.txt
```

## Package validation

After building, run:

```powershell
python ./tests/validate_release_packages.py
```

The validator checks the exact Nexus archive layout and rejects unexpected executables/scripts, nested archives, original Mimic Party binaries, missing author attribution, or prohibited internal branding strings.

NuGet dependency-version fallback warning `NU1603` is treated as an error for release builds.

## Continuous integration

GitHub Actions runs the same build and package validation on pull requests and pushes to `main` using `.github/workflows/build.yml`.

## Runtime validation

A successful compile is not treated as proof that a game mod works in a live session. Before a Nexus release, complete the runtime matrix in `PRE_RELEASE_CHECKLIST.md` using the supported Mimic Party build and the documented BepInEx 6 test target.
