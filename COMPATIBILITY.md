# Compatibility

Mimic Party Modding Core **1.0.0** is a shared runtime library. It fingerprints the installed game at runtime and does not contain a hardcoded game-build allowlist of its own. Feature mods using the Core remain responsible for validating the methods and native locations they patch.

## Captured Windows / Steam builds

### Mimic Party v0.2.33 — captured 22 September 2026

- Unity **6000.4.2f1**
- Windows x64 / Steam / IL2CPP
- BepInEx **6.0.0-be.788**
- Required Pack for the captured build: **BepInEx Pack 1.0.2**

GameAssembly.dll SHA-256:
`03757842d82c83534a686b0acbf247c9a5b76d0a15c74c7cb27458e731d4b9d4`

Mimic Party_Data/il2cpp_data/Metadata/global-metadata.dat SHA-256:
`7f4b0ab25b7ba8ee05d9abebd507d3e2af29bd94c5ae48f4daadc2ddedc8bbf2`

Core 1.0.0 loaded successfully on this captured build and reported both expected fingerprints. The Core's build-fingerprint and reflection infrastructure did not require a binary change, so R5 keeps the published Core 1.0.0 DLL byte-for-byte unchanged.

### Mimic Party v0.2.3 — captured 22 September 2026

- Unity **6000.4.2f1**
- Windows x64 / Steam / IL2CPP
- BepInEx **6.0.0-be.788**
- Required Pack: **BepInEx Pack 1.0.1**

GameAssembly.dll SHA-256:
`adc318d8ad108a2eac4e130421d20c21aef840d8ec44203fba667d6eef08e199`

Metadata SHA-256:
`96b52e058bbf5ea2a5218a2a6c01a9391c72eda9432b640bbdce8d2c10060405`

Core 1.0.0 loaded successfully with the v0.2.3 compatibility set.

### Mimic Party v0.1.73 — captured 11 September 2026

- Unity **6000.4.2f1**
- Windows x64 / Steam / IL2CPP
- BepInEx **6.0.0-be.788**

GameAssembly.dll SHA-256:
`44bbc82bdae73c1c86559a1f091ee9c7a3ae510a02c2d83f16b686ecdd9c8b11`

Metadata SHA-256:
`1586b9dd69e488706671d35490cc16377a612ca3af521031ac44464929b21e94`

Core 1.0.0 loaded successfully with Expansion 1.1.2 in the documented Windows installation.

## Boundary

The Core does not make an arbitrary dependent mod multiplayer-safe or universally compatible. A new game build can keep Core 1.0.0 compatible while still requiring updates to BepInEx compatibility handling or to a feature mod's reflection/native patches.
