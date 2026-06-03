param([string]$BaseUrl = "http://localhost:5000")

Write-Host "Seeding demo data..." -ForegroundColor Green

# Register users
$user1 = Invoke-RestMethod -Uri "$BaseUrl/api/v1/users" -Method POST -Body '{"username":"alice"}' -ContentType "application/json"
$user2 = Invoke-RestMethod -Uri "$BaseUrl/api/v1/users" -Method POST -Body '{"username":"bob"}' -ContentType "application/json"
Write-Host "Created users: alice (ID: $($user1.id)), bob (ID: $($user2.id))"

# Get auth token
$token = (Invoke-RestMethod -Uri "$BaseUrl/api/auth/token" -Method POST -Body '{"username":"alice"}' -ContentType "application/json").token
$headers = @{ Authorization = "Bearer $token" }

# Create tasks
Invoke-RestMethod -Uri "$BaseUrl/api/v1/tasks" -Method POST -Body "{`"title`":`"Set up CI/CD pipeline`",`"userId`":$($user1.id)}" -ContentType "application/json" -Headers $headers
Invoke-RestMethod -Uri "$BaseUrl/api/v1/tasks" -Method POST -Body "{`"title`":`"Write documentation`",`"userId`":$($user1.id)}" -ContentType "application/json" -Headers $headers
Invoke-RestMethod -Uri "$BaseUrl/api/v1/tasks" -Method POST -Body "{`"title`":`"Review pull requests`",`"userId`":$($user2.id)}" -ContentType "application/json" -Headers $headers

Write-Host "Demo data seeded successfully!" -ForegroundColor Green
