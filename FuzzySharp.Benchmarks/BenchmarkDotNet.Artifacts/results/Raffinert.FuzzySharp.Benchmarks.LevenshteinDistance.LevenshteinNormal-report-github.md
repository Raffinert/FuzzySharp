```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.101
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error       | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Allocated  | Alloc Ratio |
|------------------ |-----------:|------------:|----------:|------:|--------:|----------:|---------:|-----------:|------------:|
| NaiveDp           | 9,463.2 μs | 8,455.64 μs | 463.48 μs |  1.00 |    0.06 | 1593.7500 | 203.1250 | 10012112 B |       1.000 |
| FuzzySharpClassic | 5,479.2 μs |   748.67 μs |  41.04 μs |  0.58 |    0.02 |   46.8750 |        - |   300048 B |       0.030 |
| Fastenshtein      | 4,542.1 μs | 1,550.30 μs |  84.98 μs |  0.48 |    0.02 |         - |        - |     7064 B |       0.001 |
| Quickenshtein     | 1,455.0 μs |   147.24 μs |   8.07 μs |  0.15 |    0.01 |         - |        - |        1 B |       0.000 |
| FuzzySharp        |   497.7 μs |    23.78 μs |   1.30 μs |  0.05 |    0.00 |         - |        - |     3203 B |       0.000 |
