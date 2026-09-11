# Mimic Party Core — Developer Starter 1.0.0

Starter by arribbaa. This directory is source for mod authors, not a gameplay mod to install.

## What you need

- A legitimate Windows x64 Steam installation of Mimic Party.
- BepInEx 6 IL2CPP Windows x64 build 788 (the Mimic Party BepInEx Pack is the configured alternative).
- The official Modding Core 1.0.0 runtime package, installed in the game.
- .NET 8 SDK to compile the net6.0 plugin. Players do not need to install this SDK.

## Build your first plugin

1. Copy this directory outside the game and give your plugin a unique name.
2. In `Plugin.cs`, change `ModGuid` from `com.example.mimicparty.starter`, the plugin title and namespace. Change AssemblyName in the project as well. Do not identify your independent plugin as an arribbaa product.
3. From this directory run:

```powershell
dotnet build -c Release -p:MimicPartyPath="C:\Program Files (x86)\Steam\steamapps\common\Mimic Party"
```

A Core DLL at another location can be selected with `-p:CoreReference="C:\path\MimicPartyModdingCore.dll"`.

4. Copy **only your own plugin DLL** from `bin/Release/net6.0` into `BepInEx/plugins/YourMod/`.
5. Start the game, then inspect `BepInEx/LogOutput.log` for the starter's load message. The sample only registers itself, creates a configuration entry and logs build fingerprints; it does not change gameplay or contact a network.

## Build versus runtime

The starter is compile-tested against the exact publicly released Core DLL. A successful compile does not test your feature inside the game. Develop one feature at a time and verify against the exact installed game build. For game-specific method hooks you may need to reference generated interop assemblies from your own installation. Do not redistribute those files.

## Core API

Namespace: `Arribbaa.MimicParty.ModdingCore`.
Core GUID: `com.arribbaa.mimicparty.moddingcore`.

Use a hard BepInEx dependency (`>=1.0.0`) so the Core initializes before your plugin. CoreApi.Build exposes game and metadata fingerprints; CoreApi.Mods provides registration/unregistration. Reflection helpers require unique matching types/methods. CoreApi.CreatePatchTransaction validates native signatures and expected bytes before applying a transaction.

A patch transaction must remain alive as long as your feature needs the patch. Disposing it restores matching bytes. Do not create a `using var` transaction inside Load and expect its patch to survive the end of Load. Keep it in a field, and dispose it on Unload or after a failed initialization. Unpatch only your own Harmony ID.

Do not guess native patterns or offsets, bypass compatibility checks, or assume the Core makes every future game version compatible. The Core is an API foundation, not a no-code mod editor, network simulator or automatic compatibility layer for arbitrary mods.

## Publish your independent mod

Distribute your own DLL, README and license. Do not include BepInEx, the Core DLL or game-generated assemblies unless their relevant distribution terms permit it. Declare the Core and loader as dependencies. Once the Nexus Core page exists, link to that actual page; until then use the official GitHub release URL.

The starter files are reusable under the accompanying MIT license. This permission applies to the starter only. The Core/Bootstrap binaries retain their own license; permission to build independent API consumers is already granted there. Copying or rebranding the Core itself is not required to create a mod.

API documentation: https://github.com/LocoPablito/MimicParty-Modding/blob/main/docs/CORE_API_FOR_MOD_AUTHORS.md
Core release: https://github.com/LocoPablito/MimicParty-Modding/releases/tag/v1.0.0
