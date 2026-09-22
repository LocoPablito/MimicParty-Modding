# Compatibility

Mimic Party Modding Core **1.0.0** fingerprints the installed game at runtime and has no hardcoded game-build allowlist of its own.

## Mimic Party v0.2.33 — captured 22 September 2026

- Unity 6000.4.2f1
- Windows x64 / Steam / IL2CPP
- BepInEx 6.0.0-be.788
- Required Pack: **BepInEx Pack 1.0.2**

GameAssembly:
`03757842d82c83534a686b0acbf247c9a5b76d0a15c74c7cb27458e731d4b9d4`

Metadata:
`7f4b0ab25b7ba8ee05d9abebd507d3e2af29bd94c5ae48f4daadc2ddedc8bbf2`

The Core's build-fingerprint and reflection infrastructure does not require a runtime binary change for this captured build.

## Earlier captured builds

R5 retains documentation for the previously supported v0.2.3 / v0.1.73 baselines through repository history and feature-mod-specific compatibility files.

## Boundary

The Core being compatible does not automatically certify a feature mod; each feature mod must validate the game methods/native signatures it patches.
