# FuzzySharp Docker Benchmark Runner (PowerShell)
# This script builds and runs the FuzzySharp benchmarks in Docker on Windows

param(
    [switch]$BuildOnly,
    [switch]$NoCache,
    [switch]$Clean,
    [switch]$Help
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ProjectRoot = Split-Path -Parent $ScriptDir

function Show-Usage {
    Write-Host ""
    Write-Host "Usage: .\run-benchmark.ps1 [OPTIONS]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -BuildOnly           Only build the Docker image, don't run"
    Write-Host "  -NoCache             Build Docker image without cache"
    Write-Host "  -Clean               Clean up containers and images before running"
    Write-Host "  -Help                Show this help message"
    Write-Host ""
    Write-Host "Performance Configuration:"
    Write-Host "  CPU: 8 cores"
    Write-Host "  Memory: 8GB limit, 4GB reservation"
    Write-Host "  GC: Server GC with non-concurrent collection"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\run-benchmark.ps1              # Run benchmark"
    Write-Host "  .\run-benchmark.ps1 -Clean       # Clean rebuild and run"
    Write-Host "  .\run-benchmark.ps1 -BuildOnly   # Build only"
    Write-Host ""
}

function Test-DockerAvailability {
    Write-Host "🔍 Checking Docker availability..." -ForegroundColor Blue
    
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
        Write-Host ""
        Write-Host "Please install Docker Desktop:" -ForegroundColor Yellow
        Write-Host "  https://www.docker.com/products/docker-desktop/" -ForegroundColor Cyan
        Write-Host ""
        exit 1
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
        Write-Host ""
        Write-Host "Please start Docker Desktop or Docker daemon:" -ForegroundColor Yellow
        Write-Host "  - Open Docker Desktop application" -ForegroundColor Cyan
        Write-Host "  - Or start Docker service: 'Start-Service docker'" -ForegroundColor Cyan
        Write-Host ""
        exit 1
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
        Write-Host ""
        Write-Host "Docker Compose should be included with Docker Desktop." -ForegroundColor Yellow
        Write-Host "If using Docker Engine, install Docker Compose separately." -ForegroundColor Yellow
        Write-Host ""
        exit 1
    }
    
    Write-Host ""
}

if ($Help) {
    Show-Usage
    exit 0
}

Write-Host "=== FuzzySharp Docker Benchmark Runner ===" -ForegroundColor Cyan
Write-Host "Script directory: $ScriptDir"
Write-Host "Project root: $ProjectRoot"
Write-Host ""

# Check Docker availability first
Test-DockerAvailability

Write-Host "Configuration:" -ForegroundColor Green
Write-Host "  Performance Profile: High-performance (8 CPU cores, 8GB RAM)"
Write-Host "  Build only: $BuildOnly"
Write-Host "  No cache: $NoCache"
Write-Host "  Clean: $Clean"
Write-Host ""

# Make sure we're in the correct directory (the script directory)
if ((Get-Location).Path -ne $ScriptDir) {
    Write-Host "📁 Changing to script directory: $ScriptDir" -ForegroundColor Blue
    Set-Location $ScriptDir
}

# Clean up if requested
if ($Clean) {
    Write-Host "🧹 Cleaning up existing containers and images..." -ForegroundColor Yellow
    try {
        docker compose down --remove-orphans 2>$null
        docker system prune -f 2>$null
        Write-Host "✅ Cleanup completed" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️  Cleanup encountered some issues, continuing..." -ForegroundColor Yellow
    }
    Write-Host ""
}

# Create results directory
Write-Host "📁 Creating results directory..." -ForegroundColor Blue
New-Item -ItemType Directory -Force -Path "results" | Out-Null
Write-Host "✅ Results directory created" -ForegroundColor Green
Write-Host ""

# Build the Docker image
Write-Host "🔨 Building Docker image..." -ForegroundColor Blue
$buildArgs = @()
if ($NoCache) {
    $buildArgs += "--no-cache"
}

try {
    docker compose build @buildArgs fuzzysharp-benchmark
    if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }
    Write-Host "✅ Docker image built successfully" -ForegroundColor Green
}
catch {
    Write-Host "❌ Failed to build Docker image: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "  - Check Docker logs: docker logs <container_name>" -ForegroundColor Cyan
    Write-Host "  - Try cleaning up: .\run-benchmark.ps1 -Clean" -ForegroundColor Cyan
    Write-Host "  - Check available disk space" -ForegroundColor Cyan
    exit 1
}
Write-Host ""

# Exit if build-only
if ($BuildOnly) {
    Write-Host "🎯 Build completed. Exiting as requested." -ForegroundColor Green
    exit 0
}

# Run the benchmark
Write-Host "🚀 Starting benchmark..." -ForegroundColor Cyan
Write-Host "⏱️  This may take several minutes to complete..." -ForegroundColor Yellow
Write-Host ""

$StartTime = Get-Date

try {
    docker compose up fuzzysharp-benchmark
    if ($LASTEXITCODE -ne 0) { throw "Benchmark run failed with exit code $LASTEXITCODE" }
}
catch {
    Write-Host "❌ Benchmark failed: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "  - Check container logs: docker compose logs" -ForegroundColor Cyan
    Write-Host "  - Try rebuilding: .\run-benchmark.ps1 -Clean" -ForegroundColor Cyan
    Write-Host "  - Check available resources (CPU/Memory)" -ForegroundColor Cyan
    exit 1
}

$EndTime = Get-Date
$Duration = [math]::Round(($EndTime - $StartTime).TotalSeconds)

Write-Host ""
Write-Host "🎉 Benchmark completed!" -ForegroundColor Green
Write-Host "⏱️  Total time: $Duration seconds" -ForegroundColor Cyan
Write-Host "📊 Results are available in the results/ directory" -ForegroundColor Blue
Write-Host ""

Write-Host "📈 Results location: $ScriptDir\results\" -ForegroundColor Blue
if (Test-Path "results\results") {
    Write-Host "📄 Available files:" -ForegroundColor Blue
    Get-ChildItem "results\results" | Select-Object -First 10 | Format-Table Name, Length, LastWriteTime
}

Write-Host ""
Write-Host "✅ Docker benchmark run completed successfully!" -ForegroundColor Green