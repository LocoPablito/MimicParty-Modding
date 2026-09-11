# Mimic Party Modding Core

**Version 1.0.0 — by arribbaa**

A shared runtime foundation for Mimic Party mods using BepInEx 6 Unity IL2CPP.

The Core is not a rebrand or redistribution of BepInEx. BepInEx remains an external runtime requirement. This project adds Mimic Party-specific services that other mods can depend on.

## Core services

- Mimic Party build fingerprinting (`GameAssembly.dll` and `global-metadata.dat`)
- executable-section pattern scanning for `GameAssembly.dll`
- unique-signature enforcement before runtime memory changes
- transactional runtime memory patches with rollback
- runtime patch ownership/logging
- safe type/method resolution against generated IL2CPP interop assemblies
- shared mod registry for dependent plugins
- no permanent modification of `GameAssembly.dll` on disk
- no network communication or auto-updater

## Plugin GUID

`com.arribbaa.mimicparty.moddingcore`

Dependent BepInEx 6 plugins can use:

```csharp
[BepInDependency("com.arribbaa.mimicparty.moddingcore", ">=1.0.0")]
```

Independent mods may reference the documented Core API without bundling or redistributing the Core DLL. See `LICENSE.txt` for the exact terms.

## Runtime requirement

- Windows x64
- Steam version of Mimic Party
- BepInEx 6 Unity IL2CPP x64
- Tested build target for release validation: **BepInEx 6 build #788 (`6.0.0-be.788`)**

Official BepInEx builds:
https://builds.bepinex.dev/projects/bepinex_be

Use the file named:
`BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.788+5b766a3.zip`

Newer BepInEx 6 builds may work, but #788 is the pinned/test target for this release.

## Installation

1. Install the official BepInEx 6 Unity IL2CPP x64 build into the folder containing `Mimic Party.exe`.
2. Start Mimic Party once and close it so BepInEx creates its folders.
3. Extract this Core archive directly into the Mimic Party game folder.
4. Confirm this file exists:
   `Mimic Party/BepInEx/plugins/MimicPartyModdingCore.dll`
5. Start Mimic Party normally through Steam.

## Verification / troubleshooting

Open:
`Mimic Party/BepInEx/LogOutput.txt`

A successful load contains:
`Mimic Party Modding Core v1.0.0 by arribbaa loaded.`

If a game update changes the structures required by a dependent mod, that mod should fail safely instead of blindly patching an unknown location. Check the dependent mod page for an update.

## Uninstall

Make sure no installed mod still depends on the Core, then delete:
`BepInEx/plugins/MimicPartyModdingCore.dll`

The Core does not permanently patch `GameAssembly.dll` on disk.

## Source code

https://github.com/LocoPablito/MimicParty-Modding

## Credits

- **arribbaa** — Mimic Party Modding Core
- **BepInEx Team** — BepInEx runtime/plugin framework
- **HarmonyX contributors** — runtime method patching framework used through BepInEx

No BepInEx or HarmonyX binaries are redistributed in this Core archive.
