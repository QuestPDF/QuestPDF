param(
    [Parameter(Mandatory = $true)]
    [string] $PackageVersion,

    [ValidateSet('net472', 'net481')]
    [string] $TargetFramework = 'net472',

    [ValidateSet('x64', 'x86', 'AnyCPU')]
    [string] $PlatformTarget = 'AnyCPU'
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path "$PSScriptRoot/../../.."
$integrationRoot = Join-Path $repoRoot "Tests/IntegrationTests"
$project = Join-Path $integrationRoot "QuestPDF.Tests.NetFramework/QuestPDF.Tests.NetFramework.csproj"
$outputDirectory = Join-Path $repoRoot "artifacts/integration/netframework/$TargetFramework/$PlatformTarget/output"
$nugetConfig = Join-Path $integrationRoot "nuget.config"
$pdfFileName = "questpdf-integration-$TargetFramework-$PlatformTarget.pdf"

Remove-Item -Recurse -Force (Join-Path $repoRoot "artifacts/integration/netframework") -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $outputDirectory | Out-Null

dotnet restore $project --configfile $nugetConfig "-p:QuestPDFIntegrationVersion=$PackageVersion" "-p:QuestPDFIntegrationTargetFramework=$TargetFramework" "-p:QuestPDFNetFrameworkTargetFramework=$TargetFramework" "-p:PlatformTarget=$PlatformTarget"
dotnet build $project --configuration Release --framework $TargetFramework --no-restore "-p:QuestPDFIntegrationVersion=$PackageVersion" "-p:QuestPDFIntegrationTargetFramework=$TargetFramework" "-p:QuestPDFNetFrameworkTargetFramework=$TargetFramework" "-p:PlatformTarget=$PlatformTarget"

$exe = Join-Path $integrationRoot "QuestPDF.Tests.NetFramework/bin/Release/$TargetFramework/QuestPDF.Tests.NetFramework.exe"
& $exe $outputDirectory $pdfFileName $PlatformTarget

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$pdf = Join-Path $outputDirectory $pdfFileName

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
