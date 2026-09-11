$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$Dist = Join-Path $Root "dist"

Remove-Item $Dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item $Dist -ItemType Directory | Out-Null

$CoreDll = Join-Path $Root "src\MimicParty.ModdingCore\bin\Release\net6.0\MimicPartyModdingCore.dll"
$ExpansionDll = Join-Path $Root "src\MimicParty.TenPlayerExpansion\bin\Release\net6.0\MimicParty10PlayerExpansion.dll"

if (-not (Test-Path $CoreDll)) { throw "Core DLL was not built: $CoreDll" }
if (-not (Test-Path $ExpansionDll)) { throw "Expansion DLL was not built: $ExpansionDll" }

$CoreStage = Join-Path $Dist "stage-core"
$ExpansionStage = Join-Path $Dist "stage-expansion"

New-Item (Join-Path $CoreStage "BepInEx\plugins") -ItemType Directory -Force | Out-Null
New-Item (Join-Path $ExpansionStage "BepInEx\plugins") -ItemType Directory -Force | Out-Null

Copy-Item $CoreDll (Join-Path $CoreStage "BepInEx\plugins\MimicPartyModdingCore.dll")
Copy-Item $ExpansionDll (Join-Path $ExpansionStage "BepInEx\plugins\MimicParty10PlayerExpansion.dll")

Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\README.md") (Join-Path $CoreStage "README.txt")
Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\CHANGELOG.md") (Join-Path $CoreStage "CHANGELOG.txt")
Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\LICENSE.txt") (Join-Path $CoreStage "LICENSE.txt")

Copy-Item (Join-Path $Root "src\MimicParty.TenPlayerExpansion\README.md") (Join-Path $ExpansionStage "README.txt")
Copy-Item (Join-Path $Root "src\MimicParty.TenPlayerExpansion\CHANGELOG.md") (Join-Path $ExpansionStage "CHANGELOG.txt")
Copy-Item (Join-Path $Root "src\MimicParty.TenPlayerExpansion\LICENSE.txt") (Join-Path $ExpansionStage "LICENSE.txt")

$CoreZip = Join-Path $Dist "MimicParty_Modding_Core_v1.0.0_by_arribbaa_NEXUS.zip"
$ExpansionZip = Join-Path $Dist "MimicParty_10_Player_Expansion_v1.1.0_by_arribbaa_NEXUS.zip"

# Archive the stage CONTENTS, not the stage directory itself. This makes manual
# extraction into the Mimic Party game root produce BepInEx/plugins directly and
# gives mod managers the expected BepInEx archive layout.
Compress-Archive -Path (Join-Path $CoreStage "*") -DestinationPath $CoreZip
Compress-Archive -Path (Join-Path $ExpansionStage "*") -DestinationPath $ExpansionZip

$CoreHash = (Get-FileHash -LiteralPath $CoreZip -Algorithm SHA256).Hash.ToLowerInvariant()
$ExpansionHash = (Get-FileHash -LiteralPath $ExpansionZip -Algorithm SHA256).Hash.ToLowerInvariant()

@(
    "$CoreHash  $(Split-Path -Leaf $CoreZip)",
    "$ExpansionHash  $(Split-Path -Leaf $ExpansionZip)"
) | Set-Content -LiteralPath (Join-Path $Dist "SHA256SUMS.txt") -Encoding UTF8

Remove-Item $CoreStage -Recurse -Force
Remove-Item $ExpansionStage -Recurse -Force

Write-Host ""
Write-Host "Release packages created in:" -ForegroundColor Green
Write-Host $Dist
