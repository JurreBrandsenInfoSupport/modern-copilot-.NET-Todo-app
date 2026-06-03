# Start the TodoApp locally
Write-Host "Starting TodoApp..." -ForegroundColor Green
Write-Host "Swagger UI will be available at: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "Health check: http://localhost:5000/health" -ForegroundColor Cyan
dotnet run --project $PSScriptRoot/../TodoApp.csproj --urls "http://localhost:5000"
