# Mimic Party Modding Core API — for mod authors

Core version: **1.0.0**. Core GUID: `com.arribbaa.mimicparty.moddingcore`.
Namespace: `Arribbaa.MimicParty.ModdingCore`.

## Start with a working project

[Developer Starter](../examples/StarterMod/README.md) provides a complete C# project, dependency declaration, configuration example, startup logging and cleanup. It has been compile-tested against the exact released Core DLL on Windows. Download the source-only starter from the Core 1.0.0 release assets; do not install it as a gameplay mod.

The Core is a runtime library, not a no-code editor. You implement and test your own feature. It does not require 10 Player Expansion and does not automatically make arbitrary mods multiplayer-safe or compatible with future game versions.

## Declare the runtime dependency

```csharp
[BepInDependency("com.arribbaa.mimicparty.moddingcore", ">=1.0.0")]
```

Compile against the installed `MimicPartyModdingCore.dll`. Do not copy a second Core into your feature archive; list the official Core as a requirement. The starter's project reference deliberately uses `Private=false`.

## Register and unregister your mod

```csharp
CoreApi.Mods.Register("com.example.mimicparty.myplugin", "My Mimic Party Plugin", "1.0.0");
// During your plugin's Unload:
CoreApi.Mods.Unregister("com.example.mimicparty.myplugin");
```

`CoreApi.Mods.Registered` returns the registered entries; `IsRegistered(guid)` checks a registration. Registration does not install hooks or prove compatibility.

## Game/build fingerprint

```csharp
string assemblyHash = CoreApi.Build.GameAssemblySha256;
string metadataHash = CoreApi.Build.MetadataSha256;
```

Use fingerprints for diagnostics and supported-build checks together with verified method/signature contracts. The Core exposes them; each feature mod remains responsible for deciding which builds are safe for its changes.

## Runtime patch transactions

`CoreApi.CreatePatchTransaction(ownerGuid)` creates a transaction. Each `Add` entry supplies a patch name, a researched signature, patch offset, expected bytes and replacement bytes. The signature must resolve exactly once. All requested patches are validated before the first write; already-applied writes are rolled back if an apply step fails.

**Lifetime is significant:** keep an active transaction in a plugin field for as long as the patch should remain installed. A local `using var` in `Load()` disposes it at the end of that method and undoes its matching modifications. Dispose during Unload and on failed initialization. Do not blindly restore bytes changed by another mod; the transaction implementation verifies matching replacement bytes before restoration.

No copy-and-paste native signature is provided here: those bytes and offsets must come from analysis of the actual target build, not from a fabricated universal example.

## Reflection helpers

`ReflectionResolver.FindUniqueType()` and `FindUniqueMethod()` in `Arribbaa.MimicParty.ModdingCore.Runtime` operate on loaded assemblies, including generated IL2CPP interop types. Missing or ambiguous required targets must be handled explicitly. Do not select the first same-named method without checking its signature.

Prefer normal managed/Harmony hooks when practical. If you use Harmony, give your mod its own Harmony ID and remove only your own patches on unload. Native patterns are appropriate only for carefully validated IL2CPP paths that cannot be addressed cleanly through the generated proxies.

## Distribution and permissions

Independent API-consuming mods may be created and distributed under the Core license. The provided StarterMod example files are separately MIT-licensed for reuse in your projects. Neither permission grants the right to rebrand or redistribute Core/Bootstrap binaries without permission.

Do not publish game-derived interop assemblies, proprietary game binaries, private diagnostic ZIPs or copied account data. Include your own README/license and accurate game/loader/Core requirements. The BepInEx Pack is a loader option; it does not include the Core runtime. Complete 10 Player Expansion is one authorized first-party Core bundle, not a required dependency for your independent mod.
