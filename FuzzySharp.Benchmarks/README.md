# Benchmarks

## Measured build versus Classic

Measured on **2026-11-28**. **Measured build** is a development build of Raffinert.FuzzySharp including the single mask lookup in multiword partial-ratio edge scans. **Classic** is the original [FuzzySharp](https://github.com/JakeBayer/FuzzySharp) **2.0.2** NuGet package. Every pair uses the same benchmark input. Speedup is `Classic mean / measured mean`.

The measured library sources are at [e60e993](https://github.com/Raffinert/FuzzySharp/commit/e60e99337b6ae3d05153a66ce41dbed68598e470). This snapshot precedes the subsequent token-set scorer refactor; its numbers do not measure that refactor. Use the linked revision's sources with the commands below to reproduce this snapshot.

BenchmarkDotNet 0.15.8 ran the workloads in isolated processes with `MemoryDiagnoser`: two launches, six warmups and ten measured iterations per launch, 300 ms target iteration time, and no outlier removal. Host: Windows 11 (10.0.26300.9974), Intel Core Ultra 9 275HX, 24 physical/logical cores, .NET SDK 11.0.100, .NET 10.0.15 x64 RyuJIT. Sequential methods run on logical CPU 10, a P-core (affinity mask `1024`); parallel extraction methods use P-cores 10–13 (mask `15360`). Core classes were checked through [Windows CPU Set Information](https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-system_cpu_set_information). `DOTNET_TieredCompilation=0` disables JIT tier transitions and dynamic PGO, selecting optimized code from startup. This is an explicitly controlled JIT configuration rather than the runtime default. A five-second idle probe before calibration reported 1.7% total CPU use; temperature was not recorded.

These tables replace the earlier unrestricted-affinity run, which showed large variation in unchanged methods. The JIT and affinity settings differ from that run, so absolute times across the two runs do not establish a code regression. All pairs below use the same configuration; `±` denotes BenchmarkDotNet's 99.9% confidence-interval half-width.

### Scoring

Times are mean nanoseconds per operation with 99.9% confidence intervals. Allocations use the units reported by BenchmarkDotNet.

| Operation | Measured build | Classic 2.0.2 | Speedup | Allocated: measured | Allocated: Classic |
| --- | ---: | ---: | ---: | ---: | ---: |
| Ratio | 55.47 ± 0.54 ns | 174.65 ± 1.79 ns | 3.15× | 80 B | 320 B |
| PartialRatio | 246.00 ± 2.14 ns | 613.00 ± 8.36 ns | 2.49× | 256 B | 3368 B |
| PartialRatio (17-character pattern in 517-character text) | 1,051.00 ± 6.40 ns | 10,976.00 ± 48.70 ns | 10.44× | 1016 B | 59201 B |
| WeightedRatio | 3,125.00 ± 47.00 ns | 8,984.00 ± 25.30 ns | 2.87× | 3.59 KB | 12.47 KB |
| TokenSortRatio | 377.20 ± 1.59 ns | 1,452.70 ± 13.37 ns | 3.85× | 648 B | 2152 B |
| TokenSetRatio | 325.20 ± 3.70 ns | 2,017.90 ± 6.99 ns | 6.21× | 1048 B | 4224 B |
| PartialTokenSortRatio | 548.00 ± 3.94 ns | 1,498.70 ± 4.58 ns | 2.73× | 648 B | 2472 B |
| PartialTokenSetRatio | 832.90 ± 2.12 ns | 2,240.60 ± 15.23 ns | 2.69× | 1.8 KB | 5.58 KB |
| TokenAbbreviationRatio | 525.90 ± 16.90 ns | 1,213.70 ± 5.14 ns | 2.31× | 1.64 KB | 2.91 KB |
| PartialTokenAbbreviationRatio | 586.70 ± 4.67 ns | 1,368.00 ± 15.58 ns | 2.33× | 1.81 KB | 3.8 KB |
| TokenInitialismRatio1 | 52.89 ± 0.69 ns | 454.39 ± 5.38 ns | 8.59× | 280 B | 904 B |
| TokenInitialismRatio2 | 49.25 ± 0.12 ns | 373.43 ± 1.70 ns | 7.58× | 272 B | 736 B |
| TokenInitialismRatio3 | 86.23 ± 0.20 ns | 832.15 ± 7.13 ns | 9.65× | 392 B | 1552 B |
| PartialTokenInitialismRatio | 135.40 ± 2.10 ns | 918.80 ± 12.98 ns | 6.79× | 568 B | 2144 B |

### Multiword PartialRatio

Each pair consists of two independently generated strings of the same length, using an alphabet of 16 ASCII or Cyrillic characters and seed `length * 7919`. Pattern lengths exceed 64 elements, exercising multiword prefix/suffix scans and the combined mask lookup. Both libraries receive the same strings; their scores can differ because their partial-ratio algorithms differ. Times are mean microseconds per operation with 99.9% confidence intervals. Source: [PartialRatioMultiwordBenchmarks.cs](PartialRatioMultiwordBenchmarks.cs).

| Length | Alphabet | Measured build | Classic 2.0.2 | Speedup | Allocated: measured | Allocated: Classic |
| ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 65 | ASCII | 1.987 ± 0.004 μs | 25.166 ± 0.083 μs | 12.67× | 320 B | 25488 B |
| 65 | Cyrillic | 3.492 ± 0.006 μs | 25.326 ± 0.091 μs | 7.25× | 576 B | 25488 B |
| 257 | ASCII | 9.429 ± 0.082 μs | 5,422.976 ± 23.208 μs | 575.14× | 320 B | 402751 B |
| 257 | Cyrillic | 13.220 ± 0.075 μs | 5,436.591 ± 37.149 μs | 411.24× | 576 B | 402751 B |
| 1025 | ASCII | 160.978 ± 0.337 μs | 455,837.220 ± 1,739.840 μs | 2,831.67× | 320 B | 6470720 B |
| 1025 | Cyrillic | 178.948 ± 0.336 μs | 464,753.595 ± 21,788.188 μs | 2,597.14× | 576 B | 6470720 B |

### Cached scoring variants

`Cached per call` includes constructing a scorer for each operation. `Reused cache` creates the scorer during setup and excludes construction. All times below are mean nanoseconds per operation; stateless values come from the same measurements as the scoring table. Allocations for every method are in the generated raw reports.

| Operation | Classic | Measured stateless | Cached per call | Reused cache |
| --- | ---: | ---: | ---: | ---: |
| Ratio | 174.65 ± 1.79 ns | 55.47 ± 0.54 ns | 88.50 ± 1.07 ns | 44.21 ± 0.27 ns |
| WeightedRatio | 8,984.00 ± 25.30 ns | 3,125.00 ± 47.00 ns | 3,350.00 ± 14.20 ns | 1,938.00 ± 15.70 ns |
| PartialTokenSetRatio | 2,240.60 ± 15.23 ns | 832.90 ± 2.12 ns | 778.20 ± 3.66 ns | 646.40 ± 4.47 ns |
| TokenSetRatio | 2,017.90 ± 6.99 ns | 325.20 ± 3.70 ns | 294.00 ± 1.49 ns | 176.30 ± 0.64 ns |
| TokenSortRatio | 1,452.70 ± 13.37 ns | 377.20 ± 1.59 ns | 550.20 ± 3.83 ns | 218.40 ± 0.55 ns |

### Extraction

Each operation compares the same query against three event records. Current-library pipelines are created in setup. `Cached` creates a scorer per extraction call; `reused cache` reuses a scorer from setup. Parallel variants use at most four workers. Times are mean microseconds per operation. This small input does not establish parallel scaling for larger collections.

| Operation | Classic | Measured build | Cached | Parallel | Parallel + cached | Reused cache | Parallel + reused cache |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| ExtractOne | 19.320 ± 0.253 μs | 5.449 ± 0.019 μs | 4.290 ± 0.015 μs | 4.441 ± 0.012 μs | 5.138 ± 0.045 μs | 3.573 ± 0.019 μs | 3.828 ± 0.012 μs |
| ExtractAll | 19.372 ± 0.176 μs | 5.633 ± 0.016 μs | 4.515 ± 0.013 μs | 4.268 ± 0.030 μs | 5.045 ± 0.050 μs | 3.655 ± 0.013 μs | 3.595 ± 0.020 μs |

Default ExtractOne measured 3.55× faster than Classic, allocating 8.4 KB versus 27.34 KB. ExtractAll measured 3.44× faster, allocating 8.6 KB versus 27.43 KB.

### Distance APIs

The fixed-input static Levenshtein distance measured 126.70 ± 0.36 ns and 80 B in the current library versus 558.90 ± 3.65 ns and 320 B in Classic (4.41×).

The large-word workload generates 20 words with seed 37 and lengths in `[3, 1024)`. Each operation computes all 400 ordered pairs. Fastenshtein and Raffinert.FuzzySharp reuse one distance instance per source word; the other implementations use static calls. Times and allocations below are for the whole batch. The `FuzzySharp` row is Classic 2.0.2; `Raffinert.FuzzySharp` is the development build. Ratio columns use `NaiveDp` as baseline.

| Method               | Mean       | Error     | StdDev    | Ratio | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|--------------------- |-----------:|----------:|----------:|------:|-----------:|-----------:|------------:|------------:|
| NaiveDp              | 146.787 ms | 1.2307 ms | 1.4173 ms |  1.00 | 14500.0000 | 12500.0000 | 275312720 B |       1.000 |
| FuzzySharp           | 105.555 ms | 0.0896 ms | 0.1032 ms |  0.72 |          - |          - |   1545632 B |       0.006 |
| Fastenshtein         |  97.087 ms | 0.4391 ms | 0.5056 ms |  0.66 |          - |          - |     33928 B |       0.000 |
| Quickenshtein        |   8.226 ms | 0.0315 ms | 0.0363 ms |  0.06 |          - |          - |           - |       0.000 |
| Raffinert.FuzzySharp |   2.399 ms | 0.0065 ms | 0.0075 ms |  0.02 |          - |          - |      2080 B |       0.000 |

Additional small-word and normal-word benchmark sources cover maximum generated lengths of 64 and 128 respectively (exclusive); they are not part of these tables. Distance implementations: [FuzzySharp](https://github.com/JakeBayer/FuzzySharp), [Fastenshtein](https://github.com/DanHarltey/Fastenshtein), [Quickenshtein](https://github.com/Turnerj/Quickenshtein), and [Raffinert.FuzzySharp](https://github.com/Raffinert/FuzzySharp).

### Scope and reports

These are fixed-input microbenchmarks on one machine; small differences require longer runs. Partial-ratio algorithms and score rounding differ between the libraries, so this compares API workloads rather than asserting identical scores. The multiword cases compare the current library with Classic; they do not isolate the gain from the mask-lookup change alone.

### Repeatability controls

Ratio, Classic Ratio and Fastenshtein were measured in a separate calibration run before the full series, using the same P-core and JIT settings. The table shows calibration and full-series means with their 99.9% confidence intervals. Drift is `full-series mean / calibration mean - 1`.

| Control | Calibration | Full series | Drift |
| --- | ---: | ---: | ---: |
| Ratio | 55.82 ± 0.75 ns | 55.47 ± 0.54 ns | -0.63% |
| RatioClassic | 174.32 ± 2.06 ns | 174.65 ± 1.79 ns | 0.19% |
| Fastenshtein | 97.150 ± 0.342 ms | 97.087 ± 0.439 ms | -0.06% |

The commands below generate CSV, HTML and Markdown reports with confidence intervals in a local `BenchmarkDotNet.Artifacts` directory. Generated reports are ignored by Git; the tables above retain the measured summary. Scenarios and inputs are defined in the benchmark source files in this directory.

### Reproduce

Run from the repository root in PowerShell. Clearing the optional git-baseline property selects the normal Classic comparison build.

```powershell
$env:FuzzySharpPerformanceBaseline = $null
dotnet build FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj -c Release -t:Rebuild -p:GeneratePackageOnBuild=false
$bench = 'FuzzySharp.Benchmarks/bin/Release/net10.0/FuzzySharp.Benchmarks.dll'
$output = 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-28-pcore'

dotnet $bench --filter '*.RatioBenchmarks.Ratio' '*.RatioBenchmarks.RatioClassic' '*.LevenshteinDistance.LevenshteinLarge.Fastenshtein' --affinity 1024 --envVars DOTNET_TieredCompilation:0 --iterationCount 10 --iterationTime 300 --warmupCount 6 --launchCount 2 --outliers DontRemove --artifacts "$output/pilot"

$sequential = @(
    '*.RatioBenchmarks.*', '*.PartialRatioBenchmarks.*', '*.PartialRatioLongBenchmarks.*',
    '*.PartialRatioMultiwordBenchmarks.*', '*.WeightedRatioBenchmarks.*', '*Token*RatioBenchmarks*',
    '*.ExtractOneBenchmarks.ExtractOneBy', '*.ExtractOneBenchmarks.ExtractOneByClassic',
    '*.ExtractOneBenchmarks.ExtractOneByCached', '*.ExtractOneBenchmarks.ExtractOneByAcrossRunsCached',
    '*.ExtractAllBenchmarks.ExtractAllBy', '*.ExtractAllBenchmarks.ExtractAllByClassic',
    '*.ExtractAllBenchmarks.ExtractAllByCached', '*.ExtractAllBenchmarks.ExtractAllByAcrossRunsCached',
    '*.LevenshteinDistanceBenchmarks.FuzzySharpDistance',
    '*.LevenshteinDistanceBenchmarks.FuzzySharpClassicDistance',
    '*.LevenshteinDistance.LevenshteinLarge.*'
)
dotnet $bench --filter $sequential --affinity 1024 --envVars DOTNET_TieredCompilation:0 --iterationCount 10 --iterationTime 300 --warmupCount 6 --launchCount 2 --outliers DontRemove --artifacts "$output/sequential"

dotnet $bench --filter '*.ExtractOneBenchmarks.*Parallel*' '*.ExtractAllBenchmarks.*Parallel*' --affinity 15360 --envVars DOTNET_TieredCompilation:0 --iterationCount 10 --iterationTime 300 --warmupCount 6 --launchCount 2 --outliers DontRemove --artifacts "$output/parallel"
```

Use a new output directory for later runs to keep reports separate. Verify core topology and choose suitable affinity masks for your machine; these masks refer to this host only. BenchmarkDotNet records the environment and measurement distribution in every report.
