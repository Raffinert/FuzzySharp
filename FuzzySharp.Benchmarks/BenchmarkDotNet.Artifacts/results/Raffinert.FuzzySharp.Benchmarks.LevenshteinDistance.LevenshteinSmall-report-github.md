```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-1255U 2.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.301
  [Host]   : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error     | StdDev   | Ratio | RatioSD | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------------ |-----------:|----------:|---------:|------:|--------:|---------:|-------:|----------:|------------:|
| NaiveDp           | 1,841.4 μs | 753.15 μs | 41.28 μs |  1.00 |    0.03 | 371.0938 | 9.7656 | 2335169 B |       1.000 |
| FuzzySharpClassic | 1,090.0 μs |  23.48 μs |  1.29 μs |  0.59 |    0.01 |  23.4375 |      - |  149793 B |       0.064 |
| Fastenshtein      |   860.4 μs |  80.93 μs |  4.44 μs |  0.47 |    0.01 |        - |      - |    3728 B |       0.002 |
| Quickenshtein     |   531.9 μs |  52.00 μs |  2.85 μs |  0.29 |    0.01 |        - |      - |       1 B |       0.000 |
| FuzzySharp        |   117.7 μs |  11.88 μs |  0.65 μs |  0.06 |    0.00 |   0.3662 |      - |    3040 B |       0.001 |
