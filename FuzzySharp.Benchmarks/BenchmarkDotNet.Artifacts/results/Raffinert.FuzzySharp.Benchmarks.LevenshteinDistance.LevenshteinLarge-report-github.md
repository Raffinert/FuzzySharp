```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.305
  [Host]   : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                     | Mean       | Error      | StdDev     | Ratio | RatioSD | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|--------------------------- |-----------:|-----------:|-----------:|------:|--------:|-----------:|-----------:|------------:|------------:|
| NaiveDp                    | 309.507 ms | 341.439 ms | 18.7154 ms |  1.00 |    0.07 | 32500.0000 | 25000.0000 | 275312720 B |       1.000 |
| NewMatchEngineEditDistance | 208.925 ms | 140.310 ms |  7.6909 ms |  0.68 |    0.04 |          - |          - |    864800 B |       0.003 |
| FuzzySharpClassic          | 162.702 ms |  18.751 ms |  1.0278 ms |  0.53 |    0.03 |          - |          - |   1545632 B |       0.006 |
| Fastenshtein               | 156.620 ms |  62.943 ms |  3.4501 ms |  0.51 |    0.03 |          - |          - |     33928 B |       0.000 |
| Quickenshtein              |  13.698 ms |   1.212 ms |  0.0664 ms |  0.04 |    0.00 |          - |          - |           - |       0.000 |
| FuzzySharp                 |   8.146 ms |   4.043 ms |  0.2216 ms |  0.03 |    0.00 |          - |          - |     51200 B |       0.000 |
