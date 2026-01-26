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
| Ratio                                |    215.9 ns |     74.36 ns |     4.08 ns | 0.0215 |      - |     136 B |
| PartialRatio                         |    535.3 ns |    177.34 ns |     9.72 ns | 0.0210 |      - |     136 B |
| TokenSortRatio                       |    689.3 ns |    206.17 ns |    11.30 ns | 0.1116 |      - |     704 B |
| PartialTokenSortRatio                |  1,396.4 ns |    169.09 ns |     9.27 ns | 0.1106 |      - |     704 B |
| TokenSetRatio                        |    989.6 ns |    516.62 ns |    28.32 ns | 0.3443 |      - |    2160 B |
| PartialTokenSetRatio                 |  1,978.6 ns |  1,130.70 ns |    61.98 ns | 0.3433 |      - |    2160 B |
| WeightedRatio                        |  5,205.4 ns |    638.30 ns |    34.99 ns | 0.7553 |      - |    4744 B |
| TokenInitialismRatio1                |    115.3 ns |     24.10 ns |     1.32 ns | 0.0535 |      - |     336 B |
| TokenInitialismRatio2                |    111.8 ns |     81.31 ns |     4.46 ns | 0.0522 |      - |     328 B |
| TokenInitialismRatio3                |    169.9 ns |    256.42 ns |    14.06 ns | 0.0713 |      - |     448 B |
| PartialTokenInitialismRatio          |    324.6 ns |    133.84 ns |     7.34 ns | 0.0710 |      - |     448 B |
| TokenAbbreviationRatio               |    740.9 ns |    100.13 ns |     5.49 ns | 0.2766 |      - |    1736 B |
| PartialTokenAbbreviationRatio        |    788.4 ns |    308.15 ns |    16.89 ns | 0.2766 |      - |    1736 B |
| RatioClassic                         |    241.3 ns |    243.95 ns |    13.37 ns | 0.0508 |      - |     320 B |
| PartialRatioClassic                  |  1,031.1 ns |    966.09 ns |    52.95 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  1,447.5 ns |     29.62 ns |     1.62 ns | 0.3166 |      - |    1992 B |
| PartialTokenSortRatioClassic         |  1,610.8 ns |  1,316.67 ns |    72.17 ns | 0.3719 |      - |    2344 B |
| TokenSetRatioClassic                 |  1,973.8 ns |      6.18 ns |     0.34 ns | 0.6523 |      - |    4096 B |
| PartialTokenSetRatioClassic          |  2,295.7 ns |    108.17 ns |     5.93 ns | 0.8888 |      - |    5584 B |
| WeightedRatioClassic                 | 10,215.0 ns |  1,556.91 ns |    85.34 ns | 1.8768 |      - |   11810 B |
| TokenInitialismRatio1Classic         |    517.4 ns |    331.45 ns |    18.17 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    422.7 ns |     89.43 ns |     4.90 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |    984.3 ns |    108.68 ns |     5.96 ns | 0.2460 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,161.2 ns |     65.47 ns |     3.59 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,280.0 ns |  1,027.15 ns |    56.30 ns | 0.4749 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,477.3 ns |     44.82 ns |     2.46 ns | 0.6199 |      - |    3896 B |
| ExtractOne                           | 11,357.7 ns | 25,407.46 ns | 1,392.67 ns | 1.7700 |      - |   11112 B |
| ExtractOneClassic                    | 21,436.8 ns | 12,252.42 ns |   671.60 ns | 4.2725 |      - |   26851 B |
| FuzzySharpClassicDistance            |    816.4 ns |    236.72 ns |    12.98 ns | 0.0505 |      - |     320 B |
| FuzzySharpDistance                   |    326.8 ns |     58.85 ns |     3.23 ns | 0.0215 |      - |     136 B |
| FastenshteinDistance                 |    995.6 ns |  2,066.03 ns |   113.25 ns |      - |      - |         - |
| FuzzySharpDistanceFrom               |    134.4 ns |    160.56 ns |     8.80 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |    787.2 ns |     79.49 ns |     4.36 ns |      - |      - |         - |
| QuickenshteinDistance                |    582.8 ns |    279.24 ns |    15.31 ns |      - |      - |         - |
