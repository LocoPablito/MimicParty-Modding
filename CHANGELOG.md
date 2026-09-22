# Changelog

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
