# Mimic Party Core — Developer Starter 1.0.0

Starter by arribbaa. Source for independent mod authors; not a gameplay plugin for players to install.

## Prerequisites

Mimic Party Windows x64, [BepInEx Pack](https://www.nexusmods.com/mimicparty/mods/3), [Core runtime](https://www.nexusmods.com/mimicparty/mods/2), and the .NET 8 SDK. Players do not need the SDK.

## Build

Copy this project outside the game folder. Change the example GUID, title, namespace and AssemblyName for your independent mod.

```powershell
dotnet build -c Release -p:MimicPartyPath="C:\Program Files (x86)\Steam\steamapps\common\Mimic Party"
```

Select a Core DLL elsewhere with `-p:CoreReference="C:\path\MimicPartyModdingCore.dll"`.

Copy only your own compiled plugin DLL to BepInEx/plugins/YourMod/. The example registers itself, creates a configuration entry and logs build fingerprints. It does not change gameplay. The Core reference uses Private=false and is not copied into your output.

## Lifecycle

The project compiled against the released Core API. A successful compile does not establish a new feature's in-game behaviour. Verify your target build and actual game contracts. Use BasePlugin for IL2CPP; do not copy Unity Mono examples using BaseUnityPlugin.

Keep active patch transactions alive in fields and dispose them on Unload or failed initialization. Unpatch only your own Harmony ID. Do not guess native offsets or assume future compatibility.

## Publish

Publish your DLL with accurate requirements and your own credits. The Core API is available to independent mods under its license; it does not require 10 Player Expansion. The example files are MIT-licensed for reuse. That permission does not authorize rebranding or redistributing the Core itself.

[Full API guide](https://github.com/LocoPablito/MimicParty-Modding/blob/main/docs/CORE_API_FOR_MOD_AUTHORS.md)
