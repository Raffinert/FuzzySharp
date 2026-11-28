[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/banner2-direct.svg)](https://stand-with-ukraine.pp.ua)

## Terms of use<sup>[?](https://github.com/Tyrrrz/.github/blob/master/docs/why-so-political.md)</sup>

By using this project or its source code, for any purpose and in any shape or form, you grant your **implicit agreement** to all the following statements:

- You **condemn Russia and its military aggression against Ukraine**
- You **recognize that Russia is an occupant that unlawfully invaded a sovereign state**
- You **support Ukraine's territorial integrity, including its claims over temporarily occupied territories of Crimea and Donbas**
- You **reject false narratives perpetuated by Russian state propaganda**

To learn more about the war and how you can help, [click here](https://stand-with-ukraine.pp.ua). Glory to Ukraine! 🇺🇦

# Raffinert.FuzzySharp

[![nuget version](https://img.shields.io/nuget/v/Raffinert.FuzzySharp.svg?style=flat-square)](https://www.nuget.org/packages/Raffinert.FuzzySharp)
[![nuget downloads](https://img.shields.io/nuget/dt/Raffinert.FuzzySharp?label=Downloads)](https://www.nuget.org/packages/Raffinert.FuzzySharp)

Raffinert.FuzzySharp is a high-performance .NET fuzzy string matching library inspired by SeatGeek's FuzzyWuzzy and RapidFuzz.

It is a bit-parallel accelerated version of the original [FuzzySharp](https://github.com/JakeBayer/FuzzySharp), with multiple fixes to the [partial_ratio implementation](https://github.com/rapidfuzz/RapidFuzz/blob/main/api_differences.md#partial_ratio-implementation), lower-level distance APIs, cached scorers, and reusable extraction pipelines.

## Overview

Use Raffinert.FuzzySharp when you need to:

- Score two strings for similarity.
- Find the best match in a list of choices.
- Match strings while ignoring word order, duplicate words, case, punctuation, or abbreviations.
- Reuse cached scorers for one-to-many comparisons.
- Run extraction pipelines sequentially or in parallel.
- Access fast Levenshtein, Indel, and Longest Common Subsequence distance APIs directly.

## Contents

- [Installation](#installation)
- [Compatibility](#compatibility)
- [Quick Start](#quick-start)
- [Choosing a Scorer](#choosing-a-scorer)
- [Ratio Examples](#ratio-examples)
- [Process Extraction](#process-extraction)
- [Fluent Pipeline API](#fluent-pipeline-api)
- [Using Different Scorers](#using-different-scorers)
- [Distance APIs](#distance-apis)
- [String Preprocessors](#string-preprocessors)
- [Performance](#performance)
- [Migration Notes](#migration-notes)
- [Credits](#credits)
- [Support](#support)

## Installation

```powershell
Install-Package Raffinert.FuzzySharp
```

or

```bash
dotnet add package Raffinert.FuzzySharp
```

## Compatibility

Raffinert.FuzzySharp targets:

- .NET Standard 2.0 and 2.1
- .NET Framework 4.5, 4.6, 4.6.2, 4.7.2, and 4.8
- .NET Core 3.1
- .NET 6, .NET 8, .NET 9, and .NET 10

## Quick Start

Find the best match from a list of choices:

```csharp
var choices = new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" };

var result = Process.ExtractOne("cowboys", choices);
// (string: Dallas Cowboys, score: 90, index: 3)
```

Compare two strings directly:

```csharp
Fuzz.Ratio("mysmilarstring", "mysimilarstring");
// 96.55172413793103

Fuzz.WeightedRatio(
    "The quick brown fox jimps ofver the small lazy dog",
    "the quick brown fox jumps over the small lazy dog");
// 94.94949494949495
```

Similarity scores are `double` values in the range 0–100.

By default, `Fuzz` methods compare strings as-is. `Process` extraction methods use `StringPreprocessor.Full` by default, which normalizes whitespace, lowercases, and strips non-alphanumeric characters.

## Choosing a Scorer

| Scorer | Use when |
|--------|----------|
| `Fuzz.Ratio` | You need direct similarity between two strings. |
| `Fuzz.PartialRatio` | One string may be a substring or close substring of the other. |
| `Fuzz.TokenSortRatio` | Word order should not matter. |
| `Fuzz.TokenSetRatio` | Duplicate words or extra common words should have less impact. |
| `Fuzz.TokenInitialismRatio` | You need to compare an initialism with its expanded phrase. |
| `Fuzz.TokenAbbreviationRatio` | You need abbreviation-aware matching. |
| `Fuzz.WeightedRatio` | You want a general-purpose scorer that combines several strategies. |
| `Process.ExtractOne` / `ExtractTop` / `ExtractAll` | You need to search a collection of choices. |
| `Process.Configure()` | You want reusable extraction with caching, custom scorers, or parallel execution. |

## Ratio Examples

### Simple Ratios
<p align="right"><a href="https://dotnetfiddle.net/9JpFTQ">Run .NET fiddle</a></p>

```csharp
Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
// 72.22222222222221
Fuzz.Ratio("mysmilarstring", "mysimilarstring");
// 96.55172413793103
```

### Partial Ratio
<p align="right"><a href="https://dotnetfiddle.net/rk0dIO">Run .NET fiddle</a></p>

```csharp
Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
// 71.42857142857143
```

### Token Sort Ratio
<p align="right"><a href="https://dotnetfiddle.net/b5RVp2">Run .NET fiddle</a></p>

```csharp
Fuzz.TokenSortRatio("order words out of", "  words out of order");
// 100
Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
// 100
```

### Token Set Ratio
<p align="right"><a href="https://dotnetfiddle.net/ZfZRGb">Run .NET fiddle</a></p>

```csharp
Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
// 100
Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
// 100
```

### Token Initialism Ratio
<p align="right"><a href="https://dotnetfiddle.net/87181A">Run .NET fiddle</a></p>

```csharp
Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
// 88.88888888888889
Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
// 100

Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
// 53.333333333333336
Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
// 100
```

### Token Abbreviation Ratio
<p align="right"><a href="https://dotnetfiddle.net/MVlwrW">Run .NET fiddle</a></p>

```csharp
Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full);
// 40
Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full);
// 66.66666666666667
```

### Weighted Ratio
<p align="right"><a href="https://dotnetfiddle.net/n9QxAk">Run .NET fiddle</a></p>

```csharp
Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
// 94.94949494949495
```

`WeightedRatio` intentionally retains its legacy full `TokenSortRatio`/`TokenSetRatio` partial branch for compatibility.

## Process Extraction

Find the best match(es) from a collection of choices.

<p align="right"><a href="https://dotnetfiddle.net/8lEzk3">Run .NET fiddle</a></p>

```csharp
Process.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" });
// (string: Dallas Cowboys, score: 90, index: 3)
```
```csharp
Process.ExtractTop("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" }, limit: 3);
// [(string: google, score: 83.33333333333334, index: 0), (string: googleplus, score: 75.00000000000001, index: 5), (string: plexoogl, score: 42.85714285714286, index: 7)]
```
```csharp
Process.ExtractAll("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" });
// [(string: google, score: 83.33333333333334, index: 0), (string: bing, score: 36, index: 1), ...]

// With score cutoff
Process.ExtractAll("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" }, cutoff: 40);
// [(string: google, score: 83.33333333333334, index: 0), (string: googleplus, score: 75.00000000000001, index: 5), (string: plexoogl, score: 42.85714285714286, index: 7)]
```
```csharp
Process.ExtractSorted("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" });
// [(string: google, score: 83.33333333333334, index: 0), (string: googleplus, score: 75.00000000000001, index: 5), (string: plexoogl, score: 42.85714285714286, index: 7), ...]
```

Extraction uses `WeightedRatio` and `Full` preprocessing by default. Override these in the method parameters to use different scorers and processing:
```csharp
Process.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" }, s => s, ScorerCache.Get<DefaultRatioScorer>());
// (string: Dallas Cowboys, score: 57.14285714285714, index: 3)
```

### Generic Type Extraction

<p align="right"><a href="https://dotnetfiddle.net/YDtl6k">Run .NET fiddle</a></p>

Extraction can operate on objects of any type. Use the `extractor` parameter to reduce the object to the string it should be compared on:
```csharp
var events = new[]
{
    new[] { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" },
    new[] { "new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm" },
    new[] { "atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm" },
};
var query = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };
var best = Process.ExtractOneBy(query, events, strings => strings[0]);
// (value: { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" }, score: 95, index: 0)
```

If the query is already a string, it can be matched directly against generic choices by providing only a choice extractor:
```csharp
var events = new[]
{
    new { Name = "new york mets vs chicago cubs", Venue = "CitiField" },
    new { Name = "chicago cubs vs chicago white sox", Venue = "Wrigley Field" },
    new { Name = "philadelphia phillies vs atlanta braves", Venue = "Citizens Bank Park" },
};

var query = "new york mets at chicago cubs";

var best = Process.ExtractOneBy(query, events, e => e.Name);
// best.Value.Name == "new york mets vs chicago cubs"

var top = Process.ExtractTopBy(query, events, e => e.Name, limit: 2);
```

## Fluent Pipeline API

The `Process.Configure()` fluent builder creates reusable, immutable pipelines with preconfigured scoring, caching, and parallel execution.

### Basic Pipeline

Equivalent to the static `Process` methods, but reusable across multiple queries:

<p align="right"><a href="https://dotnetfiddle.net/KEFXxm">Run .NET fiddle</a></p>

```csharp
var pipeline = Process.Configure().Build();

var result1 = pipeline.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" });
//(string: Dallas Cowboys, score: 90, index: 3)
var result2 = pipeline.ExtractOne(
    "chicago cubs",
    new[]
    {
        "Boston Red Sox",
        "Los Angeles Dodgers",
        "New York Yankees",
        "San Francisco Giants",
        "St. Louis Cardinals",
        "Houston Astros"
    });
//(string: San Francisco Giants, score: 45, index: 3)
```

### Custom Scorer

<p align="right"><a href="https://dotnetfiddle.net/6JVmU9">Run .NET fiddle</a></p>

```csharp
var pipeline = Process.Configure()
    .WithScorer(ScorerCache.Get<DefaultRatioScorer>())
    .Build();

var result = pipeline.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" });
//(string: Dallas Cowboys, score: 66.66666666666667, index: 3)
```

### Parallel Execution

Enable multi-threaded processing for large choice sets:
```csharp
var pipeline = Process.Configure()
    .Parallel()
    .Build();

var results = pipeline.ExtractAll("goolge", largeChoicesList);
```

With `ParallelOptions` for fine-grained control:
```csharp
var pipeline = Process.Configure()
    .Parallel(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount })
    .Build();
```

### Cached Execution

<p align="right"><a href="https://dotnetfiddle.net/y6qMJm">Run .NET fiddle</a></p>

Automatic caching creates a `CachedWeightedRatioScorer` per extraction call, pre-initializing internal data structures for the query string:
```csharp
var pipeline = Process.Configure()
    .Cached()
    .Build();

var result = pipeline.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" });
```

Caching pays off when the query is compared with many candidates or reused across calls. For a single score, call `Fuzz.Ratio` or `Fuzz.WeightedRatio` directly; constructing a cached scorer for one comparison adds work and allocations.

### Cached + Parallel

Combine caching and parallelism. Builder methods are **order independent** -- `.Cached().Parallel()` and `.Parallel().Cached()` produce identical results:
```csharp
var pipeline = Process.Configure()
    .Cached()
    .Parallel(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount })
    .Build();

var results = pipeline.ExtractAll("goolge", largeChoicesList);
```

### External Cached Scorer

For maximum performance when running the same query against different choice sets, provide an externally managed `ICachedRatioScorer`. The scorer pre-initializes once and is reused across all extraction calls:
```csharp
using var scorer = new CachedWeightedRatioScorer("new york mets at atlanta braves");

var pipeline = Process.Configure()
    .Cached(scorer)
    .Parallel()
    .Build();

var results1 = pipeline.ExtractAll(choiceSet1);
var results2 = pipeline.ExtractAll(choiceSet2);
```

> **Note:** External cached scorers implement `IDisposable`. Use `using` to ensure proper cleanup.

### CancellationToken Support

Pass a `CancellationToken` via `ParallelOptions` to cancel long-running parallel extractions:
```csharp
var cts = new CancellationTokenSource();

var pipeline = Process.Configure()
    .Cached()
    .Parallel(new ParallelOptions { CancellationToken = cts.Token })
    .Build();

// Throws OperationCanceledException if cancelled
var results = pipeline.ExtractAll(query, largeChoicesList).ToList();
```

## Using Different Scorers

### Non-Cached Scorers (`IRatioScorer`)

Stateless scorers for use with `Process` static methods and the `WithScorer()` builder method:
```csharp
var ratio              = ScorerCache.Get<DefaultRatioScorer>();
var partialRatio       = ScorerCache.Get<PartialRatioScorer>();
var tokenSet           = ScorerCache.Get<TokenSetScorer>();
var partialTokenSet    = ScorerCache.Get<PartialTokenSetScorer>();
var tokenSort          = ScorerCache.Get<TokenSortScorer>();
var partialTokenSort   = ScorerCache.Get<PartialTokenSortScorer>();
var tokenAbbreviation  = ScorerCache.Get<TokenAbbreviationScorer>();
var partialTokenAbbrev = ScorerCache.Get<PartialTokenAbbreviationScorer>();
var weighted           = ScorerCache.Get<WeightedRatioScorer>();
```

### Cached Scorers (`ICachedRatioScorer`)

<p align="right"><a href="https://dotnetfiddle.net/Ykr94M">Run .NET fiddle</a></p>

Pre-initialize with a query string for repeated comparisons. These implement `IDisposable`:
```csharp
using var scorer = new CachedWeightedRatioScorer("search query");
double score = scorer.Score("candidate string");
// 27.14285714285714
```

Available cached scorers:
- `CachedWeightedRatioScorer` -- weighted combination (default for `.Cached()`)
- `CachedDefaultRatioScorer` -- simple Levenshtein ratio
- `CachedTokenSortScorer` -- token sort ratio
- `CachedTokenSetScorer` -- token set ratio
- `CachedPartialTokenSetScorer` -- partial token set ratio
- `CachedTokenDifferenceScorer` -- token difference ratio

## Distance APIs

### Levenshtein Distance API

<p align="right"><a href="https://dotnetfiddle.net/JzzcwS">Run .NET fiddle</a></p>

Low-level access to the bit-parallel Levenshtein distance implementation:

```csharp
// Edit distance
int distance = Levenshtein.Distance("kitten", "sitting");
// 3

// Normalized similarity (1.0 = identical, 0.0 = completely different)
double similarity = Levenshtein.NormalizedSimilarity("kitten", "sitting");
// 0.5714285714285714

// Edit operations to transform one string into another
EditOp[] ops = Levenshtein.GetEditOps("kitten", "sitting");
// [REPLACE(0, 0), REPLACE(4, 4), INSERT(6, 6)]
```

### Instance Distance Classes

The `Levenshtein`, `Indel`, and `LongestCommonSubsequence` classes also offer an **instance API** for one-to-many comparisons. The constructor pre-computes a bit-parallel pattern match vector from the source string, which is then reused across all subsequent calls. This avoids rebuilding the internal data structure on every comparison, giving a significant speedup when comparing one source against many targets.

All three implement `IDisposable` -- use `using` to return pooled arrays.

#### Levenshtein Instance

<p align="right"><a href="https://dotnetfiddle.net/hxUB72">Run .NET fiddle</a></p>

```csharp
using var lev = new Levenshtein("chicago cubs vs new york mets");

int d1 = lev.DistanceFrom("new york mets vs chicago cubs");
// 22
int d2 = lev.DistanceFrom("atlanta braves vs pittsburgh pirates");
// 26
```

#### Indel Instance

<p align="right"><a href="https://dotnetfiddle.net/UkyXk2">Run .NET fiddle</a></p>

Indel distance counts only insertions and deletions (no replacements). `NormalizedSimilarityWith` returns a value between 0.0 (completely different) and 1.0 (identical):

```csharp
using var indel = new Indel("chicago cubs");

int distance = indel.DistanceFrom("chicago white sox");
// 11
double similarity = indel.NormalizedSimilarityWith("chicago white sox");
// 0.6206896551724138
```

A generic variant `IndelT<T>` is available for comparing sequences of any `IEquatable<T>`:

```csharp
using var indel = new IndelT<string>(new[] { "hello", "world" });

int distance = indel.DistanceFrom(new[] { "hello", "there" });
// 2
double similarity = indel.NormalizedSimilarityWith(new[] { "hello", "there" });
// 0.5
```

#### LongestCommonSubsequence Instance

<p align="right"><a href="https://dotnetfiddle.net/s60rZT">Run .NET fiddle</a></p>

LCS distance is defined as `max(len1, len2) - LCS_length`:

```csharp
using var lcs = new LongestCommonSubsequence("chicago cubs");

int distance = lcs.DistanceFrom("chicago white sox");
// 8
```

## String Preprocessors
<p align="right"><a href="https://dotnetfiddle.net/cIQ6PB">Run .NET fiddle</a></p>

By default, `Fuzz` methods compare strings as-is. Pass `StringPreprocessor.Full` to normalize whitespace, lowercase, and strip non-alphanumeric characters before comparing:

```csharp
Fuzz.Ratio("new york mets", "NEW YORK METS");
// < 100 (case sensitive)

Fuzz.Ratio("new york mets", "NEW YORK METS", StringPreprocessor.Full);
// 100 (case insensitive after preprocessing)
```

`Process` extraction methods use `StringPreprocessor.Full` by default. The default processor lowercases with invariant Unicode casing, so its results are deterministic across request cultures. Pass `StringPreprocessor.None` (or a custom `processor` function) to override this behavior or provide locale-specific matching.

## Performance

Measured on **2026-11-28** using a development build of Raffinert.FuzzySharp and the original [FuzzySharp 2.0.2](https://github.com/JakeBayer/FuzzySharp) package. BenchmarkDotNet 0.15.8, Release, .NET 10.0.15 / SDK 11.0.100, Windows 11, Intel Core Ultra 9 275HX; sequential workloads pinned to P-core 10, tiered compilation and dynamic PGO disabled, two launches, six warmups and ten measured iterations per launch with a 300 ms target. No outliers were removed. Times below are means; `±` is the 99.9% confidence-interval half-width. Absolute times from earlier runs with different JIT or affinity settings are not a regression baseline.

Selected fixed-input API comparisons (mean time per operation):

| Operation | Measured build | Classic 2.0.2 | Speedup |
| --- | ---: | ---: | ---: |
| Ratio | 55.47 ± 0.54 ns | 174.65 ± 1.79 ns | 3.15× |
| PartialRatio | 246.00 ± 2.14 ns | 613.00 ± 8.36 ns | 2.49× |
| TokenSetRatio | 325.20 ± 3.70 ns | 2,017.90 ± 6.99 ns | 6.21× |
| WeightedRatio | 3,125.00 ± 47.00 ns | 8,984.00 ± 25.30 ns | 2.87× |
| PartialRatio (17-character pattern in 517-character text) | 1,051.00 ± 6.40 ns | 10,976.00 ± 48.70 ns | 10.44× |
| PartialRatio (257-character Cyrillic strings) | 13.220 ± 0.075 μs | 5,436.591 ± 37.149 μs | 411.24× |
| ExtractOne | 5.449 ± 0.019 μs | 19.320 ± 0.253 μs | 3.55× |
| ExtractAll | 5.633 ± 0.016 μs | 19.372 ± 0.176 μs | 3.44× |

The large Levenshtein workload generates 20 seeded words with lengths in `[3, 1024)` and computes 400 ordered pairs per operation. Fastenshtein and Raffinert.FuzzySharp reuse an instance per source word. The `FuzzySharp` row below is Classic 2.0.2; ratios are relative to naive DP. Times and allocations are per batch:

| Method               | Mean       | Error     | StdDev    | Ratio | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|--------------------- |-----------:|----------:|----------:|------:|-----------:|-----------:|------------:|------------:|
| NaiveDp              | 146.787 ms | 1.2307 ms | 1.4173 ms |  1.00 | 14500.0000 | 12500.0000 | 275312720 B |       1.000 |
| [FuzzySharp](https://github.com/JakeBayer/FuzzySharp) | 105.555 ms | 0.0896 ms | 0.1032 ms |  0.72 |          - |          - |   1545632 B |       0.006 |
| [Fastenshtein](https://github.com/DanHarltey/Fastenshtein) |  97.087 ms | 0.4391 ms | 0.5056 ms |  0.66 |          - |          - |     33928 B |       0.000 |
| [Quickenshtein](https://github.com/Turnerj/Quickenshtein) |   8.226 ms | 0.0315 ms | 0.0363 ms |  0.06 |          - |          - |           - |       0.000 |
| [Raffinert.FuzzySharp](https://github.com/Raffinert/FuzzySharp) |   2.399 ms | 0.0065 ms | 0.0075 ms |  0.02 |          - |          - |      2080 B |       0.000 |

See [benchmark results and reproduction instructions](https://github.com/Raffinert/FuzzySharp/blob/master/FuzzySharp.Benchmarks/README.md) for cache/parallel variants, multiword cases, allocations and inputs. These microbenchmarks cover specific inputs on one machine; small differences need longer measurements. Partial-ratio algorithms and score rounding differ between the libraries.

## Migration Notes

### From original FuzzySharp

The package name is `Raffinert.FuzzySharp`, and the default namespace is `Raffinert.FuzzySharp`. This package keeps the familiar FuzzyWuzzy-style API while adding faster distance implementations, fixed partial-ratio behavior, cached scorers, and the fluent process pipeline.

### From Raffinert.FuzzySharp v4 to v5

- `PreprocessMode`-based APIs were replaced with `Func<string, string>` preprocessors.
- Use `StringPreprocessor.Full`, `StringPreprocessor.None`, or a custom delegate instead of `PreprocessMode.Full` or `PreprocessMode.None`.
- Generic extraction methods that use extractor delegates are now named `Extract*By`, such as `ExtractOneBy` and `ExtractTopBy`.

### From Raffinert.FuzzySharp v5 to v6

Similarity scoring now uses `double` throughout the library so fractional scores are preserved instead of being rounded to integers:

```csharp
double score = Fuzz.Ratio("new york mets", "new york mets!");

ExtractedResult<string> match = Process.ExtractOne(
    "goolge",
    new[] { "google", "bing", "facebook" });

double matchScore = match.Score;
```

The following APIs changed from `int` to `double`:

- `Fuzz` similarity methods
- `Scorer` and `CachedScorer` delegates
- `IRatioScorer`, `ICachedRatioScorer`, and their generic variants
- scoring strategies, including `ICachedStrategy`
- `ExtractedResult<T>.Score`
- extraction `cutoff` parameters across standard, cached, parallel, and pipeline APIs

Extraction cutoffs can therefore retain fractional thresholds:

```csharp
var matches = Process.ExtractAll(
    "goolge",
    new[] { "google", "bing", "facebook" },
    cutoff: 87.5);
```

Custom scorers and method-group targets must update their return types accordingly:

```csharp
public sealed class CustomScorer : IRatioScorer
{
    public double Score(string input1, string input2) => /* calculate score */ 0.0;

    public double Score(
        string input1,
        string input2,
        Func<string, string> preprocessor) =>
        Score(preprocessor(input1), preprocessor(input2));
}
```

The optional `scoreCutoff` argument was removed from Indel, Levenshtein, and longest-common-subsequence distance and similarity APIs. Compute the result first and apply any threshold explicitly:

```csharp
// v5
int limitedDistance = Levenshtein.Distance("kitten", "sitting", scoreCutoff: 2);

// v6
int distance = Levenshtein.Distance("kitten", "sitting");
bool isWithinCutoff = distance <= 2;
```

This does not remove extraction filtering: `Process` and `ResultExtractor` still accept `cutoff`, now as a `double`. Raw edit distances and unnormalized similarity counts remain `int`; normalized distance and similarity values remain `double` in the range 0–1.

## Credits

- [Adam Cohen (seatgeek/fuzzywuzzy)](https://chairnerd.seatgeek.com/fuzzywuzzy-fuzzy-string-matching-in-python/)
- [Antti Haapala (python-Levenshtein)](https://github.com/ztane)
- [David Necas (python-Levenshtein)](http://publications.physics.muni.cz/author/yeti)
- [Jacob Bayer (original FuzzySharp library)](https://github.com/JakeBayer/FuzzySharp)
- [Max Bachmann (RapidFuzz)](https://github.com/rapidfuzz/RapidFuzz)
- [Mikko Ohtamaa (python-Levenshtein)](https://github.com/miohtama/python-Levenshtein)
- [Panayiotis (Java implementation)](https://github.com/xdrop)

## Support

Support the project through [GitHub Sponsors](https://github.com/sponsors/ycherkes) or via [PayPal](https://www.paypal.com/donate/?business=KXGF7CMW8Y8WJ).

See [CHANGELOG.md](CHANGELOG.md) for release history.
