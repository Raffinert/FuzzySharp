using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Raffinert.FuzzySharp.Benchmarks.Docker.Utils;
using FastLevenshtein = Fastenshtein.Levenshtein;
using FuzzLevenshtein = Raffinert.FuzzySharp.Levenshtein;
using FuzzLevenshteinClassic = FuzzySharp.Levenshtein;
using QuickLevenshtein = Quickenshtein.Levenshtein;

namespace Raffinert.FuzzySharp.Benchmarks.Docker.LevenshteinDistance;

[MemoryDiagnoser]
[Config(typeof(DockerBenchmarkConfig))]
public class LevenshteinLargeDocker
{
    private string[] _words = null!;

    [GlobalSetup]
    public void SetUp()
    {
        _words = RandomWords.Create(20, 1024);

        // Pre-warm the JIT in Docker environment
        Console.WriteLine("Pre-warming JIT compiler...");
        for (int i = 0; i < 3; i++)
        {
            LevenshteinBaseline.GetDistance(_words[0], _words[1]);
            FuzzLevenshtein.Distance(_words[0], _words[1]);
        }
        Console.WriteLine("JIT pre-warming completed.");
        Console.Out.Flush();

        // Display container information
        Console.WriteLine($"Running in container. Word count: {_words.Length}");
        Console.WriteLine($"Max word length: {_words.Max(w => w.Length)}");
        Console.WriteLine($"Environment: {Environment.OSVersion}");
        Console.WriteLine($"Processor count: {Environment.ProcessorCount}");
        Console.WriteLine($"Working set: {Environment.WorkingSet / 1024 / 1024} MB");
        Console.WriteLine($"GC settings - Server: {System.Runtime.GCSettings.IsServerGC}, Concurrent: {System.Runtime.GCSettings.LatencyMode}");
        Console.Out.Flush();

        // Test that all benchmark methods work
        Console.WriteLine("Testing benchmark methods...");
        try
        {
            NaiveDp();
            Console.WriteLine("✓ NaiveDp working");

            NewMatchEngineEditDistance();
            Console.WriteLine("✓ NewMatchEngineEditDistance working");

            FuzzySharpClassic();
            Console.WriteLine("✓ FuzzySharpClassic working");

            Fastenshtein();
            Console.WriteLine("✓ Fastenshtein working");

            Quickenshtein();
            Console.WriteLine("✓ Quickenshtein working");

            FuzzySharp();
            Console.WriteLine("✓ FuzzySharp working");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in benchmark method: {ex.Message}");
            throw;
        }

        Console.WriteLine("All benchmark methods tested successfully.");
        Console.WriteLine("Starting benchmark execution...");
        Console.Out.Flush();
    }

    [Benchmark(Baseline = true)]
    public void NaiveDp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                LevenshteinBaseline.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void NewMatchEngineEditDistance()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                LevenshteinBaseline.NewMatchEngineEditDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharpClassic()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                FuzzLevenshteinClassic.EditDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void Fastenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var levenshtein = new FastLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                levenshtein.DistanceFrom(_words[j]);
            }
        }
    }

    [Benchmark]
    public void Quickenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                QuickLevenshtein.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                FuzzLevenshtein.Distance(_words[i], _words[j]);
            }
        }
    }
}

public class DockerBenchmarkConfig : ManualConfig
{
    public DockerBenchmarkConfig()
    {
        // Use InProcessEmitToolchain for Docker containers as recommended
        AddJob(Job.Default
            .WithRuntime(CoreRuntime.Core90)
            .WithPlatform(Platform.X64)
            .WithJit(Jit.RyuJit)
            .WithToolchain(InProcessEmitToolchain.Instance)
            .WithGcMode(new GcMode
            {
                Server = true,
                Concurrent = true,
                RetainVm = false,
                Force = false
            })
            .WithStrategy(RunStrategy.Throughput)
            .WithLaunchCount(1)
            .WithWarmupCount(2)
            .WithIterationCount(3)
        // Remove explicit InvocationCount to let BenchmarkDotNet calculate it
        );

        // Add console logger to ensure output is visible
        AddLogger(ConsoleLogger.Default);

        // Disable optimizations validator which can cause issues in containers
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}