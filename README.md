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
// 97

Fuzz.WeightedRatio(
    "The quick brown fox jimps ofver the small lazy dog",
    "the quick brown fox jumps over the small lazy dog");
// 95
```

By default, `Fuzz` methods compare strings as-is. `Process` extraction methods use `StringPreprocessor.Full` by default, which normalizes whitespace, lowercases, and strips non-alphanumeric characters.

## Choosing a Scorer

| Scorer | Use when |
|--------|----------|
| `Fuzz.Ratio` | You need direct similarity between two strings. |
| `Fuzz.PartialRatio` | One string may be a substring or close substring of the other. |
| `Fuzz.ApproximateSubstringRatio` | You need the best Indel-distance match of the shorter input within the longer input. |
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
// 72
Fuzz.Ratio("mysmilarstring", "mysimilarstring");
// 97
```

### Partial Ratio
<p align="right"><a href="https://dotnetfiddle.net/rk0dIO">Run .NET fiddle</a></p>

```csharp
Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
// 71
```

### Approximate Substring Ratio

```csharp
int score = Fuzz.ApproximateSubstringRatio(
    "invoice number 12345",
    "processed invoice number 12345 successfully");
// 100
```

`ApproximateSubstringRatio` searches for the best approximate occurrence of the
shorter input, considering every possible start position implicitly. It uses the
Indel edit model: insertions and deletions cost one, while a substitution costs
two. The score is normalized relative to the shorter input's length. Equal-length
inputs are evaluated in both directions unless the first direction is an exact
match, so the high-level score is symmetric. Its numeric results are not
equivalent to `PartialRatio`.

For endpoint information, call `Indel.BestSubstringMatch` directly; its
`FirstBestEndIndex` is the first endpoint that strictly improves on the distance
of deleting the complete pattern. Equal-distance later endpoints do not replace
it, so it is not a unique match boundary. This directional API does not return a
start index or edit script.

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
// 89
Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
// 100

Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
// 53
Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
// 100
```

### Token Abbreviation Ratio
<p align="right"><a href="https://dotnetfiddle.net/MVlwrW">Run .NET fiddle</a></p>

```csharp
Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full);
// 40
Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full);
// 67
```

### Weighted Ratio
<p align="right"><a href="https://dotnetfiddle.net/n9QxAk">Run .NET fiddle</a></p>

```csharp
Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
// 95
```

## Process Extraction

Find the best match(es) from a collection of choices.

<p align="right"><a href="https://dotnetfiddle.net/8lEzk3">Run .NET fiddle</a></p>

```csharp
Process.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" });
// (string: Dallas Cowboys, score: 90, index: 3)
```
```csharp
Process.ExtractTop("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" }, limit: 3);
// [(string: google, score: 83, index: 0), (string: googleplus, score: 75, index: 5), (string: plexoogl, score: 43, index: 7)]
```
```csharp
Process.ExtractAll("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" });
// [(string: google, score: 83, index: 0), (string: bing, score: 36, index: 1), ...]

// With score cutoff
Process.ExtractAll("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" }, cutoff: 40);
// [(string: google, score: 83, index: 0), (string: googleplus, score: 75, index: 5), (string: plexoogl, score: 43, index: 7)]
```
```csharp
Process.ExtractSorted("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" });
// [(string: google, score: 83, index: 0), (string: googleplus, score: 75, index: 5), (string: plexoogl, score: 43, index: 7), ...]
```

Extraction uses `WeightedRatio` and `Full` preprocessing by default. Override these in the method parameters to use different scorers and processing:
```csharp
Process.ExtractOne("cowboys", new[] { "Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys" }, s => s, ScorerCache.Get<DefaultRatioScorer>());
// (string: Dallas Cowboys, score: 57, index: 3)
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
//(string: Dallas Cowboys, score: 67, index: 3)
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
var approximateSubstring = ScorerCache.Get<ApproximateSubstringRatioScorer>();
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
int score = scorer.Score("candidate string");
```

Available cached scorers:
- `CachedWeightedRatioScorer` -- weighted combination (default for `.Cached()`)
- `CachedDefaultRatioScorer` -- simple Levenshtein ratio
- `CachedApproximateSubstringRatioScorer` -- approximate substring Indel ratio
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

The static approximate-substring API returns the best raw Indel distance and its
endpoint:

```csharp
IndelSubstringMatch match = Indel.BestSubstringMatch(
    "invoice number 12345".AsSpan(),
    "processed invoice number 12345 successfully".AsSpan());
// match.FirstBestEndIndex identifies the first strict improvement (the final '5')
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

`Process` extraction methods use `StringPreprocessor.Full` by default. Pass `StringPreprocessor.None` (or a custom `processor` function) to override this behavior.

## Performance

The distance implementations use bit-parallel algorithms designed to reduce both execution time and allocations. The table below compares naive DP Levenshtein distance calculation (baseline), original FuzzySharp, Fastenshtein, Quickenshtein, and Raffinert.FuzzySharp.

Random words of 3 to 1024 random chars (`LevenshteinLarge.cs`):

| Method                                                          | Mean       | Error      | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|-----------------------------------------------------------------|------------|------------|-----------|-------|---------|------------|------------|-------------|-------------|
| NaiveDp                                                         | 231.563 ms | 57.5403 ms | 3.1540 ms |  1.00 |    0.02 | 43500.0000 | 34500.0000 | 275312920 B |       1.000 |
| [FuzzySharp](https://github.com/JakeBayer/FuzzySharp)           | 141.820 ms |  4.0905 ms | 0.2242 ms |  0.61 |    0.01 |          - |          - |   1545732 B |       0.006 |
| [Fastenshtein](https://github.com/DanHarltey/Fastenshtein)      | 123.356 ms | 13.0959 ms | 0.7178 ms |  0.53 |    0.01 |          - |          - |     34028 B |       0.000 |
| [Quickenshtein](https://github.com/Turnerj/Quickenshtein)       |  12.918 ms | 12.8046 ms | 0.7019 ms |  0.06 |    0.00 |          - |          - |        12 B |       0.000 |
| [Raffinert.FuzzySharp](https://github.com/Raffinert/FuzzySharp) |   4.970 ms |  0.3311 ms | 0.0181 ms |  0.02 |    0.00 |          - |          - |      3051 B |       0.000 |

Benchmark sources are in [FuzzySharp.Benchmarks](FuzzySharp.Benchmarks).

## Migration Notes

### From original FuzzySharp

The package name is `Raffinert.FuzzySharp`, and the default namespace is `Raffinert.FuzzySharp`. This package keeps the familiar FuzzyWuzzy-style API while adding faster distance implementations, fixed partial-ratio behavior, cached scorers, and the fluent process pipeline.

### From Raffinert.FuzzySharp v4 to v5

- `PreprocessMode`-based APIs were replaced with `Func<string, string>` preprocessors.
- Use `StringPreprocessor.Full`, `StringPreprocessor.None`, or a custom delegate instead of `PreprocessMode.Full` or `PreprocessMode.None`.
- Generic extraction methods that use extractor delegates are now named `Extract*By`, such as `ExtractOneBy` and `ExtractTopBy`.

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
