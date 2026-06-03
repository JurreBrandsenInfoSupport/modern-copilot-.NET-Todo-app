Write-Host "Running BDD tests..." -ForegroundColor Green
dotnet test $PSScriptRoot/../tests/Application.Tests/Application.Tests.csproj --verbosity normal --collect:"XPlat Code Coverage"
Write-Host "Tests complete!" -ForegroundColor Green
