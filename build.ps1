$ErrorActionPreference = 'Stop'
dotnet build (Join-Path $PSScriptRoot 'src/MimicParty.ModdingCore') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Core build failed' }
