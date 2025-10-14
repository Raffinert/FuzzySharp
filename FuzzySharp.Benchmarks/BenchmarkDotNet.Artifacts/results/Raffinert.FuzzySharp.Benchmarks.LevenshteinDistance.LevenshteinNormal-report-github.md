```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6332/22H2/2022Update)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.304
  [Host]   : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error        | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Allocated  | Alloc Ratio |
|------------------ |-----------:|-------------:|----------:|------:|--------:|----------:|---------:|-----------:|------------:|
| NaiveDp           | 9,217.0 μs |    975.21 μs |  53.45 μs |  1.00 |    0.01 | 1187.5000 | 140.6250 | 10012112 B |       1.000 |
| FuzzySharpClassic | 5,795.2 μs |  2,803.91 μs | 153.69 μs |  0.63 |    0.01 |   31.2500 |        - |   300048 B |       0.030 |
| Fastenshtein      | 5,140.5 μs | 15,656.86 μs | 858.21 μs |  0.56 |    0.08 |         - |        - |     7064 B |       0.001 |
| Quickenshtein     | 1,527.1 μs |    411.10 μs |  22.53 μs |  0.17 |    0.00 |         - |        - |          - |       0.000 |
| FuzzySharp        |   542.5 μs |     39.60 μs |   2.17 μs |  0.06 |    0.00 |         - |        - |     3200 B |       0.000 |
