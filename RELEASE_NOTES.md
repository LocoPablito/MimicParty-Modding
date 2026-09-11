# Mimic Party Modding Core 1.0.0

Shared dependency for Mimic Party mods, by arribbaa. Includes the unchanged Windows-runtime-verified Core DLL and the separate Interop Bootstrap 1.0.0 compatibility component.

**BepInEx is not included.** Install the official BepInEx 6 Unity IL2CPP Windows x64 build 788, then extract this archive into the folder containing Mimic Party.exe before launching.

The bootstrap repairs only the recognized locally generated duplicate-helper layout. Its 23 Windows regression checks and 6 private supplied-file metadata/load checks pass. A fresh Windows game run with this automatic bootstrap is not yet recorded; it is distinct from the successful Core/Expansion runtime session. No full ten-client gameplay validation is claimed.

Already using the 10 Player Expansion Complete package? You already have these two files; do not add duplicate copies.

Release provenance and SHA-256 manifests are included. No game/Unity binaries, executables, installer scripts, private diagnostic files or nested archives are shipped.
