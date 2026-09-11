# Changelog

## Packaging revision R3 — 11 September 2026

- Connects the dedicated Core, BepInEx Pack and Expansion pages.
- Uses BepInEx Pack as the explicit loader/bootstrap requirement.
- Removes the duplicate bootstrap from the Core runtime archive; its source is maintained in the Pack repository.
- Places documentation in MimicPartyModdingCore/ to avoid filename collisions.
- Keeps the Core 1.0.0 runtime DLL byte-for-byte unchanged.
- Retains the reusable, separately MIT-licensed Developer Starter.

## Runtime 1.0.0

Build fingerprints, mod registration, reflection helpers and guarded native runtime patch transactions. The Core does not change gameplay by itself.
