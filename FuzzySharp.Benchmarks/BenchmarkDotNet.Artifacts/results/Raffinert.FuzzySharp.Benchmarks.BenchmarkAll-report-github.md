```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean        | Error       | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|------------:|----------:|-------:|-------:|----------:|
| Ratio                                |    213.9 ns |    58.61 ns |   3.21 ns | 0.0215 |      - |     136 B |
| PartialRatio                         |    545.4 ns |   424.18 ns |  23.25 ns | 0.0210 |      - |     136 B |
| TokenSortRatio                       |    679.4 ns |   163.29 ns |   8.95 ns | 0.1116 |      - |     704 B |
| PartialTokenSortRatio                |  1,420.6 ns |    30.80 ns |   1.69 ns | 0.1106 |      - |     704 B |
| TokenSetRatio                        |  1,189.4 ns | 2,832.04 ns | 155.23 ns | 0.3433 |      - |    2160 B |
| PartialTokenSetRatio                 |  1,956.7 ns |   577.85 ns |  31.67 ns | 0.3433 |      - |    2160 B |
| WeightedRatio                        |  5,202.8 ns | 3,172.26 ns | 173.88 ns | 0.7553 |      - |    4744 B |
| TokenInitialismRatio1                |    121.4 ns |   116.78 ns |   6.40 ns | 0.0535 |      - |     336 B |
| TokenInitialismRatio2                |    143.2 ns |   561.38 ns |  30.77 ns | 0.0522 |      - |     328 B |
| TokenInitialismRatio3                |    367.6 ns | 1,366.03 ns |  74.88 ns | 0.0713 |      - |     448 B |
| PartialTokenInitialismRatio          |    361.2 ns |   926.36 ns |  50.78 ns | 0.0710 |      - |     448 B |
| TokenAbbreviationRatio               |    868.6 ns | 1,423.17 ns |  78.01 ns | 0.2766 |      - |    1736 B |
| PartialTokenAbbreviationRatio        |    872.1 ns | 1,899.14 ns | 104.10 ns | 0.2766 |      - |    1736 B |
| RatioClassic                         |    239.3 ns |   204.66 ns |  11.22 ns | 0.0508 |      - |     320 B |
| PartialRatioClassic                  |  1,068.9 ns | 1,240.70 ns |  68.01 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  1,451.8 ns |    85.17 ns |   4.67 ns | 0.3223 |      - |    2024 B |
| PartialTokenSortRatioClassic         |  1,522.1 ns |   139.94 ns |   7.67 ns | 0.3719 |      - |    2344 B |
| TokenSetRatioClassic                 |  2,170.7 ns | 2,856.50 ns | 156.57 ns | 0.6523 |      - |    4096 B |
| PartialTokenSetRatioClassic          |  2,325.4 ns |   665.98 ns |  36.50 ns | 0.8888 |      - |    5584 B |
| WeightedRatioClassic                 |  9,954.2 ns | 1,939.07 ns | 106.29 ns | 1.8768 |      - |   11810 B |
| TokenInitialismRatio1Classic         |    506.8 ns |    52.97 ns |   2.90 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    413.6 ns |   103.28 ns |   5.66 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |    962.0 ns |    62.13 ns |   3.41 ns | 0.2460 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,326.6 ns | 6,791.80 ns | 372.28 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,369.2 ns | 3,523.37 ns | 193.13 ns | 0.4749 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,493.6 ns |    62.67 ns |   3.44 ns | 0.6199 |      - |    3896 B |
| ExtractOne                           |  9,928.4 ns | 1,168.00 ns |  64.02 ns | 1.7700 |      - |   11112 B |
| ExtractOneClassic                    | 20,780.6 ns | 1,049.20 ns |  57.51 ns | 4.2725 |      - |   26851 B |
| FuzzySharpClassicDistance            |    832.9 ns |   218.91 ns |  12.00 ns | 0.0505 |      - |     320 B |
| FuzzySharpDistance                   |    315.7 ns |    39.58 ns |   2.17 ns | 0.0215 |      - |     136 B |
| FastenshteinDistance                 |    785.4 ns |   126.78 ns |   6.95 ns |      - |      - |         - |
| FuzzySharpDistanceFrom               |    127.7 ns |    40.65 ns |   2.23 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |    727.6 ns |   216.96 ns |  11.89 ns |      - |      - |         - |
| QuickenshteinDistance                |    646.4 ns |   180.36 ns |   9.89 ns |      - |      - |         - |
