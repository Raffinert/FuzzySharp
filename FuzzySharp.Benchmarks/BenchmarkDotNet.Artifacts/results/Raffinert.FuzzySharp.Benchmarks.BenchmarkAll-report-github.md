```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6332/22H2/2022Update)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.304
  [Host]   : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean        | Error        | StdDev      | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|-------------:|------------:|-------:|-------:|----------:|
| Ratio                                |    360.1 ns |    296.47 ns |    16.25 ns | 0.0153 |      - |     128 B |
| PartialRatio                         |  2,651.4 ns |    259.34 ns |    14.22 ns | 1.1902 |      - |    9968 B |
| TokenSortRatio                       |  1,082.4 ns |    175.09 ns |     9.60 ns | 0.0820 |      - |     696 B |
| PartialTokenSortRatio                |  4,907.0 ns |    683.39 ns |    37.46 ns | 2.1820 | 0.0076 |   18272 B |
| TokenSetRatio                        |  1,256.5 ns |    329.94 ns |    18.09 ns | 0.2537 |      - |    2136 B |
| PartialTokenSetRatio                 |  6,016.5 ns |  1,648.60 ns |    90.37 ns | 2.3956 |      - |   20064 B |
| WeightedRatio                        |  6,976.5 ns |  6,602.20 ns |   361.89 ns | 0.5646 |      - |    4768 B |
| TokenInitialismRatio1                |    220.9 ns |    118.72 ns |     6.51 ns | 0.0391 |      - |     328 B |
| TokenInitialismRatio2                |    232.2 ns |    112.80 ns |     6.18 ns | 0.0381 |      - |     320 B |
| TokenInitialismRatio3                |    281.2 ns |     60.16 ns |     3.30 ns | 0.0525 |      - |     440 B |
| PartialTokenInitialismRatio          |    687.5 ns |    196.35 ns |    10.76 ns | 0.2174 |      - |    1824 B |
| TokenAbbreviationRatio               |    883.8 ns |    193.59 ns |    10.61 ns | 0.2060 |      - |    1728 B |
| PartialTokenAbbreviationRatio        |  1,144.7 ns |  3,085.97 ns |   169.15 ns | 0.2747 |      - |    2312 B |
| RatioClassic                         |    353.8 ns |     79.25 ns |     4.34 ns | 0.0381 |      - |     320 B |
| PartialRatioClassic                  |  1,188.1 ns |    315.58 ns |    17.30 ns | 0.4025 |      - |    3368 B |
| TokenSortRatioClassic                |  1,662.4 ns |    350.00 ns |    19.18 ns | 0.2556 |      - |    2152 B |
| PartialTokenSortRatioClassic         |  1,742.9 ns |    327.56 ns |    17.95 ns | 0.2937 |      - |    2472 B |
| TokenSetRatioClassic                 |  2,301.8 ns |    230.46 ns |    12.63 ns | 0.5035 |      - |    4224 B |
| PartialTokenSetRatioClassic          |  2,634.0 ns |    970.43 ns |    53.19 ns | 0.6828 |      - |    5712 B |
| WeightedRatioClassic                 | 13,274.8 ns | 16,806.00 ns |   921.19 ns | 1.5259 |      - |   12769 B |
| TokenInitialismRatio1Classic         |    601.3 ns |     92.78 ns |     5.09 ns | 0.1078 |      - |     904 B |
| TokenInitialismRatio2Classic         |    557.3 ns |  2,102.62 ns |   115.25 ns | 0.0877 |      - |     736 B |
| TokenInitialismRatio3Classic         |  1,161.0 ns |    312.31 ns |    17.12 ns | 0.1850 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,266.9 ns |    129.69 ns |     7.11 ns | 0.2556 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,422.4 ns |     81.44 ns |     4.46 ns | 0.3567 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,634.0 ns |    379.42 ns |    20.80 ns | 0.4654 |      - |    3896 B |
| ExtractOne                           | 13,491.6 ns |    976.50 ns |    53.53 ns | 1.3275 |      - |   11184 B |
| ExtractOneClassic                    | 31,817.1 ns | 49,521.70 ns | 2,714.45 ns | 3.3264 |      - |   28002 B |
| FuzzySharpClassicDistance            |    834.2 ns |    143.53 ns |     7.87 ns | 0.0381 |      - |     320 B |
| FuzzySharpDistance                   |    592.4 ns |     98.07 ns |     5.38 ns | 0.0153 |      - |     128 B |
| FastenshteinDistance                 |    762.6 ns |     77.38 ns |     4.24 ns | 0.0172 |      - |     144 B |
| FuzzySharpDistanceFrom               |    185.8 ns |     31.42 ns |     1.72 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |  1,026.7 ns |    427.51 ns |    23.43 ns |      - |      - |         - |
| QuickenshteinDistance                |    672.3 ns |     75.19 ns |     4.12 ns |      - |      - |         - |
