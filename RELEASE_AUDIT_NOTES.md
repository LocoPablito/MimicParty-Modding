# Release audit notes — Mimic Party Modding Core

This repository is Core-only.

- Core source compiles in GitHub Actions.
- Public package contains only the Core plugin DLL and documentation.
- Feature mods are maintained separately and are not bundled here.
- Core runtime services are designed to avoid permanent `GameAssembly.dll` modification on disk.
- Runtime/in-game validation is still required before the first Nexus Core release: BepInEx startup, Core load, build fingerprinting, registry/API initialization, clean game exit, and confirmation that `GameAssembly.dll` remains unchanged.

10 Player Expansion repository:
https://github.com/LocoPablito/Mimic-Party---10-Player-Expansion
