# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /src

# Copy project files for dependency resolution
COPY ["FuzzySharp/FuzzySharp.csproj", "FuzzySharp/"]
COPY ["FuzzySharp.Benchmarks.Docker/FuzzySharp.Benchmarks.Docker.csproj", "FuzzySharp.Benchmarks.Docker/"]

# Restore dependencies
RUN dotnet restore "FuzzySharp.Benchmarks.Docker/FuzzySharp.Benchmarks.Docker.csproj"

# Copy all source code
COPY . .

# Build the benchmark project
WORKDIR "/src/FuzzySharp.Benchmarks.Docker"
RUN dotnet build "FuzzySharp.Benchmarks.Docker.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "FuzzySharp.Benchmarks.Docker.csproj" -c Release -o /app/publish --no-restore

# Use the official .NET 9 SDK image for running (BenchmarkDotNet requires SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime

# Install required packages for performance monitoring
RUN apt-get update && apt-get install -y \
    htop \
    procps \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Create directories for benchmark results
RUN mkdir -p /app/BenchmarkDotNet.Artifacts/results

# Set environment variables for optimal performance
ENV DOTNET_gcServer=1
ENV DOTNET_gcConcurrent=1
ENV DOTNET_GCRetainVM=0
ENV DOTNET_ReadyToRun=1
ENV DOTNET_TieredCompilation=1
ENV DOTNET_TieredPGO=1

# Create a non-root user for security
RUN useradd -m -s /bin/bash benchmarkuser && chown -R benchmarkuser:benchmarkuser /app
USER benchmarkuser

# Expose the results directory as a volume
VOLUME ["/app/BenchmarkDotNet.Artifacts"]

# Default command
ENTRYPOINT ["dotnet", "FuzzySharp.Benchmarks.Docker.dll"]