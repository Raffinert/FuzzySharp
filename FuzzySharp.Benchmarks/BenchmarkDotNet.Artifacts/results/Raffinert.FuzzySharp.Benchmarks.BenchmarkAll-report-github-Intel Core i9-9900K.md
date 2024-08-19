```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4651/22H2/2022Update)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.303
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2


```
| Method                               | Mean         | Error      | StdDev     | Gen0   | Allocated |
|------------------------------------- |-------------:|-----------:|-----------:|-------:|----------:|
| Ratio1                               |    267.44 ns |   1.695 ns |   1.416 ns | 0.0124 |     104 B |
| Ratio2                               |     20.39 ns |   0.154 ns |   0.120 ns |      - |         - |
| PartialRatio                         |  1,085.36 ns |  19.909 ns |  15.544 ns | 0.2823 |    2376 B |
| TokenSortRatio                       |    843.50 ns |  11.472 ns |  10.731 ns | 0.0677 |     568 B |
| PartialTokenSortRatio                |    889.18 ns |   7.015 ns |   5.858 ns | 0.0868 |     728 B |
| TokenSetRatio                        |    925.99 ns |  15.906 ns |  14.100 ns | 0.2613 |    2200 B |
| PartialTokenSetRatio                 |  1,195.05 ns |   9.870 ns |   9.233 ns | 0.3834 |    3208 B |
| WeightedRatio                        |  9,102.34 ns | 115.783 ns | 102.639 ns | 0.5951 |    5072 B |
| TokenInitialismRatio1                |    169.70 ns |   1.928 ns |   1.610 ns | 0.0467 |     392 B |
| TokenInitialismRatio2                |    144.10 ns |   2.427 ns |   2.270 ns | 0.0410 |     344 B |
| TokenInitialismRatio3                |    308.37 ns |   6.002 ns |   7.371 ns | 0.0830 |     696 B |
| PartialTokenInitialismRatio          |    385.93 ns |   7.534 ns |  10.562 ns | 0.1383 |    1160 B |
| TokenAbbreviationRatio               |    704.29 ns |   7.898 ns |   7.002 ns | 0.1879 |    1576 B |
| PartialTokenAbbreviationRatio        |    859.54 ns |   4.423 ns |   3.694 ns | 0.2518 |    2112 B |
| Ratio1Classic                        |    375.43 ns |   2.449 ns |   2.045 ns | 0.0381 |     320 B |
| Ratio2Classic                        |     58.34 ns |   0.755 ns |   0.631 ns | 0.0238 |     200 B |
| PartialRatioClassic                  |  1,339.50 ns |  26.293 ns |  25.824 ns | 0.4025 |    3368 B |
| TokenSortRatioClassic                |  1,863.15 ns |  21.126 ns |  18.728 ns | 0.2632 |    2216 B |
| PartialTokenSortRatioClassic         |  1,978.23 ns |  38.822 ns |  38.128 ns | 0.3014 |    2536 B |
| TokenSetRatioClassic                 |  2,530.46 ns |  43.767 ns |  71.910 ns | 0.5188 |    4352 B |
| PartialTokenSetRatioClassic          |  2,820.74 ns |  26.962 ns |  25.221 ns | 0.6981 |    5840 B |
| WeightedRatioClassic                 | 14,791.53 ns | 131.690 ns | 109.967 ns | 1.6022 |   13481 B |
| TokenInitialismRatio1Classic         |    612.49 ns |   6.019 ns |   5.630 ns | 0.1078 |     904 B |
| TokenInitialismRatio2Classic         |    507.62 ns |   4.944 ns |   4.383 ns | 0.0877 |     736 B |
| TokenInitialismRatio3Classic         |  1,177.72 ns |   9.864 ns |   8.744 ns | 0.1850 |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,348.88 ns |  26.004 ns |  28.903 ns | 0.2556 |    2144 B |
| TokenAbbreviationRatioClassic        |  1,546.42 ns |  15.717 ns |  13.933 ns | 0.3567 |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,801.85 ns |  19.023 ns |  15.885 ns | 0.4654 |    3896 B |
| ExtractOne                           | 18,669.37 ns | 294.697 ns | 275.660 ns | 1.3733 |   11728 B |
| ExtractOneClassic                    | 32,787.78 ns | 301.223 ns | 267.027 ns | 3.4180 |   29010 B |
| LevenshteinDistance                  |  1,076.24 ns |   5.410 ns |   5.060 ns | 0.0172 |     144 B |
| FastenshteinDistance                 |    835.18 ns |   7.674 ns |   6.408 ns | 0.0172 |     144 B |

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4651/22H2/2022Update)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9256.0), X64 RyuJIT VectorSize=256
  DefaultJob : .NET Framework 4.8.1 (4.8.9256.0), X64 RyuJIT VectorSize=256


```
| Method                               | Mean         | Error      | StdDev     | Gen0   | Gen1   | Allocated |
|------------------------------------- |-------------:|-----------:|-----------:|-------:|-------:|----------:|
| Ratio1                               |    518.35 ns |   6.939 ns |   6.151 ns | 0.0162 |      - |     104 B |
| Ratio2                               |     62.31 ns |   0.548 ns |   0.458 ns |      - |      - |         - |
| PartialRatio                         |  1,640.96 ns |  28.143 ns |  30.113 ns | 0.3777 |      - |    2383 B |
| TokenSortRatio                       |  2,506.94 ns |   9.018 ns |   8.435 ns | 0.1259 |      - |     802 B |
| PartialTokenSortRatio                |  2,615.04 ns |  23.872 ns |  22.330 ns | 0.1526 |      - |     963 B |
| TokenSetRatio                        |  1,847.25 ns |  11.187 ns |  10.465 ns | 0.3643 |      - |    2295 B |
| PartialTokenSetRatio                 |  2,202.00 ns |  41.946 ns |  35.027 ns | 0.5226 |      - |    3306 B |
| WeightedRatio                        | 20,540.54 ns | 215.064 ns | 179.588 ns | 0.9155 |      - |    5914 B |
| TokenInitialismRatio1                |    365.77 ns |   2.138 ns |   1.785 ns | 0.1135 |      - |     714 B |
| TokenInitialismRatio2                |    305.47 ns |   4.044 ns |   3.377 ns | 0.0916 |      - |     578 B |
| TokenInitialismRatio3                |    628.01 ns |  11.784 ns |  10.447 ns | 0.2031 |      - |    1284 B |
| PartialTokenInitialismRatio          |    751.65 ns |   6.843 ns |   5.714 ns | 0.2775 | 0.0010 |    1749 B |
| TokenAbbreviationRatio               |  1,412.27 ns |  20.168 ns |  17.878 ns | 0.2995 |      - |    1886 B |
| PartialTokenAbbreviationRatio        |  1,749.55 ns |  20.166 ns |  17.876 ns | 0.3834 |      - |    2423 B |
| Ratio1Classic                        |    462.45 ns |   3.982 ns |   3.325 ns | 0.0505 |      - |     321 B |
| Ratio2Classic                        |    145.07 ns |   1.010 ns |   0.945 ns | 0.0317 |      - |     201 B |
| PartialRatioClassic                  |  1,654.72 ns |  30.734 ns |  27.245 ns | 0.5436 | 0.0019 |    3426 B |
| TokenSortRatioClassic                |  5,589.88 ns |  37.456 ns |  33.203 ns | 0.7019 |      - |    4461 B |
| PartialTokenSortRatioClassic         |  5,721.59 ns |  45.490 ns |  42.551 ns | 0.7553 |      - |    4790 B |
| TokenSetRatioClassic                 |  5,540.99 ns |  99.102 ns |  92.700 ns | 1.0223 |      - |    6467 B |
| PartialTokenSetRatioClassic          |  5,949.43 ns |  85.917 ns |  76.163 ns | 1.2665 |      - |    7983 B |
| WeightedRatioClassic                 | 33,760.56 ns | 102.732 ns |  96.095 ns | 3.7842 |      - |   24031 B |
| TokenInitialismRatio1Classic         |  2,418.18 ns |  33.691 ns |  29.867 ns | 0.3815 |      - |    2423 B |
| TokenInitialismRatio2Classic         |  2,055.07 ns |  12.397 ns |  10.990 ns | 0.3128 |      - |    1974 B |
| TokenInitialismRatio3Classic         |  4,806.24 ns |  37.174 ns |  34.772 ns | 0.7553 |      - |    4774 B |
| PartialTokenInitialismRatioClassic   |  4,915.96 ns |  18.358 ns |  16.274 ns | 0.8545 |      - |    5384 B |
| TokenAbbreviationRatioClassic        |  4,466.58 ns |  83.986 ns |  99.979 ns | 0.7782 |      - |    4903 B |
| PartialTokenAbbreviationRatioClassic |  5,030.78 ns |  98.552 ns | 105.449 ns | 0.9308 |      - |    5866 B |
| ExtractOne                           | 39,363.95 ns | 237.263 ns | 198.125 ns | 2.0752 |      - |   13271 B |
| ExtractOneClassic                    | 67,687.60 ns | 191.599 ns | 179.222 ns | 7.4463 |      - |   47404 B |
| LevenshteinDistance                  |  1,331.46 ns |   3.978 ns |   3.527 ns | 0.0229 |      - |     144 B |
| FastenshteinDistance                 |    961.98 ns |  14.555 ns |  12.154 ns | 0.0229 |      - |     144 B |