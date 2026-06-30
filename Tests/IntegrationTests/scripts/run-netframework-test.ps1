param(
    [Parameter(Mandatory = $true)]
    [string] $PackageVersion
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path "$PSScriptRoot/../../.."
$integrationRoot = Join-Path $repoRoot "Tests/IntegrationTests"
$project = Join-Path $integrationRoot "QuestPDF.Tests.NetFramework/QuestPDF.Tests.NetFramework.csproj"
$outputDirectory = Join-Path $repoRoot "artifacts/integration/netframework/net472/output"
$nugetConfig = Join-Path $integrationRoot "nuget.config"

Remove-Item -Recurse -Force (Join-Path $repoRoot "artifacts/integration/netframework") -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $outputDirectory | Out-Null

dotnet restore $project --configfile $nugetConfig "-p:QuestPDFIntegrationVersion=$PackageVersion"
dotnet build $project --configuration Release --framework net472 --no-restore "-p:QuestPDFIntegrationVersion=$PackageVersion"

$exe = Join-Path $integrationRoot "QuestPDF.Tests.NetFramework/bin/Release/net472/QuestPDF.Tests.NetFramework.exe"
& $exe $outputDirectory

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$pdf = Join-Path $outputDirectory "questpdf-integration-net472.pdf"

if (-not (Test-Path $pdf)) {
    throw "PDF does not exist: $pdf"
}

$bytes = [System.IO.File]::ReadAllBytes($pdf)
$header = [System.Text.Encoding]::ASCII.GetString($bytes, 0, 5)
$tailStart = [Math]::Max(0, $bytes.Length - 2048)
$tail = [System.Text.Encoding]::ASCII.GetString($bytes, $tailStart, $bytes.Length - $tailStart)

if ($bytes.Length -lt 1024) {
    throw "PDF is too small: $pdf ($($bytes.Length) bytes)"
}

if ($header -ne "%PDF-") {
    throw "PDF header is invalid: $pdf"
}

if (-not $tail.Contains("%%EOF")) {
    throw "PDF EOF marker was not found: $pdf"
}

Write-Host "Validated $pdf ($($bytes.Length) bytes)"
