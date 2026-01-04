```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.101
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error     | StdDev   | Ratio | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------------ |-----------:|----------:|---------:|------:|---------:|-------:|----------:|------------:|
| NaiveDp           | 1,898.5 μs | 385.79 μs | 21.15 μs |  1.00 | 371.0938 | 9.7656 | 2335168 B |       1.000 |
| FuzzySharpClassic | 1,123.4 μs | 191.99 μs | 10.52 μs |  0.59 |  23.4375 |      - |  149792 B |       0.064 |
| Fastenshtein      |   889.9 μs |  93.41 μs |  5.12 μs |  0.47 |        - |      - |    3728 B |       0.002 |
| Quickenshtein     |   505.2 μs |  99.17 μs |  5.44 μs |  0.27 |        - |      - |         - |       0.000 |
| FuzzySharp        |   147.6 μs |  23.79 μs |  1.30 μs |  0.08 |   0.4883 |      - |    3200 B |       0.001 |
