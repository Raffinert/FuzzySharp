```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.101
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean        | Error       | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|------------:|----------:|-------:|-------:|----------:|
| Ratio                                |    388.5 ns | 1,136.19 ns |  62.28 ns | 0.0200 |      - |     128 B |
| PartialRatio                         |  2,607.4 ns | 3,614.98 ns | 198.15 ns | 1.5869 |      - |    9968 B |
| TokenSortRatio                       |    882.3 ns |   275.55 ns |  15.10 ns | 0.1106 |      - |     696 B |
| PartialTokenSortRatio                |  4,868.7 ns |   949.09 ns |  52.02 ns | 2.9068 | 0.0076 |   18272 B |
| TokenSetRatio                        |  1,335.0 ns |   525.48 ns |  28.80 ns | 0.3395 |      - |    2136 B |
| PartialTokenSetRatio                 |  6,094.1 ns | 5,722.86 ns | 313.69 ns | 3.1967 |      - |   20064 B |
| WeightedRatio                        |  6,620.3 ns | 1,842.75 ns | 101.01 ns | 0.7553 |      - |    4768 B |
| TokenInitialismRatio1                |    212.8 ns |    58.15 ns |   3.19 ns | 0.0522 |      - |     328 B |
| TokenInitialismRatio2                |    198.3 ns |    47.39 ns |   2.60 ns | 0.0508 |      - |     320 B |
| TokenInitialismRatio3                |    267.9 ns |   478.62 ns |  26.23 ns | 0.0701 |      - |     440 B |
| PartialTokenInitialismRatio          |    677.1 ns |    63.48 ns |   3.48 ns | 0.2899 |      - |    1824 B |
| TokenAbbreviationRatio               |    856.2 ns |   439.38 ns |  24.08 ns | 0.2747 |      - |    1728 B |
| PartialTokenAbbreviationRatio        |  1,201.7 ns | 2,338.88 ns | 128.20 ns | 0.3681 |      - |    2312 B |
| RatioClassic                         |    254.4 ns |    38.06 ns |   2.09 ns | 0.0505 |      - |     320 B |
| PartialRatioClassic                  |  1,032.9 ns |   263.34 ns |  14.43 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  1,709.1 ns |   658.39 ns |  36.09 ns | 0.3414 |      - |    2152 B |
| PartialTokenSortRatioClassic         |  1,750.6 ns |   576.74 ns |  31.61 ns | 0.3929 |      - |    2472 B |
| TokenSetRatioClassic                 |  2,320.4 ns |   363.94 ns |  19.95 ns | 0.6714 |      - |    4224 B |
| PartialTokenSetRatioClassic          |  2,652.7 ns | 2,332.74 ns | 127.87 ns | 0.9079 |      - |    5712 B |
| WeightedRatioClassic                 | 11,768.8 ns | 1,745.34 ns |  95.67 ns | 2.0294 |      - |   12770 B |
| TokenInitialismRatio1Classic         |    541.3 ns |   238.84 ns |  13.09 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    441.4 ns |   141.55 ns |   7.76 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |  1,064.5 ns | 1,128.52 ns |  61.86 ns | 0.2460 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,172.7 ns |   155.41 ns |   8.52 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,399.5 ns |   615.02 ns |  33.71 ns | 0.4749 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,718.5 ns | 1,322.21 ns |  72.47 ns | 0.6199 |      - |    3896 B |
| ExtractOne                           | 13,824.4 ns | 8,191.93 ns | 449.03 ns | 1.7700 |      - |   11184 B |
| ExtractOneClassic                    | 23,707.0 ns | 5,805.21 ns | 318.20 ns | 4.4556 |      - |   28003 B |
| FuzzySharpClassicDistance            |    944.4 ns |    45.50 ns |   2.49 ns | 0.0505 |      - |     320 B |
| FuzzySharpDistance                   |    565.6 ns |   117.29 ns |   6.43 ns | 0.0200 |      - |     128 B |
| FastenshteinDistance                 |    896.9 ns |   147.47 ns |   8.08 ns | 0.0229 |      - |     144 B |
| FuzzySharpDistanceFrom               |    167.5 ns |    30.73 ns |   1.68 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |    790.4 ns |    13.07 ns |   0.72 ns |      - |      - |         - |
| QuickenshteinDistance                |    898.5 ns | 3,012.57 ns | 165.13 ns |      - |      - |         - |
