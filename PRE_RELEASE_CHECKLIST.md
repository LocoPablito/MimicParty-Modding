# Pre-release checklist — Mimic Party Modding Core

Do not publish Core v1.0.0 on Nexus until all runtime items pass.

## Build / packaging

- [x] Repository contains only the Modding Core.
- [x] Feature mods are maintained in separate repositories.
- [x] Core compiles in GitHub Actions.
- [x] BepInEx compile dependency pinned to 6.0.0-be.788.
- [x] Nexus ZIP installs from archive root as `BepInEx/plugins/...`.
- [x] Public ZIP contains no BAT/CMD/PowerShell/EXE/nested archives/original game binaries.
- [x] Public package includes author attribution to arribbaa.
- [x] Public API documentation is included in the repository.
- [x] CI generates SHA256SUMS.txt.

## Runtime validation

Use BepInEx 6 Unity IL2CPP Windows x64 build #788.

- [ ] Restore/verify original Mimic Party files before testing.
- [ ] BepInEx starts Mimic Party successfully.
- [ ] `BepInEx/LogOutput.txt` is generated without chainloader failure.
- [ ] Core loads without errors.
- [ ] Core reports the running Mimic Party build fingerprint.
- [ ] Core registry/API initializes successfully.
- [ ] Runtime patch services initialize without changing `GameAssembly.dll` on disk.
- [ ] Mimic Party exits cleanly.
- [ ] `GameAssembly.dll` remains unchanged after exit.

Only after these Core-specific runtime checks pass should Core v1.0.0 be published on Nexus.
