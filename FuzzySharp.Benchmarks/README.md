# Benchmarks

## Current library versus Classic

Measured on **2026-11-28**. **Latest** is a development build of Raffinert.FuzzySharp including the single mask lookup in multiword partial-ratio edge scans. **Classic** is the original [FuzzySharp](https://github.com/JakeBayer/FuzzySharp) **2.0.2** NuGet package. Every pair uses the same benchmark input. Speedup is `Classic mean / latest mean`.

BenchmarkDotNet 0.15.8 ran all workloads below in isolated processes with `MemoryDiagnoser`: one launch, five warmups, seven measured iterations, 250 ms target iteration time. Host: Windows 11 (10.0.26300.9974), Intel Core Ultra 9 275HX, 24 physical/logical cores, .NET SDK 11.0.100, .NET 10.0.15 x64 RyuJIT. Runtime tiering and CPU affinity use their defaults. All tables come from the same benchmark invocation. Generated reports include confidence intervals and GC measurements.

### Scoring

Times are mean nanoseconds per operation. Allocations use the units reported by BenchmarkDotNet.

| Operation | Latest | Classic 2.0.2 | Speedup | Allocated: latest | Allocated: Classic |
| --- | ---: | ---: | ---: | ---: | ---: |
| Ratio | 86.14 ns | 131.20 ns | 1.52× | 80 B | 320 B |
| PartialRatio | 286.20 ns | 630.50 ns | 2.20× | 256 B | 3368 B |
| PartialRatio (17-character pattern in 517-character text) | 1,437.00 ns | 13,244.00 ns | 9.22× | 1016 B | 59201 B |
| WeightedRatio | 2,809.00 ns | 6,072.00 ns | 2.16× | 3.53 KB | 11.53 KB |
| TokenSortRatio | 397.20 ns | 835.00 ns | 2.10× | 648 B | 2024 B |
| TokenSetRatio | 209.40 ns | 1,166.00 ns | 5.57× | 1048 B | 4096 B |
| PartialTokenSortRatio | 594.40 ns | 858.70 ns | 1.44× | 648 B | 2312 B |
| PartialTokenSetRatio | 672.70 ns | 1,392.50 ns | 2.07× | 1.8 KB | 5.45 KB |
| TokenAbbreviationRatio | 348.70 ns | 743.80 ns | 2.13× | 1.64 KB | 2.91 KB |
| PartialTokenAbbreviationRatio | 416.40 ns | 906.60 ns | 2.18× | 1.81 KB | 3.8 KB |
| TokenInitialismRatio1 | 47.38 ns | 312.32 ns | 6.59× | 280 B | 904 B |
| TokenInitialismRatio2 | 42.91 ns | 259.27 ns | 6.04× | 272 B | 736 B |
| TokenInitialismRatio3 | 81.70 ns | 620.50 ns | 7.59× | 392 B | 1552 B |
| PartialTokenInitialismRatio | 138.00 ns | 741.60 ns | 5.37× | 568 B | 2144 B |

### Multiword PartialRatio

Each pair consists of two independently generated strings of the same length, using an alphabet of 16 ASCII or Cyrillic characters and seed `length * 7919`. Pattern lengths exceed 64 elements, exercising multiword prefix/suffix scans and the combined mask lookup. Both libraries receive the same strings; their scores can differ because their partial-ratio algorithms differ. Times are mean microseconds per operation. Source: [PartialRatioMultiwordBenchmarks.cs](PartialRatioMultiwordBenchmarks.cs).

| Length | Alphabet | Latest | Classic 2.0.2 | Speedup | Allocated: latest | Allocated: Classic |
| ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 65 | ASCII | 2.311 μs | 21.182 μs | 9.17× | 320 B | 25488 B |
| 65 | Cyrillic | 3.827 μs | 21.176 μs | 5.53× | 576 B | 25488 B |
| 257 | ASCII | 10.937 μs | 4,901.348 μs | 448.14× | 320 B | 402751 B |
| 257 | Cyrillic | 15.581 μs | 4,528.323 μs | 290.63× | 576 B | 402744 B |
| 1025 | ASCII | 199.265 μs | 498,397.817 μs | 2,501.18× | 320 B | 6470720 B |
| 1025 | Cyrillic | 224.963 μs | 494,415.050 μs | 2,197.76× | 576 B | 6483056 B |

### Cached scoring variants

`Cached per call` includes constructing a scorer for each operation. `Reused cache` creates the scorer during setup and excludes construction. All times below are mean nanoseconds per operation; stateless values come from the same measurements as the scoring table. Allocations for every method are in the generated raw reports.

| Operation | Classic | Latest stateless | Cached per call | Reused cache |
| --- | ---: | ---: | ---: | ---: |
| Ratio | 131.20 ns | 86.14 ns | 90.44 ns | 31.80 ns |
| WeightedRatio | 6,072.00 ns | 2,809.00 ns | 3,082.00 ns | 1,718.00 ns |
| PartialTokenSetRatio | 1,392.50 ns | 672.70 ns | 617.10 ns | 532.00 ns |
| TokenSetRatio | 1,166.00 ns | 209.40 ns | 157.60 ns | 100.60 ns |
| TokenSortRatio | 835.00 ns | 397.20 ns | 593.50 ns | 229.60 ns |

### Extraction

Each operation compares the same query against three event records. Current-library pipelines are created in setup. `Cached` creates a scorer per extraction call; `reused cache` reuses a scorer from setup. Parallel variants use at most four workers. Times are mean microseconds per operation. This small input does not establish parallel scaling for larger collections.

| Operation | Classic | Latest | Cached | Parallel | Parallel + cached | Reused cache | Parallel + reused cache |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| ExtractOne | 12.569 μs | 4.842 μs | 3.696 μs | 3.816 μs | 3.993 μs | 2.795 μs | 2.895 μs |
| ExtractAll | 12.750 μs | 4.691 μs | 3.835 μs | 3.830 μs | 4.162 μs | 2.958 μs | 2.919 μs |

Default ExtractOne measured 2.60× faster than Classic, allocating 8.34 KB versus 26.22 KB. ExtractAll measured 2.72× faster, allocating 8.41 KB versus 26.31 KB.

### Distance APIs

The fixed-input static Levenshtein distance measured 147.43 ns and 80 B in the current library versus 543.53 ns and 320 B in Classic (3.69×).

The large-word workload generates 20 words with seed 37 and lengths in `[3, 1024)`. Each operation computes all 400 ordered pairs. Fastenshtein and Raffinert.FuzzySharp reuse one distance instance per source word; the other implementations use static calls. Times and allocations below are for the whole batch. The `FuzzySharp` row is Classic 2.0.2; `Raffinert.FuzzySharp` is the development build. Ratio columns use `NaiveDp` as baseline.

| Method               | Mean       | Error      | StdDev     | Ratio | RatioSD | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|--------------------- |-----------:|-----------:|-----------:|------:|--------:|-----------:|-----------:|------------:|------------:|
| NaiveDp              | 172.717 ms | 22.5081 ms |  9.9937 ms |  1.00 |    0.08 | 14500.0000 | 12500.0000 | 275312732 B |       1.000 |
| FuzzySharp           |  96.705 ms |  5.9356 ms |  2.1167 ms |  0.56 |    0.04 |          - |          - |   1545656 B |       0.006 |
| Fastenshtein         |  95.141 ms | 25.4851 ms | 11.3156 ms |  0.55 |    0.07 |          - |          - |     33928 B |       0.000 |
| Quickenshtein        |   8.896 ms |  3.2364 ms |  1.4370 ms |  0.05 |    0.01 |          - |          - |           - |       0.000 |
| Raffinert.FuzzySharp |   1.960 ms |  0.1462 ms |  0.0649 ms |  0.01 |    0.00 |          - |          - |      2080 B |       0.000 |

The small-word and normal-word workloads cover maximum generated lengths of 64 and 128 respectively (exclusive). Distance implementations: [FuzzySharp](https://github.com/JakeBayer/FuzzySharp), [Fastenshtein](https://github.com/DanHarltey/Fastenshtein), [Quickenshtein](https://github.com/Turnerj/Quickenshtein), and [Raffinert.FuzzySharp](https://github.com/Raffinert/FuzzySharp).

### Scope and reports

These are fixed-input microbenchmarks on one machine; small differences require longer runs. Partial-ratio algorithms and score rounding differ between the libraries, so this compares API workloads rather than asserting identical scores. The multiword cases compare the current library with Classic; they do not isolate the gain from the mask-lookup change alone.

The command below generates CSV, HTML and Markdown reports with confidence intervals in a local `BenchmarkDotNet.Artifacts` directory. Generated reports are ignored by Git; the tables above retain the measured summary. Scenarios and inputs are defined in the benchmark source files in this directory.

### Reproduce

Run from the repository root in PowerShell. Clearing the optional git-baseline property selects the normal Classic comparison build.

```powershell
$env:FuzzySharpPerformanceBaseline = $null
dotnet build FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj -c Release -t:Rebuild -p:GeneratePackageOnBuild=false
$bench = 'FuzzySharp.Benchmarks/bin/Release/net10.0/FuzzySharp.Benchmarks.dll'
$output = 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-28-mask-lookup'

dotnet $bench --filter '*.RatioBenchmarks.*' '*.PartialRatioBenchmarks.*' '*.PartialRatioLongBenchmarks.*' '*.PartialRatioMultiwordBenchmarks.*' '*.WeightedRatioBenchmarks.*' '*Token*RatioBenchmarks*' '*.ExtractOneBenchmarks.*' '*.ExtractAllBenchmarks.*' '*.LevenshteinDistanceBenchmarks.*' '*.LevenshteinDistance.LevenshteinLarge.*' '*.LevenshteinDistance.LevenshteinSmall.*' '*.LevenshteinDistance.LevenshteinNormal.*' --iterationCount 7 --iterationTime 250 --warmupCount 5 --launchCount 1 --artifacts "$output/classic-comparison"
```

Use a new output directory for later runs to keep reports separate. BenchmarkDotNet records the environment and measurement distribution in every report.
