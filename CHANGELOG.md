# Changelog

## Packaging revision R7 — 30 September 2026

- Promotes the Mimic Party **v0.2.5 / Steam build 25629135** compatibility set to the stable, live-verified release path.
- Real Windows acceptance confirmed the BepInEx IL2CPP chainloader initialized and Core **1.0.0** loaded successfully.
- Core reported the expected v0.2.5 GameAssembly and metadata fingerprints.
- Core runtime DLL remains byte-for-byte unchanged; R7 is the final packaging/documentation revision for this game build.
- Coordinated live acceptance also confirmed 10 Player Expansion 1.1.5 hook installation, native capacity patching and its 10/10/10 self-check.

## Packaging revision R6 — 30 September 2026

- Documents the captured Mimic Party **v0.2.5** / Steam build **25629135** Windows x64 IL2CPP fingerprints.
- Records GameAssembly SHA-256 `02546fc8797c32a9f98a8b93a7eefc11f52fb58e83fd51db29fd83eb5601d25a` and metadata SHA-256 `dded356e7a47bc941b63e4a09280bd7fb8e777bf5748366d47345363df9a9bbf`.
- Keeps the Core **1.0.0** runtime DLL byte-for-byte unchanged; the build-fingerprint and reflection infrastructure itself does not require a binary change.
- Points v0.2.5 users to the Pack **1.0.3** compatibility candidate and Expansion **1.1.5** compatibility candidate.
- This revision is published as a **prerelease** until the v0.2.5 live startup/feature acceptance run completes; static capture alone is not represented as live verification.

## Packaging revision R5 — 22 September 2026

- Documents the captured Mimic Party v0.2.33 Windows x64 / Steam fingerprint.
- Points v0.2.33 users to BepInEx Pack 1.0.2, whose Bootstrap 1.0.2 contains the verified v0.2.33 interop-repair profile.
- Keeps the Core 1.0.0 runtime DLL byte-for-byte unchanged.
- Keeps the public Core API and Developer Starter unchanged.
- Live v0.2.33 acceptance passed: Core 1.0.0 loaded and reported the expected new fingerprints.
- Does not claim that arbitrary dependent mods automatically support v0.2.33; each feature mod must validate its own hooks/patches.

## Packaging revision R4 — 22 September 2026

- Documented the captured Mimic Party v0.2.3 Windows x64 / Steam fingerprint.
- Pointed users to BepInEx Pack 1.0.1.
- Kept the Core 1.0.0 runtime DLL byte-for-byte unchanged.

## Packaging revision R3 — 11 September 2026

- Connected the dedicated Core, BepInEx Pack and Expansion pages.
- Used BepInEx Pack as the explicit loader/bootstrap requirement.
- Removed the duplicate bootstrap from the Core runtime archive; its source is maintained in the Pack repository.
- Placed documentation in MimicPartyModdingCore/ to avoid filename collisions.
- Kept the Core 1.0.0 runtime DLL byte-for-byte unchanged.
- Retained the reusable, separately MIT-licensed Developer Starter.

## Runtime 1.0.0

Build fingerprints, mod registration, reflection helpers and guarded native runtime patch transactions. The Core does not change gameplay by itself.
