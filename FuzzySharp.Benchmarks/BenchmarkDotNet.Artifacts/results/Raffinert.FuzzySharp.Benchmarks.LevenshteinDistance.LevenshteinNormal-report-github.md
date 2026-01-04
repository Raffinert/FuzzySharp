```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-1255U 2.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.301
  [Host]   : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error       | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Allocated  | Alloc Ratio |
|------------------ |-----------:|------------:|----------:|------:|--------:|----------:|---------:|-----------:|------------:|
| NaiveDp           | 8,613.1 μs | 4,977.60 μs | 272.84 μs |  1.00 |    0.04 | 1593.7500 | 203.1250 | 10012124 B |       1.000 |
| FuzzySharpClassic | 4,866.5 μs |   866.89 μs |  47.52 μs |  0.57 |    0.02 |   46.8750 |        - |   300051 B |       0.030 |
| Fastenshtein      | 4,076.7 μs | 1,265.24 μs |  69.35 μs |  0.47 |    0.01 |         - |        - |     7070 B |       0.001 |
| Quickenshtein     | 1,330.2 μs |   111.30 μs |   6.10 μs |  0.15 |    0.00 |         - |        - |        2 B |       0.000 |
| FuzzySharp        |   588.2 μs |    83.65 μs |   4.59 μs |  0.07 |    0.00 |         - |        - |     3041 B |       0.000 |
