# Mimic Party Modding Core

**Runtime 1.0.0 · by arribbaa**

Shared runtime API for independent Mimic Party mods: build fingerprints, mod registration, reflection helpers and guarded runtime patch transactions. The Core does not change gameplay or raise the player limit on its own.

[Download on Nexus](https://www.nexusmods.com/mimicparty/mods/2) · [GitHub downloads](https://github.com/LocoPablito/MimicParty-Modding/releases/latest) · [API guide](docs/CORE_API_FOR_MOD_AUTHORS.md) · [Report an issue](https://github.com/LocoPablito/MimicParty-Modding/issues)

## Install

1. Close Mimic Party.
2. Install [BepInEx Pack for Mimic Party](https://www.nexusmods.com/mimicparty/mods/3) 1.0.0, Windows x64 IL2CPP.
3. Extract the **Core runtime ZIP** into the folder containing `Mimic Party.exe`.
4. Install your chosen feature mod and start normally through Steam.

This package installs only `BepInEx/plugins/MimicPartyModdingCore.dll`. Documentation lives in `MimicPartyModdingCore/` so it does not overwrite other packages' guides.

**Revision R2:** BepInEx and Interop Bootstrap belong to the separate Pack and are not bundled here. The Core DLL itself is unchanged. Already using the older Expansion Complete package? Its Core DLL has the same hash; do not install a renamed duplicate or remove the shared bootstrap from `BepInEx/patchers`.

## For mod authors

Download the optional **Developer Starter**, or use [the example project](examples/StarterMod/README.md). It is source code for developers, not a plugin players must install. A .NET SDK is needed to compile it, not to play.

Declare `com.arribbaa.mimicparty.moddingcore` as a hard dependency with a compatible version. Reference the installed Core DLL with `Private=false`, register your own unique mod GUID, and clean up your own hooks/transactions during unload.

Independent API-consuming mods are permitted by the Core license. Starter files are separately MIT-licensed. Do not rebrand or redistribute Core binaries without permission. The Core never requires the 10 Player Expansion.

## Support and removal

Use `BepInEx/LogOutput.log` for startup errors; remove private paths and identifiers before posting. Do not publish game binaries or diagnostic archives. Remove the Core DLL only after removing mods that require it. Keep the BepInEx Pack if other mods use it.

[Compatibility](COMPATIBILITY.md) · [Changelog](CHANGELOG.md) · [Build instructions](BUILDING.md) · [License](LICENSE.txt) · [Security](SECURITY.md)
