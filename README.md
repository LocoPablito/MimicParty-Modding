# Mimic Party Modding Core

**Version 1.0.0 · by arribbaa**

The shared runtime foundation for independent Mimic Party mods. It supplies build fingerprinting, validated runtime patch transactions, reflection helpers and a common mod registry. It does not increase the player limit or require the 10 Player Expansion.

## Downloads

| Download | For whom / contents |
|---|---|
| [Modding Core 1.0.0](https://github.com/LocoPablito/MimicParty-Modding/releases/tag/v1.0.0) | Players whose mods require Core. Includes the unchanged Core runtime and Interop Bootstrap, not the BepInEx loader. |
| [Developer Starter 1.0.0](https://github.com/LocoPablito/MimicParty-Modding/releases/download/v1.0.0/MimicParty_Core_Developer_Starter_v1.0.0.zip) | Source-only example project for mod authors. Compile-tested against the released Core. |
| [BepInEx Pack for Mimic Party 1.0.0](https://github.com/LocoPablito/MimicParty-Modding/releases/tag/bepinex-pack-v1.0.0) | Separate configured loader download. Includes official build 788 and Bootstrap; does not include Core or gameplay mods. Sources and licenses are separate release assets. |
| [10 Player Expansion](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion) | A separate feature mod. Its Complete package already includes Core. |

**Installation order: one compatible loader → Core → your chosen feature mod.** Do not install renamed duplicate copies of the Core or Bootstrap.

## Requirements and installation

Documented game: Mimic Party Windows x64 / Steam **v0.1.73**, Unity **6000.4.2f1**, captured 11 September 2026. Loader: **BepInEx 6 Unity IL2CPP Windows x64 build 788**. BepInEx 5 and Mono packages are not substitutes. [Exact fingerprints](SUPPORTED_BUILD.md).

1. Close the game. If an older on-disk patch modified native game files, restore the original files first with its uninstaller or Steam verification.
2. Install **either** the BepInEx Pack above **or** the [official BepInEx archive](https://builds.bepinex.dev/projects/bepinex_be/788/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.788%2B5b766a3.zip) into the folder containing `Mimic Party.exe`. They are alternatives, not two required downloads.
3. Extract Core into the same folder before launching, retaining `BepInEx/plugins` and `BepInEx/patchers`. Then install your chosen feature mod. Expansion Complete users already have Core.
4. Start through Steam and wait for initial interop generation. BepInEx can download its required reference libraries on first launch.

Core package files:

```text
BepInEx/plugins/MimicPartyModdingCore.dll            1.0.0
BepInEx/patchers/MimicParty.InteropBootstrap.dll     1.0.0
```

The Pack includes the identical Bootstrap at the identical path. Replacing identical bytes is harmless; do not rename the DLL to retain a second copy. Back up existing loader files and custom `BepInEx.cfg` before installing over an existing setup. Do not overwrite an unrelated `winhttp.dll` or delete working plugins/interops indiscriminately.

## Create your own mod

[Start here: build a Core-based plugin](examples/StarterMod/README.md) · [Core API](docs/CORE_API_FOR_MOD_AUTHORS.md)

The starter demonstrates a hard Core dependency, registration, configuration, fingerprint logging and unload cleanup. It has been compiled on Windows against the exact release DLL and does not copy the Core into its output. It changes no gameplay. Developers need the .NET SDK; players do not need the SDK to install the loader/Core.

The example files are **MIT-licensed** for reuse. Independent API consumers are permitted by the Core license. Your feature mod uses your own name and author credit. The Core/Bootstrap themselves retain their separate license; this is not permission to redistribute/rebrand them. [Core license](LICENSE.txt) · [Starter license](examples/StarterMod/LICENSE.txt).

The Core is an API foundation, not a no-code editor, automatic network simulator or guarantee that arbitrary hooks work on future versions. Keep native patch transactions alive for their intended lifetime and dispose/unpatch only your own changes on unload. Do not publish game-generated assemblies in your own mod package.

## Compatibility and validation

The Core runtime release DLL is byte-for-byte identical to the one loaded in the recorded successful Windows session with Expansion 1.1.2. The bootstrap is a separate automatic clean-install component. It checks the documented game fingerprints and malformed generated-assembly layout, repairs only empty duplicate helper names, backs up the original and leaves already-valid files unchanged. It does not include a Unity assembly or overwrite `GameAssembly.dll`.

The Bootstrap has 23 Windows regression checks and six supplied-file metadata/load checks. The Pack preserves all 228 official loader files unchanged. **A fresh Windows game start using the newly assembled automatic Pack has not yet been recorded.** These checks are not that missing game test or a ten-client gameplay/antivirus certification. [Validation scope](VALIDATION.md).

## Support and removal

Remove feature mods before removing a required Core DLL. Remove `MimicPartyModdingCore.dll` only when no remaining mod needs it. Remove the Bootstrap DLL to remove that helper; its original generated-file backups remain under `.arribbaa-backups`. Do not restore a known-malformed generated DLL while continuing to use this loader setup. For Pack removal/disablement use its included guide and manifest, not deletion of unrelated game files.

The Pack hides the console by default and enables disk logging. Inspect `BepInEx/LogOutput.log`. To show the console set `[Logging.Console] Enabled = true` and restart. Minimize it instead of closing it during play.

[Report an issue](https://github.com/LocoPablito/MimicParty-Modding/issues) with versions and a relevant log excerpt; remove personal paths/identifiers. Do not post game binaries or private diagnostic ZIPs. Never disable antivirus as a routine installation step.

[Build instructions](BUILDING.md) · [Changelog](CHANGELOG.md) · [Security/file behaviour](SECURITY.md)

Pack configuration and first-party components: **arribbaa**. BepInEx and bundled third-party dependencies remain the work of their original teams under their own licenses. Unofficial community project; no endorsement is implied.
