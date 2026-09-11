# Changelog

## 1.0.0 — public package, 11 September 2026

- Publishes the unchanged Core runtime verified in the Windows game test.
- Provides game/build fingerprinting, executable-section signature scanning, validated patch transactions with rollback, reflection helpers and a shared mod registry.
- Adds the separate Interop Bootstrap 1.0.0 component to the distribution. It repairs the recognized generated CoreModule duplicate-helper layout locally before ordinary plugin loading; it does not ship Unity/game binaries.
- Provides manual extraction installation, SHA-256 manifests, provenance and updated support documentation.
- Author-approved bundling with 10 Player Expansion is supported while a separate Core Nexus listing is introduced.

The bootstrap packaging is distinct from the unchanged Core runtime. Validation scope is documented in VALIDATION.md. Neither component promises compatibility with unknown future game builds.
