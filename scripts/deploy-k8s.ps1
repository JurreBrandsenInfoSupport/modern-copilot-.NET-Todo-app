# Deploy TodoApp to Kubernetes using Helm
# Usage: .\scripts\deploy-k8s.ps1 [-Namespace todoapp] [-ReleaseName todoapp] [-ValuesFile ""]

param(
    [string]$Namespace = "todoapp",
    [string]$ReleaseName = "todoapp",
    [string]$ValuesFile = ""
)

$ErrorActionPreference = "Stop"

Write-Host "Deploying TodoApp to Kubernetes..." -ForegroundColor Cyan
Write-Host "  Namespace:    $Namespace" -ForegroundColor Gray
Write-Host "  Release:      $ReleaseName" -ForegroundColor Gray

# Check prerequisites
$helm = Get-Command helm -ErrorAction SilentlyContinue
if (-not $helm) {
    Write-Error "Helm is not installed. Please install Helm: https://helm.sh/docs/intro/install/"
    exit 1
}

$kubectl = Get-Command kubectl -ErrorAction SilentlyContinue
if (-not $kubectl) {
    Write-Error "kubectl is not installed. Please install kubectl: https://kubernetes.io/docs/tasks/tools/"
    exit 1
}

# Verify cluster connectivity
Write-Host "`nVerifying cluster connectivity..." -ForegroundColor Yellow
kubectl cluster-info | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Cannot connect to Kubernetes cluster. Please check your kubeconfig."
    exit 1
}
Write-Host "  Cluster connection verified." -ForegroundColor Green

# Create namespace if it doesn't exist
$nsExists = kubectl get namespace $Namespace 2>$null
if (-not $nsExists) {
    Write-Host "`nCreating namespace '$Namespace'..." -ForegroundColor Yellow
    kubectl create namespace $Namespace
}

# Build Helm install command
$chartPath = Join-Path $PSScriptRoot "..\charts\todoapp"
$helmArgs = @(
    "upgrade", "--install",
    $ReleaseName,
    $chartPath,
    "--namespace", $Namespace,
    "--create-namespace",
    "--wait",
    "--timeout", "5m"
)

if ($ValuesFile -and (Test-Path $ValuesFile)) {
    $helmArgs += @("--values", $ValuesFile)
    Write-Host "  Values file:  $ValuesFile" -ForegroundColor Gray
}

# Deploy
Write-Host "`nRunning Helm install/upgrade..." -ForegroundColor Yellow
& helm @helmArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nDeployment successful!" -ForegroundColor Green
    Write-Host "`nPod status:" -ForegroundColor Cyan
    kubectl get pods -n $Namespace
    Write-Host "`nServices:" -ForegroundColor Cyan
    kubectl get svc -n $Namespace
} else {
    Write-Error "Helm deployment failed. Check the logs above for details."
    exit 1
}
