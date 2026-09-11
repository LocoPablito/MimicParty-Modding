# Release audit notes

This file intentionally tracks the remaining release gate for the runtime architecture.

- Source compiles in GitHub Actions.
- Runtime capacity signatures were validated against the 2026-09-05 and 2026-09-11 GameAssembly builds.
- Nexus packages are compiled BepInEx plugins and do not permanently modify GameAssembly.dll on disk.
- **Runtime/in-game validation is still required before Nexus publication:** BepInEx startup, Core load, 1/10 lobby, player 6+, voice isolation, scoring, Rematch, second round, and normal disconnect behavior.

Do not publish the runtime v1.1 architecture as the primary Nexus file until the runtime checklist passes.
