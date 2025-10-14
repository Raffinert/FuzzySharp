# FuzzySharp Docker Benchmarks

This project provides containerized benchmarking for the FuzzySharp library, specifically focusing on Levenshtein distance algorithms. The benchmarks run in isolated Docker containers to ensure consistent and reproducible performance measurements across different environments.

## 🎯 Overview

The Docker benchmark project is based on the `LevenshteinLarge` benchmark but optimized for container environments with:

- **InProcessEmitToolchain** for Docker compatibility
- Container-specific JIT pre-warming
- Environment information logging
- **High-performance configuration** (8 CPU cores, 8GB RAM)
- Automated result collection
- Privileged container mode for BenchmarkDotNet

## 🚀 Quick Start

### Prerequisites

- **Docker Desktop** (Windows/macOS) or **Docker Engine** (Linux)
- **Docker Compose** (included with Docker Desktop)
- At least 8GB available RAM
- At least 5GB available disk space

### Starting Docker

Before running benchmarks, ensure Docker is running:

#### Windows
1. Install [Docker Desktop for Windows](https://docs.docker.com/desktop/install/windows/)
2. Start Docker Desktop from the Start menu
3. Wait for the "Docker Desktop is running" notification

#### macOS
1. Install [Docker Desktop for Mac](https://docs.docker.com/desktop/install/mac/)
2. Start Docker Desktop from Applications
3. Wait for the whale icon in the menu bar to be steady

#### Linux
```bash
# Install Docker Engine (Ubuntu/Debian example)
sudo apt-get update
sudo apt-get install docker.io docker-compose

# Start Docker service
sudo systemctl start docker
sudo systemctl enable docker

# Add user to docker group (optional, requires logout/login)
sudo usermod -aG docker $USER
```

### Running Benchmarks

#### Linux/macOS (Bash)
```bash
# Make the script executable
chmod +x run-benchmark.sh

# Run benchmark
./run-benchmark.sh

# Clean rebuild and run
./run-benchmark.sh --clean --no-cache

# Build only
./run-benchmark.sh --build-only
```

#### Windows (PowerShell)
```powershell
# Run benchmark
.\run-benchmark.ps1

# Clean rebuild and run
.\run-benchmark.ps1 -Clean -NoCache

# Build only (for testing)
.\run-benchmark.ps1 -BuildOnly
```

## 📊 Performance Configuration

The Docker benchmark is configured for high performance:

- **CPU**: 8 cores
- **Memory**: 8GB limit, 4GB reservation
- **GC**: Server GC with non-concurrent collection for maximum performance
- **Environment**: Optimized .NET 9 runtime with tiered compilation and PGO

## 📈 Latest Benchmark Results

Here are the latest results from the Docker benchmark (Intel Core i9-9900K, .NET 9.0):

| Method | Mean | Ratio | Allocated | Alloc Ratio | Performance |
|--------|------|-------|-----------|-------------|-------------|
| **NaiveDp** (Baseline) | 3,006 μs | 1.00 | 2.3 MB | 1.000 | ⭐ |
| **NewMatchEngineEditDistance** | 3,210 μs | 1.07 | 49 KB | 0.022 | ⭐⭐ |
| **FuzzySharpClassic** | 2,713 μs | 0.90 | 67 KB | 0.029 | ⭐⭐ |
| **Fastenshtein** | 932 μs | 0.31 | 3.4 KB | 0.001 | ⭐⭐⭐⭐ |
| **Quickenshtein** | 381 μs | 0.13 | 2 B | 0.000 | ⭐⭐⭐⭐⭐ |
| **FuzzySharp** | 402 μs | 0.13 | 12 KB | 0.006 | ⭐⭐⭐⭐⭐ |

**Key Insights:**
- **Quickenshtein** is the fastest with minimal memory usage
- **FuzzySharp** (current implementation) is nearly as fast with excellent memory efficiency
- **Fastenshtein** offers good performance with very low memory allocation
- Memory-optimized algorithms show 22-100x less memory allocation than the baseline

## 🔧 Manual Docker Commands

If you prefer to run Docker commands manually:

```bash
# Build the image
docker build -f Dockerfile -t fuzzysharp-benchmark ..

# Run with performance settings (requires privileged mode)
docker run --privileged --rm --memory=8g --cpus=8 \
  -v $(pwd)/results:/app/BenchmarkDotNet.Artifacts \
  fuzzysharp-benchmark
```

## 📁 Project Structure

```
FuzzySharp.Benchmarks.Docker/
├── Dockerfile                          # Multi-stage Docker build with SDK
├── docker-compose.yml                  # High-performance container config
├── FuzzySharp.Benchmarks.Docker.csproj # Project file
├── Program.cs                           # Entry point with container info
├── run-benchmark.sh                     # Linux/macOS runner script
├── run-benchmark.ps1                    # Windows PowerShell runner script
├── test-docker-setup.ps1               # Setup validation script
├── test-setup.bat                       # Windows batch test runner
├── README.md                            # This file
├── LevenshteinDistance/
│   └── LevenshteinLargeDocker.cs       # Container-optimized benchmark
├── Utils/
│   ├── LevenshteinBaseline.cs          # Baseline algorithms
│   └── RandomWords.cs                   # Test data generation
└── results/                             # Benchmark results (created at runtime)
```

## 🧪 Benchmarked Algorithms

The container benchmarks the following Levenshtein distance implementations:

1. **NaiveDp** (Baseline) - Simple dynamic programming approach
2. **NewMatchEngineEditDistance** - Optimized algorithm with span usage
3. **FuzzySharpClassic** - Original FuzzySharp implementation  
4. **Fastenshtein** - Third-party optimized library
5. **Quickenshtein** - High-performance library
6. **FuzzySharp** - Current FuzzySharp implementation

## 📈 Understanding Results

Benchmark results are saved to the `results/` directory and include:

- **Mean time**: Average execution time per operation
- **Error**: Standard error of the mean
- **StdDev**: Standard deviation
- **Memory allocation**: Gen 0/1/2 collections and allocated bytes
- **Baseline ratio**: Performance relative to the baseline algorithm

### Key Metrics

- **Lower execution time** = Better performance
- **Lower memory allocation** = More efficient
- **Fewer GC collections** = Less overhead

## 🐳 Container Optimizations

The Docker setup includes several optimizations based on [BenchmarkDotNet containerization best practices](https://wojciechnagorski.github.io/2019/12/how-to-run-benchmarkdotnet-in-a-docker-container/):

### Runtime Environment
- .NET 9 SDK (required for BenchmarkDotNet)
- **InProcessEmitToolchain** for container compatibility
- **Privileged mode** for proper benchmark execution
- Linux x64 target platform
- High-performance GC configuration
- JIT pre-warming for consistent measurements

### Security
- Non-root user execution (with privileged container access)
- Minimal attack surface
- Read-only filesystem where possible

### Resource Management
- 8 CPU cores with 8GB memory limit
- Proper volume mounting for results
- Efficient layer caching in multi-stage build

## 🛠️ Customization

### Modifying Benchmark Parameters

Edit `LevenshteinLargeDocker.cs` to change:

```csharp
// Change word count and size
_words = RandomWords.Create(10, 100);  // 10 words, max 100 chars

// Modify JIT warmup iterations
for (int i = 0; i < 3; i++)  // Increase for more warmup
```

### Adding New Algorithms

1. Add package reference to the project file
2. Import the namespace in `LevenshteinLargeDocker.cs`
3. Add a new `[Benchmark]` method following existing patterns

### Custom Docker Configuration

Modify `docker-compose.yml` for different resource limits:

```yaml
cpu_count: 4        # Reduce CPU cores
mem_limit: 4g       # Reduce memory limit
mem_reservation: 2g # Reduce memory reservation
```

## 🔍 Troubleshooting

### Common Issues

**Docker Daemon Not Running**
```
Error: "error during connect: this error may indicate that the docker daemon is not running"
```
**Solution**: Start Docker Desktop or Docker service
- Windows/macOS: Open Docker Desktop
- Linux: `sudo systemctl start docker`

**Out of Memory Errors**
- Ensure your system has at least 8GB available RAM
- Close other applications to free up memory
- Reduce memory limits in docker-compose.yml if needed

**Slow Performance**
- Ensure Docker has adequate CPU/memory allocated in Docker Desktop settings
- Check if other containers are running: `docker ps`
- Monitor system resources during benchmark execution

**Build Failures**
- Clean Docker cache: `docker system prune -f`
- Rebuild without cache: `--no-cache` flag
- Check Docker disk space: `docker system df`

**Permission Errors (Linux)**
- Add user to docker group: `sudo usermod -aG docker $USER`
- Restart your session or use `newgrp docker`

**Benchmark Validation Errors**
- Our implementation uses InProcessEmitToolchain which resolves most container issues
- Privileged mode is required for BenchmarkDotNet to access performance counters

### Testing Your Setup

Use the included test script to verify your setup:

```powershell
# Windows
.\test-docker-setup.ps1

# Or double-click
.\test-setup.bat
```

### Getting Help

If you encounter issues:

1. **Check Docker status**: `docker info`
2. **View container logs**: `docker compose logs`
3. **Check system resources**: 
   - Linux: `free -h && df -h`
   - Windows: Task Manager
   - macOS: Activity Monitor

## 📊 Performance Analysis

The benchmarks provide insights into:

- **Algorithm efficiency**: Relative performance comparison
- **Memory usage patterns**: Allocation and GC pressure
- **Scalability**: Performance under high-resource conditions
- **Cross-platform consistency**: Results in standardized container environment

## 🤝 Contributing

To contribute improvements:

1. Fork the repository
2. Create a feature branch
3. Add your changes to the Docker benchmark project
4. Test with the performance configuration: `.\run-benchmark.ps1`
5. Submit a pull request with benchmark results

## 📝 License

This project follows the same license as the main FuzzySharp library.

## 🔗 Related Projects

- [FuzzySharp](https://github.com/Raffiniert/FuzzySharp) - Main library
- [BenchmarkDotNet](https://benchmarkdotnet.org/) - Benchmarking framework
- [Fastenshtein](https://github.com/DanHarltey/Fastenshtein) - Fast Levenshtein distance
- [Quickenshtein](https://github.com/Turnerj/Quickenshtein) - Optimized string distance

## 🎉 Success Story

This Docker benchmark implementation successfully demonstrates:

✅ **Working BenchmarkDotNet in containers** using modern .NET 9 SDK
✅ **Privileged mode configuration** for proper benchmark execution  
✅ **InProcessEmitToolchain** for container compatibility
✅ **High-performance configuration** with 8 CPU cores and 8GB RAM
✅ **Automated testing and validation** scripts
✅ **Comprehensive documentation** with real results
✅ **Cross-platform compatibility** (Windows/Linux/macOS)
✅ **Simplified single-profile setup** for consistent performance testing