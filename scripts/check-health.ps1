param([string]$BaseUrl = "http://localhost:5000")
$response = Invoke-RestMethod -Uri "$BaseUrl/health" -Method GET
Write-Host "Health Status: $($response.status)" -ForegroundColor $(if($response.status -eq "Healthy"){"Green"}else{"Red"})
$response.checks | ForEach-Object { Write-Host "  - $($_.name): $($_.status)" }
