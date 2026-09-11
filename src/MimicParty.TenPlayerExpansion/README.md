# Mimic Party - 10 Player Expansion

**Version 1.1.0 — by arribbaa**

The runtime-plugin successor to the v1.0.x disk patcher.

## What changed in 1.1

- No longer modifies `GameAssembly.dll` on disk.
- Runs as a BepInEx 6 IL2CPP plugin.
- Requires **Mimic Party Modding Core v1.0.0+**.
- Uses structural runtime signatures instead of fixed file offsets for the remaining native capacity constants.
- Uses runtime method hooks for the main MaxPlayers getter, round state, voice behavior and Rematch logic.
- Runtime patches are validated transactionally and rolled back on failure.
- Mimic Party updates that only move code may no longer require a new mod release.

## Features

- Up to 10 players in private lobbies.
- Players 6-10 can pass the host capacity gate.
- Room/listing maximums follow the configured player count.
- Stock voice behavior is preserved, with additional live-voice isolation during the active playback/performance window.
- Live voice returns after the performance window ends.
- Connected players stay together when Rematch starts.
- Genuine disconnect handling remains stock.
- Workshop sound packs remain handled by Mimic Party normally.

## Requirements

1. Mimic Party on Windows / Steam.
2. BepInEx 6 Unity IL2CPP x64 — release test target: **build #788 (`6.0.0-be.788`)**.
3. Mimic Party Modding Core v1.0.0 or newer.
4. For lobbies above 5 players, all participants should use the same 10 Player Expansion/Core versions.

Official BepInEx builds:
https://builds.bepinex.dev/projects/bepinex_be

## Important: updating from v1.0.x

The old v1.0.x release permanently patched `GameAssembly.dll` until restored.

Before installing v1.1:

1. Close Mimic Party.
2. Run the v1.0.x `UNINSTALL_RESTORE_ORIGINAL.bat` if the old release is currently installed.
3. In Steam, verify the integrity of Mimic Party's installed files.
4. Only then install BepInEx/Core/v1.1.

Do not run v1.1 on top of a v1.0.x-patched `GameAssembly.dll`.

## Installation

1. Install BepInEx 6 Unity IL2CPP x64 and start Mimic Party once.
2. Install **Mimic Party Modding Core v1.0.0+**.
3. Extract this archive directly into the Mimic Party game folder.
4. Confirm this file exists:
   `Mimic Party/BepInEx/plugins/MimicParty10PlayerExpansion.dll`
5. Start Mimic Party normally through Steam.
6. Create a private lobby. With the default configuration the lobby should show `1/10 players`.

## Configuration

After the first successful run:

`BepInEx/config/com.arribbaa.mimicparty.10playerexpansion.cfg`

`MaxPlayers` supports **6-10**. Default: **10**.

## Voice behavior

- Normal lobby/reaction moments: stock live voice behavior.
- Active playback/performance: live player voice is isolated so the active take can be heard clearly.
- After the active performance: live voice returns automatically.

The plugin preserves the game's existing voice-closing conditions and only adds the performance isolation condition.

## Workshop sound packs

Workshop packs are not modified by this plugin. Every player should have the selected Workshop pack installed and fully downloaded before the match starts.

## Rematch

When Rematch starts, the plugin prevents the replay-vote cleanup path from removing players who are still connected. Genuine network disconnects continue through the game's normal disconnect handling.

## Verification / troubleshooting

Open:
`Mimic Party/BepInEx/LogOutput.txt`

A successful load should include messages for:
- `Mimic Party Modding Core v1.0.0 by arribbaa loaded.`
- `Mimic Party - 10 Player Expansion v1.1.0 by arribbaa starting.`
- the four runtime capacity patches being applied
- the expansion reporting itself active

If the structural signatures or required runtime methods cannot be resolved after a future Mimic Party update, do not try to force the plugin. Check the Nexus page for an updated release.

## Uninstall

Close Mimic Party and delete:
`BepInEx/plugins/MimicParty10PlayerExpansion.dll`

Optionally delete the configuration file:
`BepInEx/config/com.arribbaa.mimicparty.10playerexpansion.cfg`

No `GameAssembly.dll` restoration is required for v1.1 because this version patches only the running process.

## Source code

https://github.com/LocoPablito/MimicParty-Modding

## Credits

- **arribbaa** — 10 Player Expansion
- **BepInEx Team** — BepInEx runtime/plugin framework
- **HarmonyX contributors** — runtime method patching framework used through BepInEx

No BepInEx, HarmonyX, or original Mimic Party binaries are redistributed in this archive.
