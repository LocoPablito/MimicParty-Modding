$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$Dist = Join-Path $Root "dist"
$CoreDll = Join-Path $Root "src\MimicParty.ModdingCore\bin\Release\net6.0\MimicPartyModdingCore.dll"

if (-not (Test-Path -LiteralPath $CoreDll)) {
    throw "Core DLL was not built: $CoreDll"
}

Remove-Item $Dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item $Dist -ItemType Directory | Out-Null

$Stage = Join-Path $Dist "stage-core"
New-Item (Join-Path $Stage "BepInEx\plugins") -ItemType Directory -Force | Out-Null

Copy-Item $CoreDll (Join-Path $Stage "BepInEx\plugins\MimicPartyModdingCore.dll")
Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\README.md") (Join-Path $Stage "README.txt")
Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\CHANGELOG.md") (Join-Path $Stage "CHANGELOG.txt")
Copy-Item (Join-Path $Root "src\MimicParty.ModdingCore\LICENSE.txt") (Join-Path $Stage "LICENSE.txt")

$Zip = Join-Path $Dist "MimicParty_Modding_Core_v1.0.0_by_arribbaa_NEXUS.zip"
Compress-Archive -Path (Join-Path $Stage "*") -DestinationPath $Zip

$Hash = (Get-FileHash -LiteralPath $Zip -Algorithm SHA256).Hash.ToLowerInvariant()
"$Hash  $(Split-Path -Leaf $Zip)" | Set-Content -LiteralPath (Join-Path $Dist "SHA256SUMS.txt") -Encoding UTF8

Remove-Item $Stage -Recurse -Force

Write-Host "Core release package created:" -ForegroundColor Green
Write-Host $Zip
