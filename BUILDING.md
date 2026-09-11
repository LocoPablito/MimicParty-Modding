# Building and release provenance

Use the .NET 8 SDK. Core targets net6.0 to match the embedded BepInEx runtime.

```powershell
dotnet build src/MimicParty.ModdingCore -c Release
dotnet build src/MimicParty.InteropBootstrap -c Release
dotnet run --project tests/InteropBootstrap.Tests -c Release -f net8.0
```

The public release keeps the exact Core binary from the verified CI artifact: source commit `46c03672bab67ed32a060198ec8f2d56d3fdfd6a`, workflow run `34606013538`. A fresh build from a later documentation commit can produce different assembly metadata and hashes even when runtime source is unchanged. Do not describe an unverified rebuild as byte-identical.

The bootstrap is built separately and its hash/source revision are recorded in the public package provenance. BepInEx and .NET build dependencies are retrieved from their declared package feeds during CI, not by the installed Core/Bootstrap at runtime.

Developer regression fixtures remain public for inspection. Private game captures, generated assemblies and diagnostic result archives do not belong in this repository or in release assets.
