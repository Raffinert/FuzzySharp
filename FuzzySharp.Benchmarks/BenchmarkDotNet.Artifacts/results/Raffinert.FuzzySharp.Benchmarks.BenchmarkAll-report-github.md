```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean        | Error        | StdDev      | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|-------------:|------------:|-------:|-------:|----------:|
| Ratio                                |    234.5 ns |    538.04 ns |    29.49 ns | 0.0215 |      - |     136 B |
| PartialRatio                         |    576.5 ns |    698.69 ns |    38.30 ns | 0.0210 |      - |     136 B |
| TokenSortRatio                       |    724.5 ns |     50.53 ns |     2.77 ns | 0.1116 |      - |     704 B |
| PartialTokenSortRatio                |  1,452.7 ns |    708.00 ns |    38.81 ns | 0.1106 |      - |     704 B |
| TokenSetRatio                        |    992.4 ns |     91.48 ns |     5.01 ns | 0.3433 |      - |    2160 B |
| PartialTokenSetRatio                 |  2,048.8 ns |  1,020.98 ns |    55.96 ns | 0.3433 |      - |    2160 B |
| WeightedRatio                        |  5,356.5 ns |    458.98 ns |    25.16 ns | 0.7553 |      - |    4744 B |
| TokenInitialismRatio1                |    115.6 ns |     34.06 ns |     1.87 ns | 0.0534 |      - |     336 B |
| TokenInitialismRatio2                |    117.6 ns |     15.59 ns |     0.85 ns | 0.0522 |      - |     328 B |
| TokenInitialismRatio3                |    158.4 ns |     62.91 ns |     3.45 ns | 0.0713 |      - |     448 B |
| PartialTokenInitialismRatio          |    349.2 ns |    484.52 ns |    26.56 ns | 0.0710 |      - |     448 B |
| TokenAbbreviationRatio               |    812.8 ns |    294.83 ns |    16.16 ns | 0.2766 |      - |    1736 B |
| PartialTokenAbbreviationRatio        |    805.5 ns |    134.20 ns |     7.36 ns | 0.2766 |      - |    1736 B |
| RatioClassic                         |    242.3 ns |     32.53 ns |     1.78 ns | 0.0505 |      - |     320 B |
| PartialRatioClassic                  |  1,000.3 ns |    176.19 ns |     9.66 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  1,526.1 ns |    444.26 ns |    24.35 ns | 0.3223 |      - |    2024 B |
| PartialTokenSortRatioClassic         |  1,751.0 ns |  1,928.06 ns |   105.68 ns | 0.3719 |      - |    2344 B |
| TokenSetRatioClassic                 |  2,348.5 ns |  3,704.26 ns |   203.04 ns | 0.6523 |      - |    4096 B |
| PartialTokenSetRatioClassic          |  2,648.3 ns |  3,649.02 ns |   200.02 ns | 0.8888 |      - |    5584 B |
| WeightedRatioClassic                 | 10,649.2 ns |  1,561.18 ns |    85.57 ns | 1.8768 |      - |   11810 B |
| TokenInitialismRatio1Classic         |    538.2 ns |    172.29 ns |     9.44 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    478.5 ns |     19.82 ns |     1.09 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |  1,040.4 ns |    260.74 ns |    14.29 ns | 0.2460 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,196.0 ns |    325.05 ns |    17.82 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,384.5 ns |    223.43 ns |    12.25 ns | 0.4749 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,623.1 ns |    437.19 ns |    23.96 ns | 0.6199 |      - |    3896 B |
| ExtractOne                           | 11,882.5 ns | 18,957.21 ns | 1,039.11 ns | 1.7700 |      - |   11112 B |
| ExtractOneClassic                    | 24,317.6 ns | 42,980.65 ns | 2,355.91 ns | 4.2725 |      - |   26851 B |
| FuzzySharpClassicDistance            |    894.1 ns |  1,601.79 ns |    87.80 ns | 0.0505 |      - |     320 B |
| FuzzySharpDistance                   |    337.8 ns |    164.22 ns |     9.00 ns | 0.0215 |      - |     136 B |
| FastenshteinDistance                 |    693.7 ns |    451.12 ns |    24.73 ns |      - |      - |         - |
| FuzzySharpDistanceFrom               |    124.3 ns |     30.04 ns |     1.65 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |    736.6 ns |    273.42 ns |    14.99 ns |      - |      - |         - |
| QuickenshteinDistance                |    571.6 ns |     54.99 ns |     3.01 ns |      - |      - |         - |
