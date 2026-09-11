$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$Dist = Join-Path $Root "dist"

Remove-Item $Dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item $Dist -ItemType Directory | Out-Null

$CoreDll = Join-Path $Root "src\MimicParty.ModdingCore\bin\Release\net6.0\MimicPartyModdingCore.dll"
$ExpansionDll = Join-Path $Root "src\MimicParty.TenPlayerExpansion\bin\Release\net6.0\MimicParty10PlayerExpansion.dll"

if (-not (Test-Path $CoreDll)) { throw "Core DLL was not built: $CoreDll" }
if (-not (Test-Path $ExpansionDll)) { throw "Expansion DLL was not built: $ExpansionDll" }

$CoreStage = Join-Path $Dist "MimicParty_Modding_Core_v1.0.0_by_arribbaa"
$ExpansionStage = Join-Path $Dist "MimicParty_10_Player_Expansion_v1.1.0_by_arribbaa"

New-Item (Join-Path $CoreStage "BepInEx\plugins") -ItemType Directory -Force | Out-Null
New-Item (Join-Path $ExpansionStage "BepInEx\plugins") -ItemType Directory -Force | Out-Null

Copy-Item $CoreDll (Join-Path $CoreStage "BepInEx\plugins\MimicPartyModdingCore.dll")
Copy-Item $ExpansionDll (Join-Path $ExpansionStage "BepInEx\plugins\MimicParty10PlayerExpansion.dll")

Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\README.md") (Join-Path $CoreStage "README.txt")
Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\CHANGELOG.md") (Join-Path $CoreStage "CHANGELOG.txt")
Copy-Item (Join-Path $Root "LICENSE.txt") (Join-Path $CoreStage "LICENSE.txt")
Copy-Item (Join-Path $Root "src\MimicParty.TenPlayerExpansion\README.md") (Join-Path $ExpansionStage "README.txt")
Copy-Item (Join-Path $Root "src\MimicParty.TenPlayerExpansion\CHANGELOG.md") (Join-Path $ExpansionStage "CHANGELOG.txt")
Copy-Item (Join-Path $Root "LICENSE.txt") (Join-Path $ExpansionStage "LICENSE.txt")

Compress-Archive -Path $CoreStage -DestinationPath (Join-Path $Dist "MimicParty_Modding_Core_v1.0.0_by_arribbaa_NEXUS.zip")
Compress-Archive -Path $ExpansionStage -DestinationPath (Join-Path $Dist "MimicParty_10_Player_Expansion_v1.1.0_by_arribbaa_NEXUS.zip")

Write-Host ""
Write-Host "Release packages created in:" -ForegroundColor Green
Write-Host $Dist
