Write-Host "Restoring dependencies..."
dotnet restore ATMSystem.slnx

Write-Host "Building solution..."
dotnet build ATMSystem.slnx --configuration Release --no-restore

Write-Host "Running tests..."
dotnet test ATMSystem.slnx --configuration Release --no-build --collect:"XPlat Code Coverage"

Write-Host "Build script completed."