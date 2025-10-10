# Check if PostgreSQL is installed
if (-not (Get-Command psql -ErrorAction SilentlyContinue)) {
    Write-Host "Error: PostgreSQL is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install PostgreSQL and add it to your PATH" -ForegroundColor Yellow
    exit 1
}

# Check if PostgreSQL is running
try {
    psql -h localhost -p 5432 -U postgres -c "SELECT 1;" 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Connection failed"
    }
} catch {
    Write-Host "Error: PostgreSQL is not running or not accessible" -ForegroundColor Red
    Write-Host "Please start PostgreSQL service" -ForegroundColor Yellow
    exit 1
}

# Check if database already exists
$dbExists = psql -U postgres -lqt 2>$null | Select-String "carpark_db"
if ($dbExists) {
    Write-Host "Database 'carpark_db' already exists" -ForegroundColor Yellow
    $recreate = Read-Host "Recreate database? (y/N)"
    if ($recreate -match "^[Yy]$") {
        psql -U postgres -c "DROP DATABASE carpark_db;" 2>$null
        psql -U postgres -c "CREATE DATABASE carpark_db;" 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Database recreated" -ForegroundColor Green
        } else {
            Write-Host "Error: Failed to recreate database" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "Using existing database" -ForegroundColor Green
    }
} else {
    psql -U postgres -c "CREATE DATABASE carpark_db;" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database 'carpark_db' created" -ForegroundColor Green
    } else {
        Write-Host "Error: Failed to create database" -ForegroundColor Red
        exit 1
    }
}

Write-Host "Setup complete" -ForegroundColor Green
