# Changelog

## v6.0.0

*Double-precision scoring and simplified distance APIs*

- **Breaking:** Removed `scoreCutoff` from the Indel, Levenshtein, LCS, and partial-ratio APIs.
- **Breaking:** Scorers, scoring strategies, `Fuzz` methods, and extracted-result scores now use `double` and preserve fractional similarity values.
- Extraction `cutoff` parameters now accept `double` values.
- Optimized pattern-match-vector construction with bulk character population, a dedicated single-block fast path, lazy non-ASCII storage, and direct dense-mask lookup for ASCII characters.

## v5.0.3

*Extractor selection performance and allocation improvements*

- Optimized `ExtractOne` and `ExtractTop` across the standard, cached, parallel, and parallel-cached extractors by selecting candidates directly instead of materializing every qualifying `ExtractedResult` and then using LINQ `Max` or `MaxN`.
- `ExtractOne` now tracks the best candidate during scoring; `ExtractTop` retains only the requested number of candidates in a bounded heap.
- Parallel extractors score candidates independently and reduce local results while preserving score ordering and first-occurrence tie-breaking.
- Added extractor selection regression tests and benchmarks for sequential, cached, parallel, and parallel-cached paths.

## v5.0.2

*Levenshtein/LCS correctness and cutoff semantics*

- Fixed `Levenshtein.Distance<T>` weighted dispatch so non-fast-path weights correctly use `GenericDistance` and return the computed value.
- Corrected `LevenshteinMaximum` length-difference math and aligned `Similarity` cutoff handling with RapidFuzz semantics by converting similarity cutoffs to distance cutoffs.
- Fixed `NormalizedSimilarity` cutoff flow to use the corresponding normalized-distance cutoff (`1 - similarityCutoff`).
- Made Levenshtein cutoff short-circuit strict-safe in both single- and multi-block paths using a remaining-length lower bound (`dist > cutoff + remaining`).
- Added Levenshtein tests for weighted cases, cutoff behavior, maximum formula, and recoverable-prefix cutoff safety.
- Updated multi-block LCS update logic and matrix construction to use the correct `u = S & U` transition form with consolidated per-block loops.

## v5.0.1

*LongestCommonSubsequence matrix correctness for long inputs*

- Fixed `LongestCommonSubsequence` matrix update logic for `s1.Length > 64` to match RapidFuzz LCSseq references.
- Corrected transition from using raw mask arithmetic (`S +/- U`) to the proper bit-parallel form based on `u = S & U`, then `S = (S + u) | (S - u)`.
- Refactored the multi-block add/sub computation into a single per-block loop while preserving carry/borrow behavior.
- This fixes incorrect early matrix rows (including the second-row failure case) and downstream reconstruction issues in `GetEditOps` and `MatchingBlocks` for long sequences.

## v5.0.0

*Breaking API changes and preprocessing fixes*

- **Breaking:** Replaced `PreprocessMode`-based APIs with `Func<string, string>` preprocessors across public scoring/extraction APIs. Calls that passed `PreprocessMode.Full`/`PreprocessMode.None` must now pass `StringPreprocessor.Full`/`StringPreprocessor.None` (or a custom delegate).
- **Breaking:** Renamed `PreProcess.StringPreprocessorFactory` to `PreProcess.StringPreprocessor`.
- **Breaking:** Renamed generic extraction methods from `Process.Extract*` to `Process.Extract*By` when an extractor delegate is provided (for example, `ExtractOne` -> `ExtractOneBy`, `ExtractTop` -> `ExtractTopBy`).
- Fixed missing preprocessing in several internal extraction/scoring paths so configured preprocessors are now applied consistently.

## v4.0.2

- Added `string query` overloads for generic choice extraction, allowing a plain string query to be matched against `IEnumerable<T>` choices with a `Func<T, string>` processor.
- Added `ExtractOne`, `ExtractSorted`, and `ExtractTop` support for this overload shape in the sequential and parallel result extractors.
- Extended the `Process` API for string-query generic choice extraction and added matching `ProcessPipeline` overloads for `ExtractTop` and `ExtractOne`.
- Added tests covering string-query generic choice extraction for `Process` and `ProcessPipeline`.

## v4.0.1
- Removed unnecessary abstractions CachedScorerBase and CachedStrategySensitiveScorerBase.

## v4.0.0

*Fluent Pipeline API, Caching, and Parallelism*

- **New fluent builder API** (`Process.Configure()`) for building reusable, immutable pipelines with preconfigured scoring, caching, and parallel execution.
- **Automatic caching** (`.Cached()`) creates a `CachedWeightedRatioScorer` per extraction call for improved performance.
- **External scorer caching** (`.Cached(scorer)`) enables across-run caching by reusing a pre-initialized `ICachedRatioScorer`.
- **Parallel execution** (`.Parallel()`) with configurable `ParallelOptions` including `CancellationToken` support.
- Builder methods are **order independent** — `.Cached().Parallel()` and `.Parallel().Cached()` produce identical results.
- **Type-safe builder design** — the type system prevents invalid configurations (e.g., passing an `IRatioScorer` to a cached pipeline).

## v3.0.9

Implemented RapidFuzz-style window pruning for the full-width partial ratio scan so it avoids evaluating every window when len2 > len1, and adjusted the suffix loop to cover the last full-width window.

## v3.0.8

Removed finalizers from CharMaskBuffer and DictionarySlimPooled as they don't use unmanaged resources.

## v3.0.7

Improved LongestCommonSequence for faster execution. This caused the PartialRatioStrategy speedup.

## v3.0.6

Optimized TokenInitialismRatio for faster execution and reduced memory allocations.

## v3.0.5

Return netstandard2.0 support, small code cleanup.

## v3.0.4

Remove unnecessary dependency, thanks to @laicasaane.

## v3.0.3

Fix partial ratio issue with empty strings, more tests.

## v3.0.1, v3.0.2

Fix critical issue with strings that contain more than 64 unique characters. The issue was introduced in v3.0.0.

## v3.0.0

*Partial Ratio Accuracy and Performance Update*

- **Fixes multiple bugs in the Partial Ratio implementation.** In earlier versions, `Fuzz.PartialRatio` could return suboptimal scores in certain cases (for example, when a short string appeared multiple times in a longer string, it didn't always pick the highest scoring match).
- **Performance Optimizations:** All distance calculations were rewritten to use bit-parallel algorithms. Additionally, the `Levenshtein.Instance`, `Indel.Instance` and `LongestCommonSequence.Instance` classes may help get max speedup — see [BenchmarkAll.FuzzySharpDistanceFrom](https://github.com/Raffinert/FuzzySharp/blob/master/FuzzySharp.Benchmarks/BenchmarkAll.cs#L213).
- Bit-parallel implementations are highly borrowed from the MIT-licensed Python library [RapidFuzz](https://github.com/rapidfuzz/RapidFuzz).

## v2.0.3

Accent to performance and allocations. See [Benchmark](https://github.com/Raffinert/FuzzySharp/blob/dc2b858dc4cc56d8cdf26411904e255a019b0549/FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.BenchmarkAll-report-github.md).
Support local languages more naturally (removed regexps "a-zA-Z"). All regexps were replaced with string manipulations (fixes [PR!7](https://github.com/JakeBayer/FuzzySharp/pull/7)).
Extra performance improvement, reused approach [Dmitry Sushchevsky](https://github.com/blowin) — see [PR!42](https://github.com/JakeBayer/FuzzySharp/pull/42).
Implemented new `Process.ExtractAll` method, see [Issue!46](https://github.com/JakeBayer/FuzzySharp/issues/46).
Remove support of outdated/vulnerable platforms netcoreapp2.0; netcoreapp2.1; netstandard1.6.

## v2.0.0

As of 2.0.0, all empty strings will return a score of 0. Prior, the partial scoring system would return a score of 100, regardless if the other input had correct value or not. This was a result of the partial scoring system returning an empty set for the matching blocks. As a result, this led to incorrect values in the composite scores; several of them (token set, token sort) relied on the prior value of empty strings.

As a result, many 1.X.X unit tests may be broken with the 2.X.X upgrade, but the upgrade to the 2.X.X series is recommended regardless, as it is closer to the ideal behavior of the library.
