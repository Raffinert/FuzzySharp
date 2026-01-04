```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.101
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error      | StdDev     | Ratio | RatioSD | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|------------------ |-----------:|-----------:|-----------:|------:|--------:|-----------:|-----------:|------------:|------------:|
| NaiveDp           | 275.644 ms | 328.375 ms | 17.9993 ms |  1.00 |    0.08 | 43500.0000 | 34500.0000 | 275312720 B |       1.000 |
| FuzzySharpClassic | 172.922 ms |  60.489 ms |  3.3156 ms |  0.63 |    0.04 |          - |          - |   1545632 B |       0.006 |
| Fastenshtein      | 138.357 ms |  35.335 ms |  1.9368 ms |  0.50 |    0.03 |          - |          - |     33928 B |       0.000 |
| Quickenshtein     |  12.897 ms |   3.267 ms |  0.1791 ms |  0.05 |    0.00 |          - |          - |        64 B |       0.000 |
| FuzzySharp        |   6.589 ms |   1.923 ms |  0.1054 ms |  0.02 |    0.00 |          - |          - |      3337 B |       0.000 |
