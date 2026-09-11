# Pre-release checklist

Do not publish the Core or 10 Player Expansion v1.1 binaries until all items pass.

## Static / build / packaging

- [x] Capacity signatures unique on the 2026-09-05 GameAssembly build
- [x] Capacity signatures unique on the 2026-09-11 GameAssembly build
- [x] No absolute file offsets used by v1.1 capacity runtime patches
- [x] Runtime patch transaction validates all requested patches before writing
- [x] Runtime rollback logic implemented
- [x] v1.0.x on-disk patch migration warning documented
- [x] Core project compiles in GitHub Actions
- [x] 10 Player Expansion project compiles in GitHub Actions
- [x] BepInEx compile dependency pinned to 6.0.0-be.788
- [x] Nexus ZIPs install from archive root as `BepInEx/plugins/...`
- [x] Nexus ZIPs contain no BAT/CMD/PowerShell/EXE/nested archives/original game binaries
- [x] Public package files contain author attribution to arribbaa
- [x] Public source repository linked in end-user documentation
- [x] Project-specific Core and Expansion licenses included
- [x] CI generates SHA256SUMS.txt for every release build

## Required runtime validation

Use **BepInEx 6 Unity IL2CPP Windows x64 build #788** for the release test.

- [ ] BepInEx 6 build #788 starts Mimic Party successfully
- [ ] `BepInEx/LogOutput.txt` is generated without chainloader failure
- [ ] Core loads without errors in `BepInEx/LogOutput.txt`
- [ ] Expansion loads without errors
- [ ] Lobby shows `1/10`
- [ ] Player 6 can join and remains connected
- [ ] 6-10 player full round passes
- [ ] Normal lobby/reaction voice remains functional
- [ ] Live voice is isolated during the active performance/playback window
- [ ] Live voice returns immediately after the performance window
- [ ] Workshop packs remain functional
- [ ] Takes and playback work for players beyond slot 5
- [ ] Scores/wheel work with players beyond slot 5
- [ ] Rematch retains connected players
- [ ] Second match starts and completes at least one round
- [ ] A genuine disconnect still removes the player normally
- [ ] Game exits cleanly and no permanent `GameAssembly.dll` change remains

Only after all runtime items pass should the Core be published and v1.1 replace v1.0.1 as the primary 10 Player Expansion file on Nexus Mods.
