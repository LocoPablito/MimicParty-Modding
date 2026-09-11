# Core API for mod authors

Core GUID: `com.arribbaa.mimicparty.moddingcore`. Runtime: **1.0.0**. Namespace: `Arribbaa.MimicParty.ModdingCore`.

[Starter project](../examples/StarterMod/README.md) · [Core download](https://www.nexusmods.com/mimicparty/mods/2) · [Loader Pack](https://www.nexusmods.com/mimicparty/mods/3)

## Declare a dependency

```csharp
[BepInDependency("com.arribbaa.mimicparty.moddingcore", ">=1.0.0")]
```

Reference the installed Core DLL with `Private=false` so you do not distribute duplicate copies. BepInEx IL2CPP plugins derive from BasePlugin, not the Unity Mono BaseUnityPlugin.

## Register your mod

```csharp
CoreApi.Mods.Register("com.example.mimicparty.myplugin", "My Plugin", "1.0.0");
// In your plugin's Unload:
CoreApi.Mods.Unregister("com.example.mimicparty.myplugin");
```

Registered returns the current registry entries; IsRegistered checks a GUID. Registration alone does not install hooks or establish compatibility.

## Build fingerprints

```csharp
string gameHash = CoreApi.Build.GameAssemblySha256;
string metadataHash = CoreApi.Build.MetadataSha256;
```

Each mod must decide which builds and method contracts it supports. The Core is not a universal compatibility or multiplayer-safety layer.

## Patch transactions

CoreApi.CreatePatchTransaction(ownerGuid) creates a transaction. Add entries with a researched signature, patch offset, expected bytes and same-length replacement bytes. The signature must resolve uniquely. All entries are resolved and their expected bytes checked before writes begin. Applied writes are rolled back on failure where matching-byte restoration is possible.

Keep the active transaction in a plugin field. A local `using var` inside Load disposes it at method exit and reverses matching changes. Dispose on Unload and on failed initialization. Use non-overlapping, in-bounds offsets verified for your target; arbitrary offsets are not made safe merely by a unique signature. Avoid concurrent patch mutation. Dispose only your own transactions and unpatch only your own Harmony ID.

Restoration refuses bytes changed by another mod. Read/verify the behaviour you alter; patch installation alone does not establish complete gameplay behaviour. No universal copy-and-paste native signature is provided.

## Reflection

ReflectionResolver.FindUniqueType and FindUniqueMethod in Arribbaa.MimicParty.ModdingCore.Runtime operate on loaded assemblies, including generated interop types. Handle missing or ambiguous targets explicitly. Prefer managed Harmony hooks where appropriate; use native writes only for researched IL2CPP paths that cannot be handled through the proxy.

## Distribution

Independent API-consuming mods are permitted by the Core license. Starter files are separately MIT-licensed. Publish your own DLL, documentation and license. Declare the Core and Pack as prerequisites; the Expansion is not a prerequisite. Never redistribute game-generated interop assemblies or private diagnostic captures.
