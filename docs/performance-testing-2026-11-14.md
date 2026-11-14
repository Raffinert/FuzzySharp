# Performance testing and tuning plan — 2026-11-14

## Test setup

- Windows 11, .NET SDK 10.0.403, .NET runtime 10.0.14, x64 RyuJIT.
- BenchmarkDotNet 0.15.8, Release build, in-process toolchain. The configured out-of-process `ShortRun` harness could not build in this restricted environment, so its `NA` rows were excluded.
- The scorer run used BenchmarkDotNet's default iteration counts. The extraction confirmation used 3 warmups, 10 measured iterations, and a 100 ms target per iteration.
- These are single-host, fixed-input microbenchmarks. Differences of a few percent and behavior on other runtimes or input distributions need separate confirmation.

Reproduction: build `FuzzySharp.Benchmarks` in Release, then run its executable with `--inProcess --filter '*RatioBenchmarks.Ratio*' '*WeightedRatioBenchmarks.WeightedRatio*'`. The token-set run used `--filter '*TokenSetRatioBenchmarks.TokenSetRatio*' --iterationCount 3 --warmupCount 1 --iterationTime 100`. The extraction confirmation used `--filter '*ExtractSelectionFocusedBenchmarks.Optimized_ExtractOne' '*ExtractSelectionFocusedBenchmarks.Optimized_Parallel_ExtractOne' --iterationCount 10 --warmupCount 3 --iterationTime 100`. The repository's `Program.cs` also adds an out-of-process `ShortRun` job, whose failed rows should be ignored in this environment.

## Measured results

| Workload | Current library | Comparison | Allocation, current / comparison |
| --- | ---: | ---: | ---: |
| `Ratio` | 51.0 ns | classic FuzzySharp: 109.5 ns | 80 B / 320 B |
| `Ratio`, scorer reused across calls | 24.3 ns | stateless call: 51.0 ns | 0 B / 80 B |
| `Ratio`, new cached scorer per call | 69.8 ns | stateless call: 51.0 ns | 168 B / 80 B |
| `WeightedRatio` | 1.951 µs | classic FuzzySharp: 5.627 µs | 3.69 KB / 11.22 KB |
| `WeightedRatio`, scorer reused across calls | 1.336 µs | stateless call: 1.951 µs | 2.58 KB / 3.69 KB |
| `WeightedRatio`, new cached scorer per call | 2.425 µs | stateless call: 1.951 µs | 12.32 KB / 3.69 KB |
| `TokenSetRatio` | 329.8 ns | classic FuzzySharp: 1,488.4 ns | 1.46 KB / 3.88 KB |
| `TokenSetRatio`, scorer reused across calls | 320.8 ns | stateless call: 329.8 ns | 1.23 KB / 1.46 KB |

The token-set measurements used only three measured iterations and varied enough that the small time difference between cached and stateless scoring is inconclusive. Its allocation difference and the cost of constructing a cached scorer are clearer signals.

`ExtractSelectionFocusedBenchmarks` scored 100,000 choices with the benchmark's cheap scorer and four workers:

| Cutoff | Sequential `ExtractOne` | Parallel `ExtractOne` | Allocation, sequential / parallel |
| ---: | ---: | ---: | ---: |
| 0 | 715.3 µs | 429.0 µs | 80 B / 803,612 B |
| 95 | 743.4 µs | 393.3 µs | 111 B / 803,815 B |

Parallel results here are about 1.7–1.9 times faster but copy the input array into a list. This case does not establish a parallel crossover for smaller choice counts or expensive scorers.

## Prioritized changes to test

1. **Avoid per-call choice copies in parallel extraction.** `ResultExtractor.Parallel.cs` and its cached variant call `choices.ToList()` even for arrays and existing lists. Use an `IList<T>` or array fast path and materialize only other enumerables. The 100,000-choice benchmark allocates about 804 KB per parallel call, close to the size of the copied reference array. Benchmark arrays, lists, and lazy enumerables at 64, 1,000, and 100,000 choices; verify index and tie behavior.
2. **Short-circuit `WeightedRatio` when the base score already wins.** The unscaled score is calculated before the token and partial branches. For similar-length strings, the other branches cannot exceed 95; for the partial branch they cannot exceed 90. Return the base score at those bounds in both stateless and cached scorers. Add exact, near-exact, and low-similarity cases to quantify the affected input distribution. Keep the score identical and validate boundary cases.
3. **Reduce token-set allocation.** `TokenSetScorerBase` splits both inputs into strings, constructs two hash sets, sorts collections, and joins several temporary strings. The cached version also clones the first hash set per score. Prototype a read-only cached token representation with membership checks and a tokenization path that avoids unnecessary intermediate arrays. Measure allocation and runtime for repeated queries with 2, 8, and 32 tokens, including duplicates and Unicode whitespace. Preserve current sorting and duplicate semantics.
4. **Apply cached scorers only to repeated queries.** The existing benchmark shows that constructing cached `Ratio` and `WeightedRatio` scorers for a single call is slower and allocates more than stateless scoring. Document this crossover in the public guidance and include realistic query-reuse counts in benchmarks.
5. **Investigate parallel top-N memory separately.** Parallel `ExtractTop` materializes choices and creates a `double[choices.Count]` before the final heap selection. Try worker-local bounded heaps and a final merge to avoid the full score array. Benchmark both cheap and weighted scorers across list sizes and limits; preserve stable ties and cutoff behavior. This remains a code-based hypothesis until measured.

Run correctness tests after each implementation, then compare old and new versions with the same BenchmarkDotNet job and host. Record mean, spread, and bytes allocated; promote a change only when it improves a representative workload without a material regression elsewhere.

## Implementation follow-up

The tuning work applied the indexed-collection fast path, the weighted-ratio bound, and the cached token-set change. Parallel top-N now rents its score buffer and returns it after preserving the original score-order selection. A worker-local heap prototype failed the expanded tie-order test and was replaced.

| Workload | Before | After | Allocation before / after |
| --- | ---: | ---: | ---: |
| Parallel `ExtractOne`, 100,000 choices, cutoff 0 | 429.0 µs | 297.7 µs | 803,612 B / 2,544 B |
| Parallel `ExtractOne`, 100,000 choices, cutoff 95 | 393.3 µs | 254.0 µs | 803,815 B / 2,541 B |
| Reused cached `TokenSetRatio` | 320.8 ns | 202.6 ns | 1.23 KB / 976 B |
| Parallel top-N, 100,000 choices, cutoff 0 | 660.9 µs | 456.9 µs | 786.33 KB / 3.98 KB |
| Parallel top-N, 100,000 choices, cutoff 95 | 472.8 µs | 402.4 µs | 786.82 KB / 3.98 KB |

These before/after runs were separate in-process runs on the same host. The high-similarity weighted-ratio benchmark includes a full-evaluation baseline within the same run: exact inputs took 22.75 ns with the early exit versus 877.61 ns with all branches evaluated (80 B versus 2,464 B). One-character-different inputs took 36.78 ns versus 890.63 ns (80 B versus 2,776 B). Inputs below the bound still evaluate the existing branches.

On the existing lower-similarity weighted benchmark, stateless scoring measured 2.022 µs after the change versus 1.951 µs before; that difference is smaller than the run-to-run spread. Reused cached scoring measured 1.269 µs and 2.22 KB after versus 1.336 µs and 2.58 KB before. The cached allocation reduction is consistent with the token-set change.

Validation: 5,147 tests passed on both .NET 8 and .NET 10, including additional repeated-cache, tie-order, one-shot enumerable, and post-return mutation cases. The library built across all configured target frameworks. The existing netcoreapp3.1 dependency support warning remained.

PR review follow-up: parallel top-N now retains each selected value before returning its result iterator. Replacing an array element or clearing a list after the call no longer changes the scored results. A repeat in-process benchmark measured 553.7 µs and 3.95 KB at cutoff 0, and 404.1 µs and 3.96 KB at cutoff 95. The first cutoff-0 run after this fix was noisy, so these timings should be treated as indicative rather than a precise before/after comparison.

The pooled top-N path reduces garbage collection pressure but still uses a buffer proportional to the number of choices. More work on tokenization and score-buffer size should be driven by broader input distributions and runtime measurements.
