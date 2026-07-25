# AI Agent Instructions: Complete Approximate Substring Matching Feature

## Objective

Complete the approximate substring matching feature in `Raffinert/FuzzySharp` using the existing bit-parallel **IndelNew** implementation as the algorithmic foundation.

The feature must expose a production-ready fuzzy score that finds how closely the shorter input matches **any substring** of the longer input, while preserving the existing behavior of `Fuzz.PartialRatio`.

Do not replace or silently change `PartialRatio`, `WeightedRatio`, token scorers, or any existing public behavior.

---

## Existing baseline

The working tree already contains an implementation equivalent to:

```csharp
public readonly struct IndelSubstringMatch : IEquatable<IndelSubstringMatch>
{
    public int Distance { get; }
    public int EndIndex { get; }
    public bool Found => EndIndex >= 0;
}

public sealed partial class Indel
{
    public static IndelSubstringMatch BestSubstringMatch<T>(
        ReadOnlySpan<T> pattern,
        ReadOnlySpan<T> text)
        where T : notnull, IEquatable<T>;

    internal static IndelSubstringMatch BestSubstringMatchImpl<T>(
        IPatternMatchVector<T> patternVector,
        ReadOnlySpan<T> text)
        where T : notnull, IEquatable<T>;
}
```

The implementation already contains:

- a single-`ulong` path for patterns up to 64 elements;
- a multi-block path for longer patterns;
- `PatternMatchVector<T>` integration;
- pooled buffers for the multi-block implementation;
- the original approximate-substring DP boundary `D[0, j] = 0`;
- early exit when an exact match is found;
- an endpoint result through `EndIndex`.

Treat this implementation as the starting point. Do not replace it with brute-force window enumeration or ordinary `O(mn)` dynamic programming. A scalar DP implementation may be added only as a test oracle.

---

## Algorithm semantics

For a non-empty pattern `P` and text `T`, the low-level operation finds:

```text
min distance(P, S)
```

where `S` is a substring candidate represented by the original IndelNew approximate-substring recurrence.

The edit model is Indel distance:

- insertion cost: `1`;
- deletion cost: `1`;
- substitution cost: `2` because it is one deletion plus one insertion.

The algorithm tracks the best distance for substrings ending at each text position and returns:

- `Distance`: the smallest raw Indel distance encountered;
- `EndIndex`: the zero-based text index where the first strictly better best match ended;
- `Found`: `true` when a text endpoint improved on deleting the complete pattern.

Preserve the current tie behavior:

- update the best result only when `currentDistance < bestDistance`;
- therefore equal-distance later matches do not replace the earlier result;
- return immediately on distance `0`, since no better result is possible.

### Empty inputs

Preserve these low-level semantics:

```text
pattern empty                  => Distance = 0, EndIndex = -1
text empty, pattern non-empty  => Distance = pattern.Length, EndIndex = -1
```

The fuzzy ratio API has separate empty-input semantics defined below.

---

## Public feature name

Use **Approximate Substring Ratio** consistently.

Required public API:

```csharp
Fuzz.ApproximateSubstringRatio(string input1, string input2)
Fuzz.ApproximateSubstringRatio(
    string input1,
    string input2,
    Func<string, string> preprocessor)
```

Required scorer type:

```csharp
ApproximateSubstringRatioScorer
```

Required cached scorer type:

```csharp
CachedApproximateSubstringRatioScorer
```

Do not call the new API `PartialRatio`. It has deliberately different semantics from RapidFuzz/FuzzyWuzzy partial ratio.

---

## Score definition

The scorer must choose the shorter processed input as the pattern and the longer processed input as the text.

For a non-empty pattern of length `m` and best raw distance `d`, calculate:

```text
similarity = 1 - d / m
score      = round(100 * similarity)
```

Equivalent C#:

```csharp
double similarity = 1.0 - match.Distance / (double)pattern.Length;
int score = (int)Math.Round(100.0 * similarity);
```

Clamp defensively to `[0, 100]` even though a correct IndelNew result should already produce a distance in `[0, pattern.Length]` for this use case.

### Fuzzy ratio empty-input behavior

Use the same user-facing convention as other fuzzy scorers:

```text
both inputs empty  => 100
one input empty    => 0
```

### Equal-length inputs

Approximate substring matching is directional because the text side has the free-start boundary.

When the processed inputs have the same non-zero length:

1. calculate `input1` as pattern against `input2` as text;
2. calculate `input2` as pattern against `input1` as text;
3. return the larger score.

This makes the public scorer symmetric and follows the approach already used by the existing partial-ratio strategy for equal-length inputs.

### Important non-goal

Do not attempt to reproduce the exact numeric result of RapidFuzz `partial_ratio`.

This feature minimizes raw approximate-substring Indel distance and normalizes it by pattern length. It is a separate metric.

---

## Required architecture

Follow the current repository layers and naming conventions.

### 1. Low-level Indel implementation

Keep or move the current implementation into a focused file such as:

```text
FuzzySharp/Indel.ApproximateSubstring.cs
```

Keep these methods:

```csharp
public static IndelSubstringMatch BestSubstringMatch<T>(
    ReadOnlySpan<T> pattern,
    ReadOnlySpan<T> text)
    where T : notnull, IEquatable<T>;

internal static IndelSubstringMatch BestSubstringMatchImpl<T>(
    IPatternMatchVector<T> patternVector,
    ReadOnlySpan<T> text)
    where T : notnull, IEquatable<T>;
```

The internal overload is required for the cached scorer so the pattern mask is constructed only once.

Keep `IndelSubstringMatch` compatible with all library targets. Do not use `record struct` or APIs unavailable on `netstandard2.0`/legacy .NET Framework targets.

A regular `readonly struct` with explicit equality is acceptable.

### 2. Generic strategy

Add:

```text
FuzzySharp/SimilarityRatio/Strategy/Generic/ApproximateSubstringRatioStrategyT.cs
```

Suggested shape:

```csharp
internal static class ApproximateSubstringRatioStrategy<T>
    where T : notnull, IEquatable<T>
{
    public static int Calculate(
        ReadOnlySpan<T> input1,
        ReadOnlySpan<T> input2);
}
```

Responsibilities:

- handle empty spans;
- choose the shorter span as pattern;
- call `Indel.BestSubstringMatch`;
- normalize by pattern length;
- evaluate both directions for equal lengths;
- return an integer in `[0, 100]`.

Do not put string preprocessing in the generic strategy.

### 3. String strategy wrapper

Add:

```text
FuzzySharp/SimilarityRatio/Strategy/ApproximateSubstringRatioStrategy.cs
```

Suggested shape:

```csharp
internal static class ApproximateSubstringRatioStrategy
{
    public static int Calculate(string input1, string input2);
}
```

Delegate to `ApproximateSubstringRatioStrategy<char>.Calculate` using spans.

### 4. Non-cached scorer

Add:

```text
FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/ApproximateSubstringRatioScorer.cs
```

Follow `PartialRatioScorer` and other simple scorer conventions.

Suggested shape:

```csharp
public sealed class ApproximateSubstringRatioScorer : SimpleRatioScorerBase
{
    protected override FuzzySharp.Scorer Scorer =>
        ApproximateSubstringRatioStrategy.Calculate;
}
```

### 5. Cached strategy

Add a cached strategy that owns one precomputed `PatternMatchVector<char>` for the processed query.

Suggested file:

```text
FuzzySharp/SimilarityRatio/Strategy/CachedApproximateSubstringRatioStrategy.cs
```

It should implement the repository's existing `ICachedStrategy` abstraction.

Required behavior:

- apply the configured preprocessor to the query once in the constructor;
- retain the processed query string and its length;
- build and own the query `PatternMatchVector<char>` once;
- apply the same preprocessor to each candidate;
- use `Indel.BestSubstringMatchImpl` when the query is no longer than the candidate;
- when a candidate is shorter than the cached query, build a temporary pattern vector for the candidate and search it in the cached query text;
- for equal lengths, evaluate both directions and use the larger score;
- dispose the owned query pattern vector exactly once;
- reject scoring after disposal only if existing cached strategies follow that convention; otherwise preserve repository behavior.

Do not mutate the cached `PatternMatchVector` during scoring. The scorer should be safe for concurrent reads until disposed.

Be careful: caching the first argument does not guarantee that it is always the shorter input. Correctness takes priority over reusing the cached mask in that case.

### 6. Cached scorer

Add:

```text
FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/CachedApproximateSubstringRatioScorer.cs
```

Follow `CachedDefaultRatioScorer` ownership conventions:

- constructor taking `string input1` and optional preprocessor owns its strategy;
- optional constructor taking `ICachedStrategy` does not own it unless explicitly requested;
- dispose only when the scorer owns the strategy.

### 7. Public `Fuzz` API

Add a new region next to `PartialRatio`:

```csharp
public static int ApproximateSubstringRatio(
    string input1,
    string input2)
{
    return ScorerCache
        .Get<ApproximateSubstringRatioScorer>()
        .Score(input1, input2);
}

public static int ApproximateSubstringRatio(
    string input1,
    string input2,
    Func<string, string> preprocessor)
{
    return ScorerCache
        .Get<ApproximateSubstringRatioScorer>()
        .Score(input1, input2, preprocessor);
}
```

Add XML documentation that clearly says:

- the shorter input is searched approximately inside the longer input;
- insertions and deletions are used;
- substitution has cost `2`;
- the metric is not identical to `PartialRatio`.

### 8. Process/extractor integration

Ensure the scorer can be used explicitly through existing process APIs:

```csharp
ProcessBuilder
    .WithScorer(new ApproximateSubstringRatioScorer())
```

and cached APIs:

```csharp
using var scorer = new CachedApproximateSubstringRatioScorer(query);

var pipeline = new ProcessBuilder()
    .Cached(scorer)
    .Build();
```

Do not change the default scorer used by `ProcessBuilder`.

Do not add the new metric to `WeightedRatio` in this feature unless an existing extension point makes that completely opt-in and backward-compatible.

---

## Correctness requirements

The bit-parallel implementation is performance-sensitive and must be verified against a simple oracle.

### Scalar oracle

Add a test-only `O(mn)` dynamic-programming implementation with these boundaries:

```text
D[i, 0] = i
D[0, j] = 0
```

For each text column `j`, calculate:

```text
D[i, j] = min(
    D[i - 1, j] + 1,        // delete pattern element
    D[i, j - 1] + 1,        // insert text element
    D[i - 1, j - 1] + cost  // 0 when equal, otherwise 2
)
```

Track the same best-distance and earliest-strict-improvement endpoint behavior as the production implementation.

Use this implementation only in tests.

### Required low-level tests

Add focused tests for:

1. Empty pattern.
2. Empty text.
3. Both empty.
4. Exact match at the beginning.
5. Exact match in the middle.
6. Exact match after an earlier approximate occurrence.
7. Repeated exact occurrences return the earliest exact endpoint.
8. A substitution costs `2`.
9. A shorter matching substring can be selected by deleting pattern elements.
10. No useful common element returns distance equal to pattern length and `Found == false` under the current tie semantics.
11. Generic non-character sequences, for example `int[]`.
12. Pattern lengths `1`, `2`, `63`, and `64`.
13. Multi-block pattern lengths `65`, `66`, `127`, `128`, and `129`.
14. Last-block masking when pattern length is not divisible by 64.
15. Carry propagation across block boundaries.
16. Borrow propagation across block boundaries.
17. Cross-block left shift.
18. Cross-block right shift.
19. Text characters/elements absent from the pattern vector.
20. Exact match crossing the 64-bit boundary.

### Paper-style example

Include a known approximate-substring example such as:

```text
pattern: ACGC
text:    GAAGCGACTGCAAACTCA
```

Verify the expected best distance and endpoint using the scalar oracle. Do not hard-code an expected endpoint copied from documentation without confirming it against the oracle.

### Differential/property tests

Add deterministic randomized tests comparing the production implementation with the scalar oracle.

Cover at least:

```text
alphabet sizes:       2, 4, and a larger character set
pattern lengths:      1..140
text lengths:         0..200
single-block cases:   many samples
multi-block cases:    many samples
random seed:          fixed and printed/assertable
```

At minimum, run several thousand deterministic cases.

Also add an exhaustive test over a binary alphabet for small lengths, for example:

```text
pattern length: 0..7
text length:    0..8
```

Compare both `Distance` and `EndIndex`.

### Required scorer tests

Verify:

```text
("abc", "xxabcxx")       => 100
("xxabcxx", "abc")       => 100
("", "")                 => 100
("abc", "")              => 0
("", "abc")              => 0
```

Also verify:

- score is always in `[0, 100]`;
- preprocessing is applied correctly;
- non-cached and cached scorers return identical scores;
- argument order does not change the public scorer result;
- equal-length directional cases use the better direction;
- repeated occurrences do not lock onto the first inferior occurrence;
- the score intentionally differs from `PartialRatio` for at least one documented example.

Do not assert that `ApproximateSubstringRatio` equals RapidFuzz/FuzzyWuzzy `partial_ratio`.

### Disposal and concurrency tests

For the cached scorer:

- verify owned resources are disposed;
- verify an externally supplied strategy is not disposed unless ownership was requested;
- run concurrent `Score` calls against one cached scorer instance and compare every result with the non-cached scorer;
- do not run scoring concurrently with `Dispose` unless the repository explicitly promises that behavior.

---

## Performance requirements

The new implementation must retain the bit-parallel complexity:

```text
O(textLength * ceil(patternLength / 64))
```

Expected allocation behavior:

- single-block uncached call: pattern-vector allocation according to existing infrastructure, no algorithm-state array allocation;
- single-block cached call: no per-call algorithm-state allocation;
- multi-block call: pooled state buffers only;
- all rented arrays must be returned in `finally` blocks;
- do not clear returned arrays unless sensitive-data policy or repository convention requires it.

Do not introduce LINQ into the hot path.

Do not materialize spans as arrays unless required by an existing interface.

Do not use exceptions for normal scorer flow.

---

## Benchmarks

Add BenchmarkDotNet coverage in `FuzzySharp.Benchmarks`.

Suggested class:

```text
ApproximateSubstringRatioBenchmarks
```

Benchmark at least:

1. `Indel.BestSubstringMatch` single block.
2. `Indel.BestSubstringMatch` multi-block.
3. `Fuzz.ApproximateSubstringRatio`.
4. `CachedApproximateSubstringRatioScorer.Score`.
5. A test/benchmark-only scalar DP baseline.
6. Existing `Fuzz.PartialRatio` as a performance reference, clearly noting that semantics differ.

Use parameter sets around important boundaries:

```text
pattern length: 8, 32, 63, 64, 65, 128, 129, 256
text length:    64, 256, 1024, 4096
```

Include datasets with:

- exact match near the start;
- exact match near the end;
- no exact match;
- repeated approximate and exact occurrences;
- random low-similarity data;
- high-similarity data.

Report both runtime and allocations using `[MemoryDiagnoser]`.

Do not claim the new scorer is faster than `PartialRatio` without benchmark evidence.

---

## Documentation

Update `README.md` with a concise section containing:

```csharp
int score = Fuzz.ApproximateSubstringRatio(
    "invoice number 12345",
    "processed invoice number 12345 successfully");
```

Explain:

- it finds the best approximate occurrence of the shorter string;
- it considers all possible start positions implicitly;
- later repeated occurrences are not hidden by an earlier inferior occurrence;
- insertions/deletions are the edit model;
- normalization is relative to pattern length;
- `EndIndex` is available from `Indel.BestSubstringMatch`;
- the start index and edit script are not returned;
- it is not numerically compatible with `PartialRatio`.

Add an entry to `CHANGELOG.md` describing the new API without promising exact RapidFuzz compatibility.

---

## Compatibility requirements

The library multi-targets old .NET Framework, `netstandard2.0`, and modern .NET.

The implementation must compile for all configured target frameworks.

In particular:

- do not use `record struct`;
- do not rely on APIs introduced after `netstandard2.0` without existing polyfills or conditional compilation;
- use `System.Memory`/existing span support already configured by the project;
- preserve nullable annotations/style currently used by the repository;
- do not add a runtime dependency solely for this feature.

At minimum, run:

```bash
dotnet build FuzzySharp/FuzzySharp.csproj -f netstandard2.0
dotnet test FuzzySharp.Test/FuzzySharp.Test.csproj -f net8.0
dotnet test FuzzySharp.Test/FuzzySharp.Test.csproj -f net10.0
```

When running on Windows with the required targeting packs, also run the legacy .NET Framework test targets configured by the project.

Run the full repository test suite, not only the newly added tests.

---

## Code-quality requirements

- Follow existing formatting, namespace, naming, XML documentation, and file-layout conventions.
- Keep bit-vector variable names aligned with the paper where useful, but prefer readable names already present in the baseline.
- Preserve `unchecked` arithmetic where wraparound is part of the bit-vector algorithm.
- Keep single-block and multi-block paths separate unless a refactor demonstrably improves readability without hurting performance.
- Add comments only around non-obvious boundary conditions, carries, borrows, shifts, and last-block masking.
- Avoid comments that simply restate the code.
- Do not expose internal pooled buffers.
- Do not change `IPatternMatchVector<T>` unless absolutely necessary.
- Do not commit generated benchmark artifacts unless the repository already tracks them intentionally.

---

## Out of scope

Do not implement these in this feature unless required to fix correctness:

- start-index reconstruction;
- edit-operation traceback;
- returning every matching endpoint;
- Unicode grapheme-cluster segmentation;
- culture-aware equality inside the generic algorithm;
- replacement of `PartialRatio`;
- integration into `WeightedRatio` defaults;
- SIMD intrinsics beyond the existing word-parallel implementation;
- approximate matching with substitution cost `1`.

These may be separate follow-up features.

---

## Acceptance criteria

The feature is complete only when all of the following are true:

- [ ] Existing single-block and multi-block IndelNew code is retained or equivalently optimized.
- [ ] Production results match the scalar DP oracle for exhaustive and deterministic randomized tests.
- [ ] Pattern lengths on both sides of every 64-bit boundary are covered.
- [ ] `Fuzz.ApproximateSubstringRatio` is public and documented.
- [ ] `ApproximateSubstringRatioScorer` is available.
- [ ] `CachedApproximateSubstringRatioScorer` is available and reuses a precomputed pattern vector when valid.
- [ ] Cached and non-cached results are identical.
- [ ] The public scorer is symmetric, including equal-length directional cases.
- [ ] Existing `Fuzz.PartialRatio` behavior is unchanged.
- [ ] Existing default `WeightedRatio` and `ProcessBuilder` behavior is unchanged.
- [ ] All tests pass on `net8.0` and `net10.0`.
- [ ] The library builds for `netstandard2.0`.
- [ ] Legacy targets are validated when the required Windows targeting packs are available.
- [ ] Benchmarks cover single-block, multi-block, cached, uncached, and scalar-oracle paths.
- [ ] README and changelog are updated.
- [ ] No unreturned pooled arrays or undisposed cached pattern vectors remain.

---

## Final agent response

After implementation, report:

1. files added and changed;
2. the final public API;
3. the exact scoring semantics;
4. correctness-test coverage and randomized test seed;
5. build/test commands executed and their results;
6. benchmark summary, including allocations;
7. any deliberate deviations from this specification;
8. any remaining risks or recommended follow-up work.

Do not report completion if tests were not run. If a target cannot be tested in the current environment, state that explicitly and list the exact unverified target.
