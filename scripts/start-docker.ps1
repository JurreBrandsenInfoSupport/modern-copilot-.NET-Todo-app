# Start all services with Docker Compose
Write-Host "Starting TodoApp with Docker Compose..." -ForegroundColor Green
docker-compose -f $PSScriptRoot/../docker-compose.yml up --build -d
Write-Host "Services started!" -ForegroundColor Green
Write-Host "API: http://localhost:8080/swagger" -ForegroundColor Cyan
Write-Host "Seq (logs): http://localhost:8081" -ForegroundColor Cyan
Write-Host "PostgreSQL: localhost:5432" -ForegroundColor Cyan
