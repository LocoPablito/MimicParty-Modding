# Pre-release checklist

Do not publish the Core or 10 Player Expansion v1.1 binaries until all items pass.

- [x] Capacity signatures unique on the 2026-09-05 GameAssembly build
- [x] Capacity signatures unique on the 2026-09-11 GameAssembly build
- [x] No absolute file offsets used by v1.1 capacity runtime patches
- [x] Runtime patch transaction validates all requested patches before writing
- [x] Runtime rollback logic implemented
- [x] v1.0.x on-disk patch migration warning documented
- [x] Core project compiles in GitHub Actions
- [x] 10 Player Expansion project compiles in GitHub Actions
- [ ] BepInEx 6 IL2CPP starts Mimic Party successfully
- [ ] Core loads without errors in BepInEx/LogOutput.txt
- [ ] Expansion loads without errors
- [ ] Lobby shows 1/10
- [ ] Player 6 can join
- [ ] 6-10 player full round passes
- [ ] Voice closes only during active performance/take and restores afterwards
- [ ] Workshop packs remain functional
- [ ] Scores/wheel work with players beyond slot 5
- [ ] Rematch retains connected players
- [ ] Second match starts and completes a round
- [ ] Real disconnect still removes the player normally

Only after all unchecked items pass should the runtime architecture replace v1.0.1 on Nexus.
