$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK not found. Install .NET 8 SDK (or newer), then run this script again."
}

$Project = Join-Path $Root "src\MimicParty.ModdingCore\MimicParty.ModdingCore.csproj"

Push-Location $Root
try {
    dotnet restore $Project
    dotnet build $Project -c Release --no-restore
    & "$Root\package.ps1"
}
finally {
    Pop-Location
}
