$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK not found. Install .NET 8 SDK (or newer), then run this script again."
}

Push-Location $Root
try {
    dotnet restore .\src\MimicParty.ModdingCore\MimicParty.ModdingCore.csproj
    dotnet restore .\src\MimicParty.TenPlayerExpansion\MimicParty.TenPlayerExpansion.csproj

    dotnet build .\src\MimicParty.ModdingCore\MimicParty.ModdingCore.csproj -c Release --no-restore
    dotnet build .\src\MimicParty.TenPlayerExpansion\MimicParty.TenPlayerExpansion.csproj -c Release --no-restore

    & "$Root\package.ps1"
}
finally {
    Pop-Location
}
