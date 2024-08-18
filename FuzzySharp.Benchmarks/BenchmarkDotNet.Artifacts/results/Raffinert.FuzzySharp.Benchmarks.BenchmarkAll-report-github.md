```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4037/23H2/2023Update/SunValley3)
12th Gen Intel Core i7-1255U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.400
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method                               | Mean         | Error        | StdDev       | Median       | Gen0   | Gen1   | Allocated |
|------------------------------------- |-------------:|-------------:|-------------:|-------------:|-------:|-------:|----------:|
| Ratio1                               |    206.81 ns |     2.409 ns |     2.136 ns |    207.29 ns | 0.0165 |      - |     104 B |
| Ratio2                               |     13.76 ns |     0.319 ns |     0.899 ns |     13.29 ns |      - |      - |         - |
| PartialRatio                         |    723.24 ns |    22.148 ns |    59.498 ns |    692.31 ns | 0.3786 | 0.0010 |    2376 B |
| TokenSortRatio                       |    801.65 ns |    54.000 ns |   159.219 ns |    882.82 ns | 0.0896 |      - |     568 B |
| PartialTokenSortRatio                |    899.87 ns |    30.597 ns |    89.254 ns |    921.35 ns | 0.1154 |      - |     728 B |
| TokenSetRatio                        |  1,093.68 ns |    28.071 ns |    80.993 ns |  1,096.79 ns | 0.3500 |      - |    2200 B |
| PartialTokenSetRatio                 |  1,380.95 ns |    52.967 ns |   154.507 ns |  1,392.58 ns | 0.5112 |      - |    3208 B |
| WeightedRatio                        | 12,561.44 ns |   767.193 ns | 2,225.766 ns | 13,232.62 ns | 0.7935 |      - |    5072 B |
| TokenInitialismRatio1                |    294.56 ns |     6.757 ns |    18.946 ns |    297.41 ns | 0.0625 |      - |     392 B |
| TokenInitialismRatio2                |    275.14 ns |     5.562 ns |    15.503 ns |    272.03 ns | 0.0548 |      - |     344 B |
| TokenInitialismRatio3                |    542.62 ns |    10.893 ns |    29.635 ns |    541.23 ns | 0.1106 |      - |     696 B |
| PartialTokenInitialismRatio          |    749.64 ns |    15.039 ns |    32.373 ns |    744.13 ns | 0.1845 |      - |    1160 B |
| TokenAbbreviationRatio               |  1,270.08 ns |    24.756 ns |    41.361 ns |  1,255.59 ns | 0.2508 |      - |    1576 B |
| PartialTokenAbbreviationRatio        |  1,536.55 ns |    45.771 ns |   129.097 ns |  1,561.22 ns | 0.3357 |      - |    2112 B |
| Ratio1Classic                        |    677.17 ns |    13.437 ns |    29.212 ns |    681.43 ns | 0.0505 |      - |     320 B |
| Ratio2Classic                        |    104.42 ns |     2.102 ns |     3.626 ns |    105.17 ns | 0.0318 |      - |     200 B |
| PartialRatioClassic                  |  2,249.40 ns |    44.588 ns |   118.242 ns |  2,274.26 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  3,071.78 ns |    92.892 ns |   266.524 ns |  3,143.59 ns | 0.3510 |      - |    2216 B |
| PartialTokenSortRatioClassic         |  3,317.62 ns |    64.881 ns |    82.054 ns |  3,327.15 ns | 0.4005 |      - |    2536 B |
| TokenSetRatioClassic                 |  4,309.09 ns |    85.081 ns |   184.959 ns |  4,337.85 ns | 0.6905 |      - |    4352 B |
| PartialTokenSetRatioClassic          |  4,771.35 ns |    92.361 ns |   230.012 ns |  4,849.64 ns | 0.9308 |      - |    5840 B |
| WeightedRatioClassic                 | 24,181.32 ns |   721.231 ns | 2,046.011 ns | 24,472.06 ns | 2.1362 |      - |   13482 B |
| TokenInitialismRatio1Classic         |  1,041.92 ns |    20.745 ns |    39.470 ns |  1,044.25 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    824.97 ns |    26.765 ns |    75.051 ns |    844.97 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |  1,971.98 ns |    39.316 ns |    91.901 ns |  1,989.39 ns | 0.2460 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  2,249.70 ns |    44.057 ns |    65.943 ns |  2,259.86 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  2,727.98 ns |    84.791 ns |   241.914 ns |  2,779.33 ns | 0.4730 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  3,162.92 ns |    88.249 ns |   247.460 ns |  3,193.32 ns | 0.6180 |      - |    3896 B |
| ExtractOne                           | 33,770.23 ns | 1,260.134 ns | 3,595.234 ns | 34,371.46 ns | 1.8616 |      - |   11728 B |
| ExtractOneClassic                    | 54,594.63 ns | 1,971.629 ns | 5,625.169 ns | 55,347.68 ns | 4.5776 |      - |   29011 B |
| LevenshteinDistance                  |  2,096.37 ns |    58.508 ns |   167.872 ns |  2,141.95 ns | 0.0229 |      - |     144 B |
| FastenshteinDistance                 |  1,533.82 ns |    38.323 ns |   108.715 ns |  1,564.52 ns | 0.0229 |      - |     144 B |
