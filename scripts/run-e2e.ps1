# Run Playwright E2E tests locally
# Prerequisites: .NET 8 SDK, Node.js 20+, npm

param(
    [switch]$Install,
    [switch]$Headed
)

$ErrorActionPreference = "Stop"
$rootDir = Split-Path -Parent $PSScriptRoot
$frontendDir = Join-Path $rootDir "frontend"

Write-Host "=== E2E Test Runner ===" -ForegroundColor Cyan

# Install dependencies if requested
if ($Install) {
    Write-Host "`nInstalling frontend dependencies..." -ForegroundColor Yellow
    Push-Location $frontendDir
    npm ci
    npx playwright install chromium --with-deps
    Pop-Location
}

# Build the .NET API
Write-Host "`nBuilding .NET API..." -ForegroundColor Yellow
Push-Location $rootDir
dotnet build --quiet
Pop-Location

# Start the API server in the background
Write-Host "`nStarting API server on http://localhost:5000..." -ForegroundColor Yellow
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--no-build", "--urls", "http://localhost:5000" -WorkingDirectory $rootDir -PassThru -NoNewWindow

# Start the Vite dev server in the background
Write-Host "Starting Vite dev server on http://localhost:5173..." -ForegroundColor Yellow
$viteProcess = Start-Process -FilePath "npm" -ArgumentList "run", "dev" -WorkingDirectory $frontendDir -PassThru -NoNewWindow

# Wait for servers to be ready
Write-Host "`nWaiting for servers..." -ForegroundColor Yellow

$maxRetries = 30
for ($i = 1; $i -le $maxRetries; $i++) {
    try {
        $null = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 2
        Write-Host "  API server is ready" -ForegroundColor Green
        break
    } catch {
        if ($i -eq $maxRetries) {
            Write-Host "  API server failed to start" -ForegroundColor Red
            Stop-Process -Id $apiProcess.Id -ErrorAction SilentlyContinue
            Stop-Process -Id $viteProcess.Id -ErrorAction SilentlyContinue
            exit 1
        }
        Start-Sleep -Seconds 2
    }
}

for ($i = 1; $i -le $maxRetries; $i++) {
    try {
        $null = Invoke-WebRequest -Uri "http://localhost:5173" -UseBasicParsing -TimeoutSec 2
        Write-Host "  Vite dev server is ready" -ForegroundColor Green
        break
    } catch {
        if ($i -eq $maxRetries) {
            Write-Host "  Vite dev server failed to start" -ForegroundColor Red
            Stop-Process -Id $apiProcess.Id -ErrorAction SilentlyContinue
            Stop-Process -Id $viteProcess.Id -ErrorAction SilentlyContinue
            exit 1
        }
        Start-Sleep -Seconds 2
    }
}

# Run Playwright tests
Write-Host "`nRunning Playwright E2E tests..." -ForegroundColor Cyan
Push-Location $frontendDir

$playwrightArgs = "playwright", "test"
if ($Headed) {
    $playwrightArgs += "--headed"
}

try {
    & npx @playwrightArgs
    $testExitCode = $LASTEXITCODE
} finally {
    Pop-Location
    # Clean up background processes
    Write-Host "`nStopping servers..." -ForegroundColor Yellow
    Stop-Process -Id $apiProcess.Id -ErrorAction SilentlyContinue
    Stop-Process -Id $viteProcess.Id -ErrorAction SilentlyContinue
}

if ($testExitCode -eq 0) {
    Write-Host "`n=== All E2E tests passed! ===" -ForegroundColor Green
} else {
    Write-Host "`n=== E2E tests failed (exit code: $testExitCode) ===" -ForegroundColor Red
    Write-Host "View the report: npx playwright show-report (from frontend/)" -ForegroundColor Yellow
}

exit $testExitCode
