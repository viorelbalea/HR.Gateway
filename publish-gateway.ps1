# ========================================
# HR.Gateway - Script Publish pentru IIS
# ========================================

param(
    [string]$OutputPath = ".\publish\gateway",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  HR.Gateway - Publish pentru IIS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Verificare proiect
Write-Host "[1/5] Verificare proiect..." -ForegroundColor Yellow
$projectPath = ".\HR.Gateway.Api\HR.Gateway.Api.csproj"
if (-not (Test-Path $projectPath)) {
    Write-Host "ERROR: Nu gasesc proiectul la: $projectPath" -ForegroundColor Red
    exit 1
}
Write-Host "Proiect gasit" -ForegroundColor Green

# 2. Curatare folder publish
Write-Host ""
Write-Host "[2/5] Curatare folder publish..." -ForegroundColor Yellow
if (Test-Path $OutputPath) {
    Write-Host "  Stergere folder existent: $OutputPath" -ForegroundColor Gray
    Remove-Item -Path $OutputPath -Recurse -Force
}
Write-Host "Folder pregatit" -ForegroundColor Green

# 3. Publish aplicatie
Write-Host ""
Write-Host "[3/5] Publish aplicatie (.NET 9)..." -ForegroundColor Yellow
Write-Host "  Configuration: $Configuration" -ForegroundColor Gray
Write-Host "  Runtime: win-x64" -ForegroundColor Gray
Write-Host "  Self-contained: false" -ForegroundColor Gray
Write-Host ""

dotnet publish $projectPath -c $Configuration -r win-x64 --self-contained false -o $OutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Publish failed!" -ForegroundColor Red
    exit 1
}
Write-Host "Publish reusit" -ForegroundColor Green

# 4. Verificare fisiere importante
Write-Host ""
Write-Host "[4/5] Verificare fisiere..." -ForegroundColor Yellow

$requiredFiles = @(
    "HR.Gateway.Api.dll",
    "HR.Gateway.Api.exe",
    "appsettings.json",
    "appsettings.Production.json",
    "web.config"
)

$missingFiles = @()
foreach ($file in $requiredFiles) {
    $filePath = Join-Path $OutputPath $file
    if (Test-Path $filePath) {
        Write-Host "  OK: $file" -ForegroundColor Green
    } else {
        Write-Host "  MISSING: $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host ""
    Write-Host "WARNING: Lipsesc fisiere importante!" -ForegroundColor Yellow
    Write-Host "Fisiere lipsa: $($missingFiles -join ', ')" -ForegroundColor Yellow
}

# 5. Statistici
Write-Host ""
Write-Host "[5/5] Statistici..." -ForegroundColor Yellow

$totalSize = (Get-ChildItem -Path $OutputPath -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$fileCount = (Get-ChildItem -Path $OutputPath -Recurse -File).Count

Write-Host "  Locatie: $OutputPath" -ForegroundColor Cyan
Write-Host "  Fisiere: $fileCount" -ForegroundColor Cyan
Write-Host "  Dimensiune: $([math]::Round($totalSize, 2)) MB" -ForegroundColor Cyan

# 6. SUCCESS
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  PUBLISH REUSIT!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Pasi urmatori:" -ForegroundColor Yellow
Write-Host "  1. Verifica appsettings.Production.json" -ForegroundColor White
Write-Host "  2. Citeste DEPLOYMENT-GUIDE.md pentru instructiuni IIS" -ForegroundColor White
Write-Host "  3. Stop IIS Application Pool" -ForegroundColor White
Write-Host "  4. Copiaza fisierele din $OutputPath pe server" -ForegroundColor White
Write-Host "  5. Start IIS Application Pool" -ForegroundColor White
Write-Host ""
