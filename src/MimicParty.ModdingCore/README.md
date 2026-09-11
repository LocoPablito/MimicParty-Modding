# Mimic Party Modding Core

**Version 1.0.0 — by arribbaa**

A shared runtime foundation for Mimic Party mods using BepInEx 6 Unity IL2CPP.

The Core is not a rebrand or redistribution of BepInEx. BepInEx remains an external
runtime requirement. This project adds Mimic Party-specific services that other mods
can depend on.

## Core services

- Mimic Party build fingerprinting (`GameAssembly.dll` and `global-metadata.dat`)
- executable-section pattern scanning for `GameAssembly.dll`
- unique-signature enforcement
- transactional runtime memory patches with rollback
- runtime patch ownership/logging
- safe type/method resolution against generated IL2CPP interop assemblies
- shared mod registry for dependent plugins
- no permanent modification of `GameAssembly.dll` on disk

## Plugin GUID

`com.arribbaa.mimicparty.moddingcore`

Dependent BepInEx 6 plugins can use:

```csharp
[BepInDependency("com.arribbaa.mimicparty.moddingcore", ">=1.0.0")]
```

## Runtime requirement

BepInEx 6, Unity IL2CPP, Windows x64.

Install BepInEx into the Mimic Party game root and run the game once. Then place
`MimicPartyModdingCore.dll` in:

`Mimic Party/BepInEx/plugins/`

## Author

arribbaa
