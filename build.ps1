$ErrorActionPreference = 'Stop'
$Root = $PSScriptRoot
foreach ($Project in @('MimicParty.ModdingCore', 'MimicParty.InteropBootstrap')) {
    dotnet build (Join-Path $Root "src/$Project") -c Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $Project" }
}
Write-Host 'Source builds completed. Public release artifacts preserve their separately recorded verified binaries.'
