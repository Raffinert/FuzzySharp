#!/bin/bash

# FuzzySharp Docker Benchmark Runner
# This script builds and runs the FuzzySharp benchmarks in Docker

set -e  # Exit on any error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

echo "=== FuzzySharp Docker Benchmark Runner ==="
echo "Script directory: $SCRIPT_DIR"
echo "Project root: $PROJECT_ROOT"
echo

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo
    echo "Options:"
    echo "  --build-only         Only build the Docker image, don't run"
    echo "  --no-cache           Build Docker image without cache"
    echo "  --clean              Clean up containers and images before running"
    echo "  --help               Show this help message"
    echo
    echo "Performance Configuration:"
    echo "  CPU: 8 cores"
    echo "  Memory: 8GB limit, 4GB reservation"
    echo "  GC: Server GC with non-concurrent collection"
    echo
    echo "Examples:"
    echo "  $0                   # Run benchmark"
    echo "  $0 --clean           # Clean rebuild and run"
    echo "  $0 --build-only      # Build only"
}

# Function to check Docker availability
check_docker_availability() {
    echo "🔍 Checking Docker availability..."
    
    # Check if Docker command exists
    if ! command -v docker &> /dev/null; then
        echo "❌ Docker is not installed or not in PATH"
        echo
        echo "Please install Docker:"
        echo "  - Linux: https://docs.docker.com/engine/install/"
        echo "  - macOS: https://docs.docker.com/docker-for-mac/install/"
        echo
        exit 1
    fi
    
    DOCKER_VERSION=$(docker --version 2>/dev/null || echo "")
    if [ -z "$DOCKER_VERSION" ]; then
        echo "❌ Docker command failed"
        exit 1
    fi
    echo "✅ Docker found: $DOCKER_VERSION"
    
    # Check if Docker daemon is running
    if ! docker info &> /dev/null; then
        echo "❌ Docker daemon is not running"
        echo
        echo "Please start Docker daemon:"
        echo "  - Linux: sudo systemctl start docker"
        echo "  - macOS: Open Docker Desktop application"
        echo
        exit 1
    fi
    
    SERVER_VERSION=$(docker info --format "{{.ServerVersion}}" 2>/dev/null || echo "unknown")
    echo "✅ Docker daemon is running (Server version: $SERVER_VERSION)"
    
    # Check if Docker Compose is available
    if command -v "docker compose" &> /dev/null; then
        COMPOSE_VERSION=$(docker compose version 2>/dev/null || echo "")
        echo "✅ Docker Compose available: $COMPOSE_VERSION"
    elif command -v "docker-compose" &> /dev/null; then
        COMPOSE_VERSION=$(docker-compose --version 2>/dev/null || echo "")
        echo "✅ Docker Compose (legacy) available: $COMPOSE_VERSION"
    else
        echo "❌ Docker Compose is not available"
        echo
        echo "Please install Docker Compose:"
        echo "  https://docs.docker.com/compose/install/"
        echo
        exit 1
    fi
    
    echo
}

# Parse command line arguments
BUILD_ONLY=false
NO_CACHE=""
CLEAN=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --build-only)
            BUILD_ONLY=true
            shift
            ;;
        --no-cache)
            NO_CACHE="--no-cache"
            shift
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        --help)
            show_usage
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Check Docker availability first
check_docker_availability

echo "Configuration:"
echo "  Performance Profile: High-performance (8 CPU cores, 8GB RAM)"
echo "  Build only: $BUILD_ONLY"
echo "  No cache: $([ -n "$NO_CACHE" ] && echo "true" || echo "false")"
echo "  Clean: $CLEAN"
echo

# Change to the Docker project directory
cd "$SCRIPT_DIR"

# Clean up if requested
if [ "$CLEAN" = true ]; then
    echo "🧹 Cleaning up existing containers and images..."
    docker compose down --remove-orphans 2>/dev/null || true
    docker system prune -f 2>/dev/null || true
    echo "✅ Cleanup completed"
    echo
fi

# Create results directory
echo "📁 Creating results directory..."
mkdir -p results
echo "✅ Results directory created"
echo

# Build the Docker image
echo "🔨 Building Docker image..."
BUILD_FAILED=false

docker compose build $NO_CACHE fuzzysharp-benchmark || BUILD_FAILED=true

if [ "$BUILD_FAILED" = true ]; then
    echo "❌ Failed to build Docker image"
    echo
    echo "Troubleshooting tips:"
    echo "  - Check Docker logs: docker logs <container_name>"
    echo "  - Try cleaning up: $0 --clean"
    echo "  - Check available disk space: df -h"
    exit 1
fi

echo "✅ Docker image built successfully"
echo

# Exit if build-only
if [ "$BUILD_ONLY" = true ]; then
    echo "🎯 Build completed. Exiting as requested."
    exit 0
fi

# Run the benchmark
echo "🚀 Starting benchmark..."
echo "⏱️  This may take several minutes to complete..."
echo

START_TIME=$(date +%s)
RUN_FAILED=false

docker compose up fuzzysharp-benchmark || RUN_FAILED=true

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))

if [ "$RUN_FAILED" = true ]; then
    echo
    echo "❌ Benchmark failed"
    echo
    echo "Troubleshooting tips:"
    echo "  - Check container logs: docker compose logs"
    echo "  - Try rebuilding: $0 --clean"
    echo "  - Check available resources: free -h && nproc"
    exit 1
fi

echo
echo "🎉 Benchmark completed!"
echo "⏱️  Total time: ${DURATION} seconds"
echo "📊 Results are available in the results/ directory"
echo

echo "📈 Results location: $SCRIPT_DIR/results/"
if [ -d "results/results" ]; then
    echo "📄 Available files:"
    ls -la "results/results/" | head -10
fi

echo
echo "✅ Docker benchmark run completed successfully!"