$ErrorActionPreference = "Stop"

Write-Host "Restoring dependencies..."
dotnet restore ATMSystem.slnx
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "Building Debug..."
dotnet build ATMSystem.slnx --configuration Debug --no-restore
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "Building Release..."
dotnet build ATMSystem.slnx --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "Running tests with coverage..."
dotnet test ATMSystem.slnx --no-restore --collect:"XPlat Code Coverage"
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "Build completed successfully."