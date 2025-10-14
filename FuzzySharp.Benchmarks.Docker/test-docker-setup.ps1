# Test Docker Setup (PowerShell)
# This script verifies the Docker benchmark setup without running the full benchmark

param(
    [switch]$Help
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

function Show-Usage {
    Write-Host ""
    Write-Host "Usage: .\test-docker-setup.ps1 [OPTIONS]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Help                Show this help message"
    Write-Host ""
    Write-Host "This script tests:"
    Write-Host "  - Docker availability and daemon status"
    Write-Host "  - Docker Compose functionality"
    Write-Host "  - Project file compilation"
    Write-Host "  - Docker build preparation"
    Write-Host ""
}

function Test-DockerAvailability {
    Write-Host "🔍 Testing Docker availability..." -ForegroundColor Blue
    
    # Check if Docker command exists
    try {
        $dockerVersion = docker --version 2>$null
        if (-not $dockerVersion) {
            throw "Docker command not found"
        }
        Write-Host "✅ Docker found: $dockerVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Docker is not installed or not in PATH" -ForegroundColor Red
        return $false
    }
    
    # Check if Docker daemon is running
    try {
        $dockerInfo = docker info --format "{{.ServerVersion}}" 2>$null
        if (-not $dockerInfo) {
            throw "Docker daemon not responding"
        }
        Write-Host "✅ Docker daemon is running (Server version: $dockerInfo)" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Docker daemon is not running" -ForegroundColor Red
        return $false
    }
    
    # Check if Docker Compose is available
    try {
        $composeVersion = docker compose version 2>$null
        if (-not $composeVersion) {
            # Try docker-compose (legacy)
            $composeVersion = docker-compose --version 2>$null
            if (-not $composeVersion) {
                throw "Docker Compose not found"
            }
        }
        Write-Host "✅ Docker Compose available: $composeVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Docker Compose is not available" -ForegroundColor Red
        return $false
    }
    
    return $true
}

function Test-ProjectCompilation {
    Write-Host "🔍 Testing project compilation..." -ForegroundColor Blue
    
    try {
        $buildOutput = dotnet build "$ScriptDir\FuzzySharp.Benchmarks.Docker.csproj" --verbosity quiet 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed: $buildOutput"
        }
        Write-Host "✅ Project compiles successfully" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ Project compilation failed: $_" -ForegroundColor Red
        return $false
    }
}

function Test-DockerfileValidation {
    Write-Host "🔍 Testing Dockerfile validation..." -ForegroundColor Blue
    
    if (-not (Test-Path "$ScriptDir\Dockerfile")) {
        Write-Host "❌ Dockerfile not found" -ForegroundColor Red
        return $false
    }
    
    if (-not (Test-Path "$ScriptDir\docker-compose.yml")) {
        Write-Host "❌ docker-compose.yml not found" -ForegroundColor Red
        return $false
    }
    
    Write-Host "✅ Docker configuration files found" -ForegroundColor Green
    return $true
}

if ($Help) {
    Show-Usage
    exit 0
}

Write-Host "=== FuzzySharp Docker Setup Test ===" -ForegroundColor Cyan
Write-Host "Script directory: $ScriptDir"
Write-Host ""

Set-Location $ScriptDir

$testResults = @{
    Docker = Test-DockerAvailability
    Compilation = Test-ProjectCompilation
    Configuration = Test-DockerfileValidation
}

Write-Host ""
Write-Host "=== Test Results Summary ===" -ForegroundColor Cyan

$allPassed = $true
foreach ($test in $testResults.GetEnumerator()) {
    $status = if ($test.Value) { "✅ PASS" } else { "❌ FAIL"; $allPassed = $false }
    Write-Host "  $($test.Key): $status" -ForegroundColor $(if ($test.Value) { "Green" } else { "Red" })
}

Write-Host ""
if ($allPassed) {
    Write-Host "🎉 All tests passed! Your Docker benchmark setup is ready." -ForegroundColor Green
    Write-Host ""
    Write-Host "Performance Configuration:" -ForegroundColor Yellow
    Write-Host "  - CPU: 8 cores" -ForegroundColor Cyan
    Write-Host "  - Memory: 8GB limit, 4GB reservation" -ForegroundColor Cyan
    Write-Host "  - GC: Server GC with non-concurrent collection" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Run the benchmark: .\run-benchmark.ps1" -ForegroundColor Cyan
    Write-Host "  2. Build only: .\run-benchmark.ps1 -BuildOnly" -ForegroundColor Cyan
    Write-Host "  3. View help: .\run-benchmark.ps1 -Help" -ForegroundColor Cyan
} else {
    Write-Host "❌ Some tests failed. Please resolve the issues above." -ForegroundColor Red
    Write-Host ""
    Write-Host "Common solutions:" -ForegroundColor Yellow
    Write-Host "  - Start Docker Desktop" -ForegroundColor Cyan
    Write-Host "  - Install Docker: https://www.docker.com/products/docker-desktop/" -ForegroundColor Cyan
    Write-Host "  - Check .NET SDK installation: dotnet --version" -ForegroundColor Cyan
}

Write-Host ""