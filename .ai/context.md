# FuzzySharp Technical Context (AI oriented)

This document is derived from the code bundle at `codefetch/codebase.md`. It is intended to give an AI agent enough context to work in this repo without deep reverse engineering.

## Scope and purpose
- Library: `Raffinert.FuzzySharp` provides fast fuzzy string matching and similarity scoring.
- Entry points: `Fuzz` (single comparisons), `Process` (matching against choice sets), and `Process.Configure()` (fluent builder for pipelines).
- Core algorithms: Indel, Levenshtein, LCS (LongestCommonSubsequence), and Partial Ratio, with bit-parallel optimizations.
- Performance: extensive use of pooled buffers, precomputed pattern vectors, cached scorers, and optional parallel execution.

## Repository map (high signal)
- `FuzzySharp/` core library
  - `Fuzz.cs` public scoring API
  - `Process.cs` extract/best-match static API + `Configure()` builder entry point
  - `ProcessBuilder.cs` fluent builders: `ProcessBuilder`, `CachedProcessBuilder`, `CachedScorerProcessBuilder`
  - `ProcessPipeline.cs` immutable pipelines: `ProcessPipeline`, `CachedScorerProcessPipeline`
  - `ProcessOptions.cs` internal option structs: `ProcessOptions`, `CachedScorerProcessOptions`
  - `ProcessExecutor.cs` internal dispatch (sequential/parallel/cached)
  - `CachedScorerProcessExecutor.cs` internal dispatch for pre-initialized cached scorer
  - `SimilarityRatio/` scoring strategy and scorer implementations
  - `PreProcess/` input normalization
  - `Extractor/` result extraction pipeline
    - `ResultExtractor.cs` sequential non-cached extraction
    - `ResultExtractor.Cached.cs` sequential cached extraction
    - `ResultExtractor.Parallel.cs` parallel non-cached extraction
    - `ResultExtractor.Parallel.Cached.cs` parallel cached extraction
  - `Edits/` edit operations and matching blocks
  - `Utils/` low-level data structures and performance helpers
- `FuzzySharp.Test/` NUnit tests
- `FuzzySharp.Benchmarks/` BenchmarkDotNet benchmarks
- `Directory.Build.props` imports SourceLink props

## Key namespaces
- `Raffinert.FuzzySharp` (public API, distance classes, builder, and pipeline types)
- `Raffinert.FuzzySharp.SimilarityRatio.*` (scorers and strategies)
- `Raffinert.FuzzySharp.Extractor` (ResultExtractor, ExtractedResult)
- `Raffinert.FuzzySharp.PreProcess` (PreprocessMode, StringPreprocessorFactory)
- `Raffinert.FuzzySharp.Utils` (PatternMatchVector, Polyfill, pooled dictionary, heap, etc.)

## Public API entry points

### `Fuzz` (static)
Location: `FuzzySharp/Fuzz.cs`

Purpose: one-off similarity scores for two strings. Each method has an overload that accepts `PreprocessMode`.

Algorithms exposed:
- `Ratio` (simple Indel-based similarity)
- `PartialRatio` (best alignment of shorter in longer)
- `TokenSortRatio`, `PartialTokenSortRatio`
- `TokenSetRatio`, `PartialTokenSetRatio`
- `TokenDifferenceRatio`, `PartialTokenDifferenceRatio`
- `TokenInitialismRatio`, `PartialTokenInitialismRatio`
- `TokenAbbreviationRatio`, `PartialTokenAbbreviationRatio`
- `WeightedRatio` (composite heuristic)

Implementation pattern: `Fuzz.*` methods fetch scorer singletons from `ScorerCache` and call `Score`.

### `Process` (static)
Location: `FuzzySharp/Process.cs`

Purpose: match a query against a set of choices and return scored results.

Key methods:
- `ExtractAll` (unsorted)
- `ExtractSorted` (descending by score)
- `ExtractTop` (top N)
- `ExtractOne` (best match)
- `Configure()` (returns `ProcessBuilder` for fluent pipeline construction)

Inputs:
- `query` (string or generic `T`)
- `choices` (IEnumerable)
- `processor` (maps T to string; defaults to `PreprocessMode.Full` for strings)
- `scorer` (`IRatioScorer` or `ICachedRatioScorer`)
- `cutoff` (minimum score)

Default behavior:
- Default scorer = `WeightedRatioScorer`
- Default string processor = `PreprocessMode.Full`

### `Process.Configure()` — Fluent Builder / Pipeline API
Location: `FuzzySharp/ProcessBuilder.cs`, `FuzzySharp/ProcessPipeline.cs`

Purpose: build immutable, reusable pipelines with baked-in scorer, caching, and parallelism settings.

#### Builder classes

**`ProcessBuilder`** (public sealed class)
- Entry point: `Process.Configure()` returns a new `ProcessBuilder`
- Methods:
  - `Parallel(ParallelOptions parallelOptions = null)` — enables parallel execution
  - `WithParallelOptions(ParallelOptions parallelOptions)` — sets options, implicitly enables parallel
  - `WithScorer(IRatioScorer scorer)` — sets scoring algorithm
  - `Cached()` — returns `CachedProcessBuilder` (auto-caching mode)
  - `Cached(ICachedRatioScorer scorer)` — returns `CachedScorerProcessBuilder` (external scorer mode)
  - `Build()` — produces `ProcessPipeline`

**`CachedProcessBuilder`** (public sealed class)
- Auto-caching mode: creates a `CachedWeightedRatioScorer` per call internally
- Methods:
  - `Parallel(ParallelOptions parallelOptions = null)`
  - `WithParallelOptions(ParallelOptions parallelOptions)`
  - `Build()` — produces `ProcessPipeline` (with `UseCaching=true` in options)

**`CachedScorerProcessBuilder`** (public sealed class)
- External scorer mode: caller provides an `ICachedRatioScorer` with the query already baked in
- Methods:
  - `Parallel(ParallelOptions parallelOptions = null)`
  - `WithParallelOptions(ParallelOptions parallelOptions)`
  - `Build()` — produces `CachedScorerProcessPipeline`

#### Pipeline structs

**`ProcessPipeline`** (public readonly struct)
- Immutable pipeline holding `ProcessOptions`
- Methods mirror `Process` static API: `ExtractAll`, `ExtractTop`, `ExtractSorted`, `ExtractOne`
- Each has string and generic overloads
- Delegates to `ProcessExecutor` internally

**`CachedScorerProcessPipeline`** (public readonly struct)
- Immutable pipeline holding `CachedScorerProcessOptions`
- Methods do NOT take a `query` parameter (scorer already has the query baked in)
- Same extract methods: `ExtractAll`, `ExtractTop`, `ExtractSorted`, `ExtractOne`
- Delegates to `CachedScorerProcessExecutor` internally

#### Internal support types

**`ProcessOptions`** (internal readonly struct)
- Properties: `UseParallel`, `ParallelOptions`, `Scorer` (`IRatioScorer`), `UseCaching`

**`CachedScorerProcessOptions`** (internal readonly struct)
- Properties: `UseParallel`, `ParallelOptions`, `CachedScorer` (`ICachedRatioScorer`)

**`ProcessExecutor`** (internal static class)
- Dispatches between sequential/parallel and cached/uncached paths
- When `UseCaching=true`: creates `CachedWeightedRatioScorer(processedQuery)` per call, delegates to `CachedScorerProcessExecutor`, and disposes the scorer
- When `UseParallel=true`: calls `ResultExtractor.Parallel.*`
- Otherwise: calls `ResultExtractor.*` (sequential)

**`CachedScorerProcessExecutor`** (internal static class)
- Dispatches between `ResultExtractor.Parallel.Cached.*` and `ResultExtractor.Cached.*`
- Takes `ICachedRatioScorer scorer` (no query parameter needed)

#### Builder call flow

```
Process.Configure() → ProcessBuilder
  ├── .Build() → ProcessPipeline → ProcessExecutor → ResultExtractor / ResultExtractor.Parallel
  ├── .Cached() → CachedProcessBuilder
  │     └── .Build() → ProcessPipeline (UseCaching=true) → ProcessExecutor → CachedScorerProcessExecutor
  └── .Cached(ICachedRatioScorer) → CachedScorerProcessBuilder
        └── .Build() → CachedScorerProcessPipeline → CachedScorerProcessExecutor → ResultExtractor.Cached / .Parallel.Cached
```

### Distance APIs
Location: `FuzzySharp/Indel.*.cs`, `FuzzySharp/Levenshtein.*.cs`, `FuzzySharp/LongestCommonSubsequence.*.cs`

Public types:
- `Indel` (static distance/similarity + cached instance class `Indel(string)`).
- `IndelT<T>` (generic companion, `IndelT<T>(T[] source) where T : IEquatable<T>`, with `DistanceFrom`, `NormalizedSimilarityWith`; `IDisposable`).
- `Levenshtein` (static distance/similarity + cached instance class `Levenshtein(string)`).
- `LongestCommonSubsequence` (static distance/similarity + cached instance class `LongestCommonSubsequence(string)`).
- `EditOp`, `MatchingBlock`, `OpCode` for edit/matching output.

These classes are independent of `Fuzz` and can be used directly.

## Scoring pipeline and call flow

Typical flow for `Fuzz.*`:

```
Fuzz.* -> ScorerCache -> IRatioScorer -> Strategy -> Core algorithm (Indel/LCS/Partial)
```

Typical flow for `Process.*` (static API):

```
Process.* -> ResultExtractor -> IRatioScorer or ICachedRatioScorer -> score per choice
```

Typical flow for `Process.Configure().*` (pipeline API):

```
Process.Configure() -> ProcessBuilder -> ProcessPipeline -> ProcessExecutor -> ResultExtractor (or .Parallel / .Cached / .Parallel.Cached)
```

Key implications:
- `ScorerCache` is a singleton factory that returns a single instance per scorer type.
- `ScorerBase` applies `PreprocessMode` when you call the overload with preprocessing.
- `Process` defaults to `PreprocessMode.Full` and `WeightedRatioScorer`.
- Pipelines bake in scorer, caching, and parallelism settings at build time.

## Preprocessing and tokenization

### `PreprocessMode` and `StringPreprocessorFactory`
Location: `FuzzySharp/PreProcess/*`

Modes:
- `Full`: lowercases, keeps only letters or digits, converts others to spaces, and trims.
- `None`: no preprocessing.

### String token utilities
Location: `FuzzySharp/Extensions/StringExtensions.cs`

Notable helpers:
- `ExtractTokens` splits on non-letter characters (letters only, not digits).
- `SplitByAnySpace` splits on whitespace.
- `GetSortedWords` splits by whitespace and sorts tokens.
- `NormalizeSpacesAndSort` joins sorted tokens by single spaces.
- `GetInitials` builds initialism from the first character of each whitespace-separated token.

## Scorers and strategies (string)

### Simple ratio scorers
Location: `SimilarityRatio/Scorer/StrategySensitive/Simple/*`

Implementation:
- `DefaultRatioScorer` -> `DefaultRatioStrategy.Calculate` -> `Indel.NormalizedSimilarity`.
- `PartialRatioScorer` -> `PartialRatioStrategy.Calculate` -> partial alignment.

### Token sort scorers
Location: `SimilarityRatio/Scorer/StrategySensitive/TokenSort/*`

Behavior:
- Sort tokens in each input (`NormalizeSpacesAndSort`), then score the sorted strings.
- `TokenSortScorer` uses default ratio; `PartialTokenSortScorer` uses partial ratio.

### Token set scorers
Location: `SimilarityRatio/Scorer/StrategySensitive/TokenSet/*`

Behavior:
- Tokenize to sets.
- Compute intersection and remainders.
- Build three comparison strings:
  - `sortedIntersection`
  - `sortedIntersection + tokens1 remainder`
  - `sortedIntersection + tokens2 remainder`
- Return the max of three scores.

### Token difference scorers
Location: `SimilarityRatio/Scorer/StrategySensitive/TokenDifference/*`

Behavior:
- Split into tokens, sort, then score arrays of strings (not chars).
- Default uses `DefaultRatioStrategy<string>` (Indel on token arrays).
- Partial variant uses `PartialRatioStrategy<string>`.

### Token initialism scorers
Location: `SimilarityRatio/Scorer/StrategySensitive/TokenInitialism/*`

Behavior:
- Identify longer and shorter input.
- Only attempt scoring if `longer.Length / shorter.Length >= 3`.
- Compute initials from longer and score initials vs shorter.

### Token abbreviation scorers
Location: `SimilarityRatio/Scorer/StrategySensitive/TokenAbbreviation/*`

Behavior:
- Ensure `shorter` and `longer` ordered by length.
- Only attempt if length ratio >= 1.5.
- Tokenize both strings (letters only). If shorter has > 4 tokens, return 0.
- Generate permutations of longer tokens, compare token-by-token if shorter token is an ordered subsequence of longer token.
- Score average per permutation and return max.

### Weighted ratio (composite)
Location: `SimilarityRatio/Scorer/Composite/WeightedRatioScorer.cs` and `SimilarityRatio/Scorer/Composite/CachedWeightedRatioScorer.cs`

Behavior:
- Always compute base `Ratio`.
- Compute `TokenSortRatio` and `TokenSetRatio` and scale by `UNBASE_SCALE = 0.95`.
- For large length differences, also consider partial ratios:
  - Enable partials only when `lenRatio >= 1.5`.
  - If `lenRatio > 8`, reduce partial scale to 0.6.
- Return the max of base vs scaled token/partial scores.

## Generic sequence support

Location: `SimilarityRatio/Scorer/Generic/*` and `SimilarityRatio/Strategy/Generic/*`

Purpose:
- Provide scorer/strategy implementations for `T[]` where `T : IEquatable<T>`.
- Used by token difference scorers to compare string token arrays.

Key types:
- `IRatioScorer<T>`, `ICachedRatioScorer<T>`, `ScorerBase<T>`
- `DefaultRatioStrategy<T>` and `PartialRatioStrategy<T>`

## Cached scoring

Entry points:
- `Process.Configure().Cached().Build()` for auto-caching (creates `CachedWeightedRatioScorer` per call).
- `Process.Configure().Cached(scorer).Build()` for external cached scorer.
- `CachedWeightedRatioScorer` for direct per-query caching.

Key cached components:
- `CachedDefaultRatioStrategy` caches `Indel` for a processed query string.
- `CachedDefaultRatioScorer` wraps a cached strategy.
- `CachedTokenSortScorer` and `CachedTokenSetScorer` cache token preprocessing for input1.

Disposal:
- Cached scorers often allocate pooled buffers or hold `PatternMatchVector`.
- Types implementing `IDisposable` must be disposed when created by callers.
- `ProcessExecutor` takes care of disposal for scorers it creates internally (auto-caching path).
- When using `CachedScorerProcessBuilder`, the caller owns the `ICachedRatioScorer` and must dispose it.

## Extractor pipeline

Location: `FuzzySharp/Extractor/*`

Key types:
- `ExtractedResult<T>` contains `Value`, `Score`, and `Index` (original position).
- `ResultExtractor` (partial class) implements extraction in four modes:

### Sequential non-cached (`ResultExtractor`)
- `ExtractWithoutOrder` yields matching items in input order, filtering by cutoff.
- `ExtractSorted` sorts by score descending.
- `ExtractTop` uses `MaxN` (min-heap) for top N selection, then reverses to highest-first.
- `ExtractOne` returns the max score.

### Sequential cached (`ResultExtractor.Cached`)
- Same methods but takes `ICachedRatioScorer` instead of `query + IRatioScorer`.

### Parallel non-cached (`ResultExtractor.Parallel`)
- Same method signatures as sequential but uses `System.Threading.Tasks.Parallel.ForEach`.
- Materializes choices to a list, creates result arrays, and filters/sorts after parallel scoring.
- Accepts `ParallelOptions` for thread control.

### Parallel cached (`ResultExtractor.Parallel.Cached`)
- Parallel execution with pre-initialized `ICachedRatioScorer`.
- Same pattern as parallel non-cached but uses `scorer.Score(processor(choice))`.

## Core distance algorithms

### Indel
Location: `FuzzySharp/Indel.*.cs`

Notes:
- Indel distance = `len(s1) + len(s2) - 2 * LCS(s1, s2)`.
- `NormalizedSimilarity` returns `1 - normalized distance`.
- Uses `PatternMatchVector` and `LongestCommonSubsequence` for bit-parallel computation.
- `IndelT<T>` provides generic sequence support (`T[] where T : IEquatable<T>`).

### Levenshtein
Location: `FuzzySharp/Levenshtein.*.cs`

Notes:
- Uses Myers bit-parallel algorithm.
- Dispatches to single-ulong (<= 64) or multi-ulong (> 64) paths.
- Supports custom insert/delete/replace costs; falls back to generic DP if costs are non-standard.
- Exposes edit ops and matching blocks (`GetEditOps`, `GetMatchingBlocks`).

### LongestCommonSubsequence (LCS)
Location: `FuzzySharp/LongestCommonSubsequence.*.cs`

Notes:
- Bit-parallel LCS for speed.
- Provides `Distance`, `Similarity`, normalized versions, `MatchingBlocks`, and `Opcodes`.

### Partial ratio
Location: `SimilarityRatio/Strategy/PartialRatioStrategy.cs` and `Strategy/Generic/PartialRatioStrategyT.cs`

Notes:
- Finds optimal alignment of shorter span within longer span.
- Uses `PatternMatchVector` and `Indel.BlockNormalizedSimilarity` to score windows.

## Performance and memory notes

Key primitives:
- `PatternMatchVector` and `PatternMatchVectorChar` use pooled buffers; `PatternMatchVectorChar` has an ASCII fast path.
- `DictionarySlimPooled` is a pooled dictionary used for pattern masks.
- `ArrayPool<T>` is heavily used to reduce allocations in Levenshtein and LCS.
- `SequenceUtils` trims common prefix/suffix and swaps to keep shorter inputs first.
- `Polyfill.PopCount` provides a cross-target popcount implementation (`BitOperations.PopCount` on NET6+, manual bit-hack otherwise).
- `Polyfill.ArrayFill` provides a cross-target array fill (`Array.Fill` on NETCOREAPP2+/NETSTANDARD2.1+, manual loop otherwise).

## Build, test, and benchmark

Targets from project files:
- Library (`FuzzySharp.csproj`): `netstandard2.0`, `netstandard2.1`, `netcoreapp3.1`, `net45`, `net46`, `net462`, `net472`, `net48`, `NET60`, `NET80`, `NET90`, `NET10.0`
- Tests (`FuzzySharp.Test.csproj`): `netframework4.6.2`, `netframework4.7.2`, `NET8.0`, `NET10.0`
- Benchmarks (`FuzzySharp.Benchmarks.csproj`): `NET10.0`

Typical commands:
```
dotnet build FuzzySharp.sln
dotnet test FuzzySharp.Test/FuzzySharp.Test.csproj
dotnet run --project FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj
```

## External dependencies (from csproj)

Library (`FuzzySharp/FuzzySharp.csproj`):
- `IndexRange` 1.0.3 (older frameworks)
- `System.Memory` 4.5.5 (older frameworks)
- `Meziantou.Polyfill` 1.0.49 (private assets, only for `CollectionExtensions.GetValueOrDefault`)

Tests (`FuzzySharp.Test/FuzzySharp.Test.csproj`):
- `nunit` 3.14.0
- `NUnit.Console` 3.20.1
- `NUnit3TestAdapter` 5.0.0
- `Microsoft.NET.Test.Sdk` 17.14.1
- `Quickenshtein` 1.5.1
- `FuzzySharp` 2.0.2 (package reference)

Benchmarks (`FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj`):
- `BenchmarkDotNet` 0.15.2
- `Fastenshtein` 1.0.10
- `Quickenshtein` 1.5.1
- `FuzzySharp` 2.0.2 (package reference)
- `Microsoft.VisualStudio.DiagnosticsHub.BenchmarkDotNetDiagnosers` 18.3.36812.1

## Notes and gotchas

- Default preprocessing differs by API:
  - `Fuzz.*` does no preprocessing unless you pass `PreprocessMode`.
  - `Process.*` defaults to `PreprocessMode.Full` for strings.
- `ExtractedResult<T>.CompareTo` compares only `Score`. Use `Index` for stable ordering if needed.
- `TokenAbbreviation` can be expensive due to permutations; it short-circuits when shorter has more than 4 tokens.
- Cached scorers (`CachedWeightedRatioScorer`, `CachedDefaultRatioStrategy`, `Indel`, `Levenshtein`, `LongestCommonSubsequence`) hold pooled buffers and must be disposed when created directly.
- `TokenInitialism` and `TokenAbbreviation` include length ratio checks; short strings can return 0.
- `Process.Cached` no longer exists as a nested class. Caching is accessed via the builder: `Process.Configure().Cached().Build()` or `Process.Configure().Cached(scorer).Build()`.
- When using `CachedScorerProcessPipeline`, the caller owns the `ICachedRatioScorer` lifetime. When using `ProcessPipeline` with `UseCaching=true`, the executor creates and disposes cached scorers internally.

## File-level starting points

If you need to change behavior or add features, start here:
- Public API: `FuzzySharp/Fuzz.cs`, `FuzzySharp/Process.cs`
- Builder/Pipeline API: `FuzzySharp/ProcessBuilder.cs`, `FuzzySharp/ProcessPipeline.cs`
- Internal dispatch: `FuzzySharp/ProcessExecutor.cs`, `FuzzySharp/CachedScorerProcessExecutor.cs`
- Composite behavior: `SimilarityRatio/Scorer/Composite/WeightedRatioScorer.cs`
- Core algorithms: `FuzzySharp/Indel.*.cs`, `FuzzySharp/Levenshtein.*.cs`, `FuzzySharp/LongestCommonSubsequence.*.cs`
- Token logic: `SimilarityRatio/Scorer/StrategySensitive/Token*/*`
- Extraction pipeline: `FuzzySharp/Extractor/ResultExtractor*.cs`
- Preprocessing: `FuzzySharp/PreProcess/StringPreprocessorFactory.cs`
