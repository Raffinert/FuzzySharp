# Benchmarks

## Current library versus Classic

Measured on **2026-11-28**. **Latest** is a development build of Raffinert.FuzzySharp. **Classic** is the original [FuzzySharp](https://github.com/JakeBayer/FuzzySharp) **2.0.2** NuGet package. Every pair uses the same benchmark input. Speedup is `Classic mean / latest mean`.

BenchmarkDotNet 0.15.8 ran the workloads in isolated processes with `MemoryDiagnoser`: one launch, three warmups, five measured iterations, 150 ms target iteration time. Host: Windows 11 (10.0.26300.9974), Intel Core Ultra 9 275HX, 24 physical/logical cores, .NET SDK 11.0.100, .NET 10.0.15 x64 RyuJIT. The partial-token workloads were repeated after single-word mask reuse; each table uses the latest measurement for its workload. Generated reports include confidence intervals and GC measurements.

### Scoring

Times are mean nanoseconds per operation. Allocations use the units reported by BenchmarkDotNet.

| Operation | Latest | Classic 2.0.2 | Speedup | Allocated: latest | Allocated: Classic |
| --- | ---: | ---: | ---: | ---: | ---: |
| Ratio | 49.76 ns | 103.04 ns | 2.07× | 80 B | 320 B |
| PartialRatio | 229.90 ns | 463.90 ns | 2.02× | 256 B | 3368 B |
| PartialRatio (17-character pattern in 517-character text) | 935.70 ns | 8,811.30 ns | 9.42× | 1016 B | 59201 B |
| WeightedRatio | 1,886.00 ns | 5,373.00 ns | 2.85× | 3.53 KB | 11.53 KB |
| TokenSortRatio | 266.20 ns | 630.70 ns | 2.37× | 648 B | 2024 B |
| TokenSetRatio | 180.23 ns | 958.07 ns | 5.32× | 1048 B | 4096 B |
| PartialTokenSortRatio | 419.90 ns | 692.40 ns | 1.65× | 648 B | 2312 B |
| PartialTokenSetRatio | 530.80 ns | 1,143.10 ns | 2.15× | 1.8 KB | 5.45 KB |
| TokenAbbreviationRatio | 262.70 ns | 584.00 ns | 2.22× | 1.64 KB | 2.91 KB |
| PartialTokenAbbreviationRatio | 343.30 ns | 743.10 ns | 2.16× | 1.81 KB | 3.8 KB |
| TokenInitialismRatio1 | 36.15 ns | 257.49 ns | 7.12× | 280 B | 904 B |
| TokenInitialismRatio2 | 33.97 ns | 215.78 ns | 6.35× | 272 B | 736 B |
| TokenInitialismRatio3 | 52.09 ns | 514.77 ns | 9.88× | 392 B | 1552 B |
| PartialTokenInitialismRatio | 99.24 ns | 631.00 ns | 6.36× | 568 B | 2144 B |

### Cached scoring variants

`Cached per call` includes constructing a scorer for each operation. `Reused cache` creates the scorer during setup and excludes construction. All times below are mean nanoseconds per operation; stateless values come from the same measurements as the scoring table. Allocations for every method are in the linked raw reports.

| Operation | Classic | Latest stateless | Cached per call | Reused cache |
| --- | ---: | ---: | ---: | ---: |
| Ratio | 103.04 ns | 49.76 ns | 66.33 ns | 23.80 ns |
| WeightedRatio | 5,373.00 ns | 1,886.00 ns | 2,097.00 ns | 1,104.00 ns |
| PartialTokenSetRatio | 1,143.10 ns | 530.80 ns | 458.10 ns | 393.30 ns |
| TokenSetRatio | 958.07 ns | 180.23 ns | 135.45 ns | 84.10 ns |
| TokenSortRatio | 630.70 ns | 266.20 ns | 432.00 ns | 155.20 ns |

### Extraction

Each operation compares the same query against three event records. Current-library pipelines are created in setup. `Cached` creates a scorer per extraction call; `reused cache` reuses a scorer from setup. Parallel variants use at most four workers. Times are mean microseconds per operation. This small input does not establish parallel scaling for larger collections.

| Operation | Classic | Latest | Cached | Parallel | Parallel + cached | Reused cache | Parallel + reused cache |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| ExtractOne | 14.277 μs | 3.413 μs | 2.713 μs | 3.416 μs | 3.692 μs | 2.016 μs | 2.731 μs |
| ExtractAll | 10.441 μs | 3.101 μs | 2.712 μs | 3.781 μs | 3.929 μs | 1.977 μs | 2.958 μs |

Default ExtractOne measured 4.18× faster than Classic, allocating 8.34 KB versus 25.66 KB. ExtractAll measured 3.37× faster, allocating 8.41 KB versus 26.31 KB.

### Distance APIs

The fixed-input static Levenshtein distance measured 119.97 ns and 80 B in the current library versus 417.57 ns and 320 B in Classic (3.48×).

The large-word workload generates 20 words with seed 37 and lengths in `[3, 1024)`. Each operation computes all 400 ordered pairs. Fastenshtein and Raffinert.FuzzySharp reuse one distance instance per source word; the other implementations use static calls. Times and allocations below are for the whole batch. The `FuzzySharp` row is Classic 2.0.2; `Raffinert.FuzzySharp` is the local library. Ratio columns use `NaiveDp` as baseline.

| Method               | Mean       | Error     | StdDev    | Ratio | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|--------------------- |-----------:|----------:|----------:|------:|-----------:|-----------:|------------:|------------:|
| NaiveDp              | 141.729 ms | 3.8903 ms | 1.0103 ms |  1.00 | 14000.0000 | 12000.0000 | 275312720 B |       1.000 |
| FuzzySharp           |  97.509 ms | 0.7006 ms | 0.1819 ms |  0.69 |          - |          - |   1545632 B |       0.006 |
| Fastenshtein         |  82.023 ms | 6.6170 ms | 1.7184 ms |  0.58 |          - |          - |     33928 B |       0.000 |
| Quickenshtein        |   6.707 ms | 0.8855 ms | 0.2300 ms |  0.05 |          - |          - |           - |       0.000 |
| Raffinert.FuzzySharp |   1.888 ms | 0.0225 ms | 0.0035 ms |  0.01 |          - |          - |      2080 B |       0.000 |

The small-word and normal-word workloads cover maximum generated lengths of 64 and 128 respectively (exclusive). Distance implementations: [FuzzySharp](https://github.com/JakeBayer/FuzzySharp), [Fastenshtein](https://github.com/DanHarltey/Fastenshtein), [Quickenshtein](https://github.com/Turnerj/Quickenshtein), and [Raffinert.FuzzySharp](https://github.com/Raffinert/FuzzySharp).

### Scope and reports

17 of the 17 matched current-versus-Classic cases had lower measured time. The largest measured speedup was TokenInitialismRatio3 at 9.88×. These are fixed-input microbenchmarks on one machine; small differences require longer runs. Partial-ratio algorithms and score rounding differ between the libraries, so this compares API workloads rather than asserting identical scores.

The commands below generate CSV, HTML and Markdown reports with confidence intervals in a local `BenchmarkDotNet.Artifacts` directory. Generated reports are ignored by Git; the tables above retain the measured summary. Scenarios and inputs are defined in the benchmark source files in this directory.

### Reproduce

Run from the repository root in PowerShell. Clearing the optional git-baseline property selects the normal Classic comparison build.

```powershell
$env:FuzzySharpPerformanceBaseline = $null
dotnet build FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj -c Release -t:Rebuild -p:GeneratePackageOnBuild=false
$bench = 'FuzzySharp.Benchmarks/bin/Release/net10.0/FuzzySharp.Benchmarks.dll'
$output = 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-28'
dotnet $bench --filter '*.RatioBenchmarks.*' '*.PartialRatioBenchmarks.*' '*.PartialRatioLongBenchmarks.*' '*.WeightedRatioBenchmarks.*' '*Token*RatioBenchmarks*' '*.ExtractOneBenchmarks.*' '*.ExtractAllBenchmarks.*' '*.LevenshteinDistanceBenchmarks.*' '*.LevenshteinDistance.LevenshteinLarge.*' --iterationCount 5 --iterationTime 150 --warmupCount 3 --launchCount 1 --artifacts "$output/classic-comparison"
dotnet $bench --filter '*.LevenshteinDistance.LevenshteinSmall.*' '*.LevenshteinDistance.LevenshteinNormal.*' --iterationCount 5 --iterationTime 150 --warmupCount 3 --launchCount 1 --artifacts "$output/distance-sizes"
```

Use a new output directory for later runs to keep dated reports separate. BenchmarkDotNet records the environment and measurement distribution in every report.
