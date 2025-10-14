```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6332/22H2/2022Update)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.304
  [Host]   : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error       | StdDev    | Ratio | RatioSD | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------------ |-----------:|------------:|----------:|------:|--------:|---------:|-------:|----------:|------------:|
| NaiveDp           | 1,725.6 μs |    82.02 μs |   4.50 μs |  1.00 |    0.00 | 277.3438 | 7.8125 | 2335168 B |       1.000 |
| FuzzySharpClassic | 1,148.2 μs |   185.91 μs |  10.19 μs |  0.67 |    0.01 |  17.5781 |      - |  149792 B |       0.064 |
| Fastenshtein      |   912.5 μs |    42.46 μs |   2.33 μs |  0.53 |    0.00 |        - |      - |    3728 B |       0.002 |
| Quickenshtein     |   632.8 μs | 3,085.94 μs | 169.15 μs |  0.37 |    0.08 |        - |      - |         - |       0.000 |
| FuzzySharp        |   158.2 μs |    26.36 μs |   1.44 μs |  0.09 |    0.00 |   0.2441 |      - |    3200 B |       0.001 |
