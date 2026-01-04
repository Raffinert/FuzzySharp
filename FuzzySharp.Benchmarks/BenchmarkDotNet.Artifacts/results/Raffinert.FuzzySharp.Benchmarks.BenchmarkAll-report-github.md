```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-1255U 2.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.301
  [Host]   : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean        | Error         | StdDev       | Median      | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|--------------:|-------------:|------------:|-------:|-------:|----------:|
| Ratio                                |    262.0 ns |     164.34 ns |      9.01 ns |    260.3 ns | 0.0191 |      - |     120 B |
| PartialRatio                         |  2,186.5 ns |   2,186.50 ns |    119.85 ns |  2,173.6 ns | 1.5869 |      - |    9960 B |
| TokenSortRatio                       |    869.5 ns |   1,864.63 ns |    102.21 ns |    858.8 ns | 0.1087 |      - |     688 B |
| PartialTokenSortRatio                |  4,660.6 ns |   5,317.95 ns |    291.49 ns |  4,567.8 ns | 2.9068 | 0.0076 |   18264 B |
| TokenSetRatio                        |  1,242.0 ns |   1,435.29 ns |     78.67 ns |  1,221.8 ns | 0.3910 |      - |    2464 B |
| PartialTokenSetRatio                 |  5,222.5 ns |   4,743.26 ns |    259.99 ns |  5,223.6 ns | 3.2501 |      - |   20392 B |
| WeightedRatio                        |  5,414.7 ns |   2,906.83 ns |    159.33 ns |  5,489.6 ns | 0.8240 |      - |    5184 B |
| TokenInitialismRatio1                |    220.9 ns |     121.77 ns |      6.67 ns |    217.5 ns | 0.0815 |      - |     512 B |
| TokenInitialismRatio2                |    265.9 ns |     869.88 ns |     47.68 ns |    259.5 ns | 0.0739 |      - |     464 B |
| TokenInitialismRatio3                |    485.0 ns |   2,062.09 ns |    113.03 ns |    466.5 ns | 0.1297 |      - |     816 B |
| PartialTokenInitialismRatio          |    723.0 ns |   1,349.58 ns |     73.98 ns |    760.4 ns | 0.3500 |      - |    2200 B |
| TokenAbbreviationRatio               |    694.1 ns |     106.14 ns |      5.82 ns |    696.2 ns | 0.2737 |      - |    1720 B |
| PartialTokenAbbreviationRatio        |  1,080.5 ns |   4,773.68 ns |    261.66 ns |  1,016.2 ns | 0.3672 | 0.0010 |    2304 B |
| RatioClassic                         |    276.9 ns |      85.32 ns |      4.68 ns |    278.7 ns | 0.0505 |      - |     320 B |
| PartialRatioClassic                  |  1,172.2 ns |   1,136.42 ns |     62.29 ns |  1,179.3 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  1,462.9 ns |   1,100.31 ns |     60.31 ns |  1,437.7 ns | 0.3414 |      - |    2152 B |
| PartialTokenSortRatioClassic         |  1,693.2 ns |   3,658.44 ns |    200.53 ns |  1,601.1 ns | 0.3929 |      - |    2472 B |
| TokenSetRatioClassic                 |  2,261.7 ns |   3,267.86 ns |    179.12 ns |  2,178.5 ns | 0.6714 |      - |    4224 B |
| PartialTokenSetRatioClassic          |  2,710.2 ns |   3,688.19 ns |    202.16 ns |  2,596.2 ns | 0.9079 |      - |    5712 B |
| WeightedRatioClassic                 | 11,356.2 ns |   8,422.49 ns |    461.66 ns | 11,256.2 ns | 2.0294 |      - |   12770 B |
| TokenInitialismRatio1Classic         |    485.8 ns |     337.46 ns |     18.50 ns |    477.7 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    425.6 ns |     388.93 ns |     21.32 ns |    415.8 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |    920.9 ns |     583.42 ns |     31.98 ns |    926.2 ns | 0.2470 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,249.5 ns |   1,351.62 ns |     74.09 ns |  1,270.7 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,380.1 ns |   1,054.49 ns |     57.80 ns |  1,373.7 ns | 0.4749 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,578.8 ns |   1,061.68 ns |     58.19 ns |  1,558.4 ns | 0.6199 |      - |    3896 B |
| ExtractOne                           | 12,119.2 ns |  10,269.77 ns |    562.92 ns | 11,880.5 ns | 1.9379 |      - |   12208 B |
| ExtractOneClassic                    | 34,581.9 ns | 335,434.41 ns | 18,386.29 ns | 24,937.2 ns | 4.4556 |      - |   28003 B |
| FuzzySharpClassicDistance            |  1,211.0 ns |   3,749.37 ns |    205.52 ns |  1,152.3 ns | 0.0496 |      - |     320 B |
| FuzzySharpDistance                   |    449.8 ns |     439.37 ns |     24.08 ns |    436.9 ns | 0.0191 |      - |     120 B |
| FastenshteinDistance                 |    672.1 ns |     182.02 ns |      9.98 ns |    668.2 ns | 0.0229 |      - |     144 B |
| FuzzySharpDistanceFrom               |    125.8 ns |     112.56 ns |      6.17 ns |    123.8 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |    810.2 ns |   1,648.46 ns |     90.36 ns |    805.1 ns |      - |      - |         - |
| QuickenshteinDistance                |    619.5 ns |     145.74 ns |      7.99 ns |    617.7 ns |      - |      - |         - |
