# Mimic Party Modding Core

**Version 1.0.0 · by arribbaa**

The shared runtime foundation for Mimic Party mods. The Core supplies build fingerprinting, validated runtime patch transactions, reflection helpers and a common mod registry. It does not increase the player limit by itself.

## Downloads

[Download Core](https://github.com/LocoPablito/MimicParty-Modding/releases/latest) · [10 Player Expansion](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion) · [Support](https://github.com/LocoPablito/MimicParty-Modding/issues)

For the 10 Player Expansion, the author's **Complete — Core included** package is the simplest option. Do not install a second copy of the Core under another filename or subfolder.

## Requirements

- Mimic Party, Windows x64 / Steam. Documented game build: **v0.1.73**, captured 11 September 2026, Unity **6000.4.2f1**.
- **BepInEx 6 Unity IL2CPP, Windows x64, build 788**. BepInEx 5 and Unity Mono packages are not substitutes.

[Official BepInEx build 788 download](https://builds.bepinex.dev/projects/bepinex_be/788/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.788%2B5b766a3.zip)

BepInEx is an external dependency, not included in this package. Do not substitute somebody else's installed BepInEx folder or generated interop DLLs.

## Installation

1. Close Mimic Party. If upgrading from an old on-disk patch, restore the original game files first using its uninstaller or Steam's file verification.
2. Extract the official BepInEx archive into the folder containing `Mimic Party.exe`.
3. Extract this Core package into the same folder **before starting the game**. Preserve the `BepInEx/plugins` and `BepInEx/patchers` paths.
4. Install the feature mods you intend to use, then start the game normally through Steam. Initial interop generation can take longer than a normal launch; do not interrupt it.

Installed components:

```text
BepInEx/plugins/MimicPartyModdingCore.dll             1.0.0
BepInEx/patchers/MimicParty.InteropBootstrap.dll      1.0.0
```

The separate compatibility bootstrap runs before ordinary plugins. For the recognized game and generated-metadata layout it repairs duplicate empty helper names in the local generated `UnityEngine.CoreModule.dll`, preserving original bytes in a backup. It does not redistribute a Unity assembly or replace `GameAssembly.dll`. Already-valid generated files are left untouched.

## Verification and compatibility

The Core runtime was loaded successfully in an actual Windows game session with Expansion 1.1.2. Its release DLL is byte-for-byte identical to that tested file. The automatic bootstrap is a separate clean-install component; see [validation scope](VALIDATION.md) for the distinction between metadata/regression tests and a full game test.

Do not assume a future Steam update is compatible merely because the mod worked previously. The bootstrap accepts only the documented fingerprint; an unknown layout is not modified. See [supported build](SUPPORTED_BUILD.md).

## Uninstall / troubleshooting

Remove `MimicPartyModdingCore.dll` only after removing mods that require it. Remove `MimicParty.InteropBootstrap.dll` to remove the compatibility helper. Generated-file backups are stored below the affected folder in `.arribbaa-backups` with `.original` filenames; they are not additional plugins. Do not restore a known-malformed generated DLL while still using this BepInEx setup.

If a console is visible, minimize it rather than closing it during play. To hide it on subsequent launches, set `Enabled = false` in the `[Logging.Console]` section of `BepInEx/config/BepInEx.cfg`, then restart. The diagnostic log remains in `BepInEx/LogOutput.log` unless its disk logging configuration was changed separately.

For support include the game version, installed mod versions and the relevant log excerpt. Remove personal paths and identifiers before posting publicly. Never upload your game binaries or private diagnostic archives to an issue.

## Mod authors / licensing

[Core API](docs/CORE_API_FOR_MOD_AUTHORS.md) · [Build instructions](BUILDING.md) · [Changelog](CHANGELOG.md) · [Security and file behaviour](SECURITY.md)

Independent mods can reference the documented Core API under the license. Third-party redistribution still requires permission. The author's own official Complete package is an explicitly authorized distribution; it is not a general permission for others to bundle the Core. Source is available for inspection under [LICENSE.txt](LICENSE.txt).
