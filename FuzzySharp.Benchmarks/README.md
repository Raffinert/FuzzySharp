# Benchmarks

## Latest library versus Classic

Measured on 2026-11-24. **Latest** is the project reference to Raffinert.FuzzySharp **6.0.2** at commit `aab9111`; **Classic** is the `FuzzySharp` **2.0.2** NuGet package referenced by this benchmark project. Each row compares the corresponding methods on the same benchmark input. The speedup column is `Classic mean / latest mean`, so values above 1 mean the latest library was faster in this run.

BenchmarkDotNet 0.15.8 ran the `ShortRun` job in isolated processes: one launch, three warmup iterations and three measured iterations, with `MemoryDiagnoser`. Host: Windows 11 (10.0.26300.9974), Intel Core Ultra 9 275HX, 24 logical cores, .NET SDK 10.0.403 and .NET 10.0.14 x64 RyuJIT.

### Scoring

Times are mean nanoseconds per operation; allocations are bytes per operation.

| Operation | Latest | Classic | Speedup | Allocated: latest | Allocated: Classic |
| --- | ---: | ---: | ---: | ---: | ---: |
| `Ratio` | 55.20 ns | 124.60 ns | 2.26× | 80 B | 320 B |
| `PartialRatio` | 239.98 ns | 536.44 ns | 2.24× | 256 B | 3,368 B |
| `PartialRatio` (short pattern in 517-character text) | 1,132.50 ns | 10,083.48 ns | 8.90× | 1,016 B | 59,201 B |
| `WeightedRatio` | 2,273.55 ns | 6,312.93 ns | 2.78× | 3,776 B | 11,809 B |
| `TokenSortRatio` | 307.50 ns | 727.27 ns | 2.37× | 648 B | 2,024 B |
| `TokenSetRatio` | 337.43 ns | 1,143.08 ns | 3.39× | 1,496 B | 4,096 B |
| `PartialTokenSortRatio` | 562.95 ns | 808.24 ns | 1.44× | 648 B | 2,344 B |
| `PartialTokenSetRatio` | 592.46 ns | 1,362.45 ns | 2.30× | 1,848 B | 5,584 B |
| `TokenAbbreviationRatio` | 314.88 ns | 663.63 ns | 2.11× | 1,680 B | 2,984 B |
| `PartialTokenAbbreviationRatio` | 375.46 ns | 838.65 ns | 2.23× | 1,856 B | 3,896 B |
| `TokenInitialismRatio1` | 42.68 ns | 314.91 ns | 7.38× | 280 B | 904 B |
| `TokenInitialismRatio2` | 38.45 ns | 234.78 ns | 6.11× | 272 B | 736 B |
| `TokenInitialismRatio3` | 60.71 ns | 614.33 ns | 10.12× | 392 B | 1,552 B |
| `PartialTokenInitialismRatio` | 106.53 ns | 687.16 ns | 6.45× | 568 B | 2,144 B |

#### Cached scoring variants

The following current-library methods have cached scorers. `Cached per call` includes creating a scorer for each operation; `reused cache` creates the scorer in benchmark setup, so its construction cost is excluded. All times are mean nanoseconds per operation. These are separate `ShortRun` measurements, so their stateless values may differ slightly from the table above.

| Operation | Classic | Latest stateless | Cached per call | Reused cache |
| --- | ---: | ---: | ---: | ---: |
| `Ratio` | 118.97 ns | 60.65 ns | 73.81 ns | 28.11 ns |
| `WeightedRatio` | 6,173.40 ns | 2,167.15 ns | 2,678.42 ns | 1,440.17 ns |
| `PartialTokenSetRatio` | 1,362.45 ns | 592.46 ns | 518.48 ns | 439.90 ns |
| `TokenSetRatio` | 1,143.08 ns | 337.43 ns | 271.46 ns | 216.66 ns |
| `TokenSortRatio` | 727.27 ns | 307.50 ns | 496.64 ns | 164.01 ns |

Parallel variants are benchmarked for extraction below; these scoring benchmarks do not include a parallel scoring API. The reproduction commands generate the full BenchmarkDotNet results, including allocations for cached variants.

### Extraction and distance

The extraction variants use the same three event records and query. The current-library cached and parallel pipelines are created during benchmark setup. `Cached` creates a scorer for each call; `reused cache` reuses a scorer from setup and excludes its construction cost. Parallel variants use at most four workers. Times are mean microseconds per operation.

| Operation | Classic | Latest | Cached | Parallel | Parallel + cached | Reused cache | Parallel + reused cache |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `ExtractOne` | 12.626 μs | 4.593 μs | 3.169 μs | 3.858 μs | 4.833 μs | 2.437 μs | 3.556 μs |
| `ExtractAll` | 12.598 μs | 4.360 μs | 3.503 μs | 4.189 μs | 4.682 μs | 2.884 μs | 3.323 μs |

Against Classic, the default latest-library methods measured **2.75×** faster for `ExtractOne` and **2.89×** for `ExtractAll` in this run. Default `ExtractOne` allocated **8.96 KB** versus **26.22 KB** for Classic; `ExtractAll` allocated **9.23 KB** versus **26.31 KB**. The reproduction command generates allocations for every variant. With only three choices, these measurements do not establish how parallel execution scales to larger collections.

Levenshtein distance measured **122.60 ns and 80 B** in the latest library versus **367.99 ns and 320 B** in Classic, or **3.00×** faster in this case.

Across the 17 matched latest-versus-Classic cases, the latest library had lower measured time and allocated fewer bytes. The largest measured speedup was `TokenInitialismRatio3` at 10.12×. These are specific inputs, and `ShortRun` has only three measured iterations; several confidence intervals are wide. Small differences should be rerun with a longer job before making a performance claim.

The tables use separate runs for the basic scorer pairs, token scorers, cache variants and extraction variants. Repeated means may differ slightly between runs.

### Reproduce

From the repository root in PowerShell, with packages restored:

```powershell
dotnet build FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj -c Release --no-restore
$env:NuGetAudit = 'false' # The online NuGet audit feed was unavailable for this run.
$bench = 'FuzzySharp.Benchmarks/bin/Release/net10.0/FuzzySharp.Benchmarks.dll'
dotnet $bench --filter '*RatioBenchmarks.Ratio' '*RatioBenchmarks.RatioClassic' '*PartialRatioBenchmarks.PartialRatio' '*PartialRatioBenchmarks.PartialRatioClassic' '*WeightedRatioBenchmarks.WeightedRatio' '*WeightedRatioBenchmarks.WeightedRatioClassic' --artifacts 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-24/classic-core' --join
dotnet $bench --filter '*Token*RatioBenchmarks*' '*PartialRatioLongBenchmarks*' '*ExtractOneBenchmarks.ExtractOneBy' '*ExtractOneBenchmarks.ExtractOneByClassic' '*ExtractAllBenchmarks.ExtractAllBy' '*ExtractAllBenchmarks.ExtractAllByClassic' '*LevenshteinDistanceBenchmarks.FuzzySharpDistance' '*LevenshteinDistanceBenchmarks.FuzzySharpClassicDistance' --artifacts 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-24/classic-comparison' --join
dotnet $bench --filter '*.RatioBenchmarks.*' '*.WeightedRatioBenchmarks.*' --artifacts 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-24/scoring-cache-variants' --join
dotnet $bench --filter '*ExtractOneBenchmarks*' '*ExtractAllBenchmarks*' --artifacts 'FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/2026-11-24/extraction-variants' --join
```

For a stable comparison, run on an otherwise idle machine. BenchmarkDotNet records the environment and the full measurement distribution in its generated reports.
