# Mimic Party - 10 Player Expansion

**Version 1.1.0 — by arribbaa**

The runtime-plugin successor to the v1.0.x disk patcher.

## What changed in 1.1

- No longer modifies `GameAssembly.dll` on disk.
- Runs as a BepInEx 6 IL2CPP plugin.
- Requires **Mimic Party Modding Core v1.0.0+**.
- Uses structural runtime signatures instead of fixed file offsets for the remaining
  native capacity constants.
- Uses runtime method hooks for the main MaxPlayers getter, round state, voice behavior
  and rematch logic.
- Runtime patches are validated transactionally and rolled back on failure.
- Mimic Party game updates that only move code may no longer require a new mod release.

## Features

- Up to 10 players in private lobbies.
- Players 6-10 can pass the host capacity gate.
- Room/listing maximums follow the configured player count.
- Live voice closes during the active performance/take and returns afterwards.
- Connected players stay together when Rematch starts.
- Real disconnect behavior remains stock.

## Requirements

1. Mimic Party on Windows / Steam.
2. BepInEx 6 Unity IL2CPP x64.
3. Mimic Party Modding Core v1.0.0 or newer.

## Installation

If you previously used 10 Player Expansion v1.0.x, restore the original game file first
with that version's uninstaller and verify Mimic Party through Steam. Do not run the
runtime plugin on top of the old on-disk patch.

Place `MimicParty10PlayerExpansion.dll` in:

`Mimic Party/BepInEx/plugins/`

The Core must also be installed in `BepInEx/plugins/`.

## Configuration

After the first run:

`BepInEx/config/com.arribbaa.mimicparty.10playerexpansion.cfg`

`MaxPlayers` supports 6-10. Default: 10.

## Author

arribbaa
