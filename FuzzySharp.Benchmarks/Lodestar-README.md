# Raffinert.FuzzySharp versus Lodestar.Fuzzy

This suite compares Raffinert.FuzzySharp **6.0.3**, library sources at
[`ac2e0a2`](https://github.com/Raffinert/FuzzySharp/commit/ac2e0a280bbed232174c38f18c8ed1df36ecc7bf),
with unmodified Lodestar sources at
[`52d30eae62e4222014d789d30dd0de8c5e07bb4d`](https://github.com/CyrilB1531/lodestar/tree/52d30eae62e4222014d789d30dd0de8c5e07bb4d).
That checkout declares Lodestar.Fuzzy 0.5.1, Lodestar.Text 0.7.1 and
Lodestar.Abstractions 0.2.1. These are source-build identities, not a claim that
these versions are published on NuGet. The runner builds the original projects
without modifying their algorithms or build configuration.

## Inputs and correctness

- The original Lodestar example uses two 43-character sentences:
  `the quick brown fox jumps over the lazy dog` and
  `the lazy dog jumps over the quick brown fox`.
- Deterministic seed `390039 + length`, lengths 8, 16, 32, 64, 65, 128, 256 and
  1024, and ASCII, Cyrillic and mixed BMP Unicode alphabets.
- Independent equal-length strings, exact embedded substrings, embedded
  substrings with one replacement, and disjoint alphabets. Exact/near substring
  haystacks have length `3 * pattern length + 4`.
- The verification matrix checks all 97 input pairs with Ratio, PartialRatio,
  TokenSetRatio and WeightedRatio/WRatio: 388 comparisons, tolerance `1e-9`.
  A difference stops the runner before measurement. Each measured setup repeats
  the check independently. Matching on this matrix does not prove universal
  semantic equivalence, particularly for the two weighted-ratio implementations.
- Both APIs use their default UTF-16 unit semantics; no text preprocessing is
  applied. Code-point/grapheme overloads, nulls and malformed UTF-16 are outside
  the matrix. Synthetic strings have no whitespace, so their token-set rows
  measure single-token inputs; the original sentences exercise token reordering.

The measured suite contains 11 representative input pairs covering all eight
lengths and all three alphabets, with four operations each (88 methods). Scenario
labels identify every input; the full verification matrix is larger than the
measured subset. Both sides include the shared operation-dispatch overhead.

Extraction additionally compares ExtractOne and top-5 over 100 and 1000 choices.
Both sides use Ratio, an identical candidate order, no normalization and cutoff
zero. The five best choices have unique scores and are deliberately non-perfect
so both scan the whole list.
Setup checks every candidate's score, the selected value/index and top-5 order.
Both results are fully materialized. These are sequential stateless comparisons;
Lodestar.Fuzzy at this revision does not expose a direct cached Fuzz-scorer API.

**Tie behavior differs:** on a seeded corpus with only one near-match and many
tied lower scores, top-5 rankings differed even when candidate scores matched.
The timed corpus uses five distinct top scores; it does not measure tie handling.
The runner saves a separate `ties.txt` probe rather than treating these APIs as
universally interchangeable. The probe is also available through
`--probe-lodestar-ties` on the comparison-enabled benchmark executable.

## Measurement protocol

BenchmarkDotNet 0.15.8, .NET 10.0.15, SDK 11.0.100, Release, Intel Core Ultra 9
275HX, Windows 11. Two process launches per method, five warmups and seven
measured iterations per launch, 200 ms target iteration time, MemoryDiagnoser,
no outlier removal. Logical CPU 10 is a P-core on this host (mask 1024).
`DOTNET_TieredCompilation=0` disables tier transitions and dynamic PGO. This
controlled profile is explicitly different from default-runtime performance.
All comparisons within a table use the same profile. Historical Classic tables
are not used as a performance baseline for this run.

Times are per operation, with BenchmarkDotNet's 99.9% confidence-interval
half-width. Ratios are Lodestar mean divided by Raffinert mean: values above one
favor Raffinert; values below one favor Lodestar. Allocation figures are per
operation. Small differences or broad intervals should not be treated as wins.

## Results

Measured on **2026-11-28**. All 388 score comparisons passed; extraction values,
scores and rankings matched on the timed corpus. All **96 measured methods**
completed successfully (88 scoring methods and 8 extraction methods).

Among 44 scoring pairs, the 99.9% confidence intervals are separated in favor of
Raffinert in **12** cases and Lodestar in **32**. Every mean's interval half-width
is below 5%. These are workload-specific comparisons, not an overall library
ranking. Extraction's tie-behavior difference is described above.

| Scenario | Operation | Raffinert (ns) | Lodestar (ns) | Lodestar / Raffinert | Allocated: Raffinert | Allocated: Lodestar |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Original | PartialRatio | 764.48 ± 6.80 | 564.47 ± 11.88 | 0.74× | 160 B | 0 B |
| Original | Ratio | 112.26 ± 0.96 | 57.01 ± 0.24 | 0.51× | 80 B | 0 B |
| Original | TokenSetRatio | 689.70 ± 3.21 | 585.84 ± 3.07 | 0.85× | 1664 B | 896 B |
| Original | WeightedRatio | 2,331.93 ± 17.14 | 715.20 ± 3.11 | 0.31× | 2816 B | 1120 B |
| 0008-ASCII-Near | PartialRatio | 236.41 ± 1.94 | 191.26 ± 1.17 | 0.81× | 256 B | 0 B |
| 0008-ASCII-Near | Ratio | 69.28 ± 1.82 | 34.54 ± 0.03 | 0.50× | 80 B | 0 B |
| 0008-ASCII-Near | TokenSetRatio | 322.48 ± 2.98 | 80.03 ± 0.79 | 0.25× | 752 B | 64 B |
| 0008-ASCII-Near | WeightedRatio | 766.79 ± 7.23 | 882.35 ± 6.66 | 1.15× | 1232 B | 184 B |
| 0016-Cyrillic-Equal | PartialRatio | 824.19 ± 5.87 | 353.21 ± 3.49 | 0.43× | 288 B | 0 B |
| 0016-Cyrillic-Equal | Ratio | 280.43 ± 1.60 | 237.95 ± 0.46 | 0.85× | 144 B | 0 B |
| 0016-Cyrillic-Equal | TokenSetRatio | 620.37 ± 4.91 | 329.59 ± 1.24 | 0.53× | 816 B | 64 B |
| 0016-Cyrillic-Equal | WeightedRatio | 1,290.49 ± 9.51 | 916.60 ± 5.58 | 0.71× | 1168 B | 64 B |
| 0032-Mixed-Exact | PartialRatio | 777.83 ± 4.02 | 723.40 ± 1.74 | 0.93× | 320 B | 0 B |
| 0032-Mixed-Exact | Ratio | 337.95 ± 6.37 | 313.72 ± 0.57 | 0.93× | 144 B | 0 B |
| 0032-Mixed-Exact | TokenSetRatio | 776.76 ± 10.58 | 530.62 ± 2.81 | 0.68× | 816 B | 64 B |
| 0032-Mixed-Exact | WeightedRatio | 2,432.34 ± 12.63 | 3,007.36 ± 5.66 | 1.24× | 1488 B | 376 B |
| 0064-ASCII-Near | PartialRatio | 1,277.96 ± 11.66 | 2,473.40 ± 11.26 | 1.94× | 256 B | 0 B |
| 0064-ASCII-Near | Ratio | 280.72 ± 1.52 | 167.45 ± 0.23 | 0.60× | 80 B | 0 B |
| 0064-ASCII-Near | TokenSetRatio | 648.05 ± 2.41 | 299.54 ± 1.39 | 0.46× | 752 B | 64 B |
| 0064-ASCII-Near | WeightedRatio | 2,667.48 ± 22.59 | 8,069.58 ± 25.21 | 3.03× | 1232 B | 632 B |
| 0065-ASCII-Exact | PartialRatio | 1,608.75 ± 4.78 | 7,185.08 ± 11.13 | 4.47× | 256 B | 0 B |
| 0065-ASCII-Exact | Ratio | 517.61 ± 1.48 | 242.38 ± 0.46 | 0.47× | 80 B | 0 B |
| 0065-ASCII-Exact | TokenSetRatio | 903.40 ± 11.56 | 377.05 ± 1.32 | 0.42× | 752 B | 64 B |
| 0065-ASCII-Exact | WeightedRatio | 3,731.95 ± 16.41 | 22,078.91 ± 109.98 | 5.92× | 1232 B | 640 B |
| 0065-Cyrillic-Absent | PartialRatio | 19,057.70 ± 109.76 | 770.09 ± 4.90 | 0.04× | 3480 B | 0 B |
| 0065-Cyrillic-Absent | Ratio | 832.78 ± 2.65 | 515.27 ± 0.72 | 0.62× | 144 B | 0 B |
| 0065-Cyrillic-Absent | TokenSetRatio | 1,311.88 ± 5.65 | 730.10 ± 5.29 | 0.56× | 816 B | 64 B |
| 0065-Cyrillic-Absent | WeightedRatio | 22,804.12 ± 105.72 | 3,205.09 ± 12.77 | 0.14× | 4648 B | 632 B |
| 0065-Cyrillic-Near | PartialRatio | 4,010.38 ± 11.32 | 9,584.68 ± 15.24 | 2.39× | 464 B | 0 B |
| 0065-Cyrillic-Near | Ratio | 1,063.08 ± 3.67 | 610.69 ± 2.67 | 0.57× | 144 B | 0 B |
| 0065-Cyrillic-Near | TokenSetRatio | 1,807.27 ± 8.32 | 1,077.91 ± 4.18 | 0.60× | 816 B | 64 B |
| 0065-Cyrillic-Near | WeightedRatio | 8,649.38 ± 43.30 | 29,910.06 ± 179.43 | 3.46× | 1632 B | 640 B |
| 0128-ASCII-Near | PartialRatio | 4,868.11 ± 17.30 | 27,043.04 ± 28.36 | 5.56× | 336 B | 0 B |
| 0128-ASCII-Near | Ratio | 965.35 ± 3.07 | 439.61 ± 0.39 | 0.46× | 80 B | 0 B |
| 0128-ASCII-Near | TokenSetRatio | 1,442.59 ± 4.64 | 683.57 ± 3.68 | 0.47× | 752 B | 1120 B |
| 0128-ASCII-Near | WeightedRatio | 8,580.24 ± 27.09 | 81,999.17 ± 292.93 | 9.56× | 1312 B | 2200 B |
| 0256-Mixed-Equal | PartialRatio | 10,949.13 ± 205.57 | 7,376.64 ± 6.65 | 0.67× | 576 B | 0 B |
| 0256-Mixed-Equal | Ratio | 1,743.85 ± 4.21 | 1,270.38 ± 7.29 | 0.73× | 144 B | 0 B |
| 0256-Mixed-Equal | TokenSetRatio | 2,704.50 ± 14.41 | 1,941.55 ± 10.48 | 0.72× | 816 B | 64 B |
| 0256-Mixed-Equal | WeightedRatio | 6,876.93 ± 45.69 | 4,504.56 ± 33.30 | 0.66× | 1168 B | 64 B |
| 1024-Mixed-Near | PartialRatio | 495,377.59 ± 1,962.73 | 7,334,581.03 ± 15,099.75 | 14.81× | 467 B | 0 B |
| 1024-Mixed-Near | Ratio | 58,981.70 ± 461.79 | 37,941.81 ± 188.37 | 0.64× | 144 B | 0 B |
| 1024-Mixed-Near | TokenSetRatio | 73,562.49 ± 386.68 | 44,751.95 ± 318.20 | 0.61× | 816 B | 8288 B |
| 1024-Mixed-Near | WeightedRatio | 780,457.98 ± 2,005.58 | 22,212,296.03 ± 152,789.01 | 28.46× | 1637 B | 16536 B |

### Extraction

| Candidates | Limit | Raffinert (ns) | Lodestar (ns) | Lodestar / Raffinert | Allocated: Raffinert | Allocated: Lodestar |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 100 | 1 | 11,773.00 ± 88.00 | 6,209.00 ± 7.50 | 0.53× | 8176 B | 32 B |
| 100 | 5 | 12,746.00 ± 152.70 | 6,657.00 ± 41.50 | 0.52× | 9104 B | 208 B |
| 1000 | 1 | 121,317.00 ± 1,154.10 | 62,971.00 ± 137.80 | 0.52× | 80176 B | 32 B |
| 1000 | 5 | 123,081.00 ± 1,393.20 | 63,762.00 ± 287.20 | 0.52× | 81104 B | 208 B |


## Reproduce

From the repository root in PowerShell:

```powershell
# Select an appropriate processor mask for your host; 1024 is this host's P-core 10.
./FuzzySharp.Benchmarks/Compare-Lodestar.ps1 -Affinity 1024
./FuzzySharp.Benchmarks/Compare-Lodestar.ps1 -Affinity 1024 -Filter '*LodestarExtractionBenchmarks*'

# Only build and compare all scores:
./FuzzySharp.Benchmarks/Compare-Lodestar.ps1 -VerifyOnly
```

The runner pins the Lodestar commit, restores dependencies, builds both libraries,
records the candidate commit and local diff summary, verifies scores, and runs
the selected benchmarks. Sources and reports are under ignored `artifacts/`.
Keep `run.txt`, `scores.csv` and `benchmarks/results/*` together. The optional
`FuzzySharpLodestarRoot` build property is inherited by BenchmarkDotNet's child
build; normal solution builds and package consumers have no Lodestar dependency.
