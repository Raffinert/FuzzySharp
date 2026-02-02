# FuzzySharp Technical Context (AI oriented)

This document is derived from the code bundle at `codefetch/projects/FuzzySharp.codebase.md`. It is intended to give an AI agent enough context to work in this repo without deep reverse engineering.

## Scope and purpose
- Library: `Raffinert.FuzzySharp` provides fast fuzzy string matching and similarity scoring.
- Entry points: `Fuzz` (single comparisons) and `Process` (matching against choice sets).
- Core algorithms: Indel, Levenshtein, LCS (LongestCommonSubsequence), and Partial Ratio, with bit-parallel optimizations.
- Performance: extensive use of pooled buffers, precomputed pattern vectors, and cached scorers.

## Repository map (high signal)
- `FuzzySharp/` core library
  - `Fuzz.cs` public scoring API
  - `Process.cs` and `Process.Cached.cs` extract/best-match APIs
  - `SimilarityRatio/` scoring strategy and scorer implementations
  - `PreProcess/` input normalization
  - `Extractor/` result extraction pipeline
  - `Edits/` edit operations and matching blocks
  - `Utils/` low-level data structures and performance helpers
- `FuzzySharp.Test/` NUnit tests
- `FuzzySharp.Benchmarks/` BenchmarkDotNet benchmarks
- `Directory.Build.props` imports SourceLink props

## Key namespaces
- `Raffinert.FuzzySharp` (public API and distance classes)
- `Raffinert.FuzzySharp.SimilarityRatio.*` (scorers and strategies)
- `Raffinert.FuzzySharp.Extractor` (ResultExtractor, ExtractedResult)
- `Raffinert.FuzzySharp.PreProcess` (PreprocessMode, StringPreprocessorFactory)
- `Raffinert.FuzzySharp.Utils` (PatternMatchVector, pooled dictionary, heap, etc.)

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
Location: `FuzzySharp/Process.cs` and `FuzzySharp/Process.Cached.cs`

Purpose: match a query against a set of choices and return scored results.

Key methods:
- `ExtractAll` (unsorted)
- `ExtractSorted` (descending by score)
- `ExtractTop` (top N)
- `ExtractOne` (best match)

Inputs:
- `query` (string or generic `T`)
- `choices` (IEnumerable)
- `processor` (maps T to string; defaults to `PreprocessMode.Full` for strings)
- `scorer` (`IRatioScorer` or `ICachedRatioScorer`)
- `cutoff` (minimum score)

Default behavior:
- Default scorer = `WeightedRatioScorer`
- Default string processor = `PreprocessMode.Full`

### `Process.Cached`
Purpose: optimized for repeated comparisons of many choices against one query.

Behavior:
- If a cached scorer is provided, it is used directly.
- Otherwise, creates a `CachedWeightedRatioScorer` for the (processed) query, uses it for all choices, and disposes it.

### Distance APIs
Location: `FuzzySharp/Indel.*.cs`, `FuzzySharp/Levenshtein.*.cs`, `FuzzySharp/LongestCommonSequence.*.cs`

Public types:
- `Indel` (static distance/similarity + cached instance class `Indel(string)`).
- `Levenshtein` (static distance/similarity + cached instance class `Levenshtein(string)`).
- `LongestCommonSubsequence` (static distance/similarity + cached instance class `LongestCommonSubsequence(string)`).
- `EditOp`, `MatchingBlock`, `OpCode` for edit/matching output.

These classes are independent of `Fuzz` and can be used directly.

## Scoring pipeline and call flow

Typical flow for `Fuzz.*`:

```
Fuzz.* -> ScorerCache -> IRatioScorer -> Strategy -> Core algorithm (Indel/LCS/Partial)
```

Typical flow for `Process.*`:

```
Process.* -> ResultExtractor -> IRatioScorer or ICachedRatioScorer -> score per choice
```

Key implications:
- `ScorerCache` is a singleton factory that returns a single instance per scorer type.
- `ScorerBase` applies `PreprocessMode` when you call the overload with preprocessing.
- `Process` defaults to `PreprocessMode.Full` and `WeightedRatioScorer`.

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
Location: `SimilarityRatio/Scorer/Composite/WeightedRatioScorer.cs`

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
- `Process.Cached` for choice sets.
- `CachedWeightedRatioScorer` for per-query caching.

Key cached components:
- `CachedDefaultRatioStrategy` caches `Indel` for a processed query string.
- `CachedDefaultRatioScorer` wraps a cached strategy.
- `CachedTokenSortScorer` and `CachedTokenSetScorer` cache token preprocessing for input1.

Disposal:
- Cached scorers often allocate pooled buffers or hold `PatternMatchVector`.
- Types implementing `IDisposable` must be disposed when created by callers.
- `Process.Cached` takes care of disposal only for the scorers it creates internally.

## Extractor pipeline

Location: `FuzzySharp/Extractor/*`

Key types:
- `ExtractedResult<T>` contains `Value`, `Score`, and `Index` (original position).
- `ResultExtractor` implements the extract operations.

Algorithms:
- `ExtractWithoutOrder` yields matching items in input order, filtering by cutoff.
- `ExtractSorted` sorts by score descending.
- `ExtractTop` uses `MaxN` (min-heap) for top N selection, then reverses to highest-first.
- `ExtractOne` returns the max score (ties resolved by `ExtractedResult.CompareTo` which compares only `Score`).

## Core distance algorithms

### Indel
Location: `FuzzySharp/Indel.*.cs`

Notes:
- Indel distance = `len(s1) + len(s2) - 2 * LCS(s1, s2)`.
- `NormalizedSimilarity` returns `1 - normalized distance`.
- Uses `PatternMatchVector` and `LongestCommonSequence` for bit-parallel computation.

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
- `NumericsPolyfill.PopCount` provides a cross-target popcount implementation.

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
- `Meziantou.Polyfill` 1.0.49 (private assets)

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
- Cached scorers (`CachedWeightedRatioScorer`, `CachedDefaultRatioStrategy`, `Indel`, `Levenshtein`, `LongestCommonSequence`) hold pooled buffers and must be disposed when created directly.
- `TokenInitialism` and `TokenAbbreviation` include length ratio checks; short strings can return 0.

## File-level starting points

If you need to change behavior or add features, start here:
- Public API: `FuzzySharp/Fuzz.cs`, `FuzzySharp/Process.cs`, `FuzzySharp/Process.Cached.cs`
- Composite behavior: `SimilarityRatio/Scorer/Composite/WeightedRatioScorer.cs`
- Core algorithms: `FuzzySharp/Indel.*.cs`, `FuzzySharp/Levenshtein.*.cs`, `FuzzySharp/LongestCommonSubsequence.*.cs`
- Token logic: `SimilarityRatio/Scorer/StrategySensitive/Token*/*`
- Preprocessing: `FuzzySharp/PreProcess/StringPreprocessorFactory.cs`
