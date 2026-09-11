# Mimic Party Modding Core API — for mod authors

**Core GUID:** `com.arribbaa.mimicparty.moddingcore`

## Dependency

```csharp
[BepInDependency("com.arribbaa.mimicparty.moddingcore", ">=1.0.0")]
```

Reference `MimicPartyModdingCore.dll` while compiling your plugin. Do not bundle a
second copy of the Core inside your own mod archive; declare it as a requirement.

## Register your mod

```csharp
CoreApi.Mods.Register(
    "com.example.mimicparty.myplugin",
    "My Mimic Party Plugin",
    "1.0.0");
```

## Build fingerprint

```csharp
string assemblyHash = CoreApi.Build.GameAssemblySha256;
string metadataHash = CoreApi.Build.MetadataSha256;
```

Use fingerprints for diagnostics, not as the only compatibility mechanism.

## Runtime pattern patch

```csharp
using var transaction = CoreApi.CreatePatchTransaction("com.example.myplugin");

transaction
    .Add(
        "Example patch",
        "48 8B ?? ?? ??",
        patchOffset: 3,
        expected: new byte[] { 0x05 },
        replacement: new byte[] { 0x0A })
    .Apply();
```

The Core requires a signature to resolve exactly once. It validates all requested
patches before writing the first byte. If a write fails, already-applied writes in
that transaction are rolled back.

When the transaction is disposed, it restores only bytes that still match the
replacement written by that transaction, avoiding blind overwrite of a later mod.

## Reflection helpers

`ReflectionResolver.FindUniqueType()` and `FindUniqueMethod()` are designed for the
generated BepInEx IL2CPP interop assemblies and fail closed on missing or ambiguous
targets.

## Design rule

Prefer normal managed/Harmony runtime hooks when possible. Use native pattern patches
only for IL2CPP logic that cannot be cleanly changed through the generated managed
proxy.
