<source_code>
FuzzySharp/.filenesting.json
```
{
  "help": "https://go.microsoft.com/fwlink/?linkid=866610",
  "root": false,

  "pathSegment": {
    "add": {
      ".*": [
        ".cs"
      ]
    }
  }
}

```

FuzzySharp/CachedScorerProcessExecutor.cs
```
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Internal execution logic for cached Process operations with a pre-initialized scorer.
/// The scorer already has the query baked in — methods here take no query parameter.
/// Dispatches to sequential or parallel cached ResultExtractor paths.
/// </summary>
internal static class CachedScorerProcessExecutor
{
    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        ICachedRatioScorer scorer,
        int cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractWithoutOrder(
                choices, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractWithoutOrder(choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        ICachedRatioScorer scorer,
        int limit,
        int cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractTop(
                choices, processor, scorer, limit, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractTop(choices, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        ICachedRatioScorer scorer,
        int cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractSorted(
                choices, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractSorted(choices, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        ICachedRatioScorer scorer,
        int cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractOne(
                choices, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractOne(choices, processor, scorer, cutoff);
    }
}
```

FuzzySharp/Delegates.cs
```
﻿using System;

namespace Raffinert.FuzzySharp;

public delegate int Scorer(string input1, string input2);
public delegate int CachedScorer(string input2);
public delegate void Processor<T>(ref ReadOnlySpan<T> str) where T : IEquatable<T>;
```

FuzzySharp/Fuzz.cs
```
﻿using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp;

public static class Fuzz
{
    #region Ratio
    /// <summary>
    /// Calculates a Levenshtein simple ratio between the strings.
    /// This indicates a measure of similarity
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int Ratio(string input1, string input2)
    {
        return ScorerCache.Get<DefaultRatioScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Calculates a Levenshtein simple ratio between the strings.
    /// This indicates a measure of similarity
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int Ratio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<DefaultRatioScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion

    #region PartialRatio
    /// <summary>
    /// Inconsistent substrings lead to problems in matching. This ratio
    /// uses a heuristic called "best partial" for when two strings
    /// are of noticeably different lengths.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Inconsistent substrings lead to problems in matching. This ratio
    /// uses a heuristic called "best partial" for when two strings
    /// are of noticeably different lengths.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int PartialRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion

    #region TokenSortRatio
    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenSortRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenSortScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int TokenSortRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<TokenSortScorer>().Score(input1, input2, preprocessMode);
    }

    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenSortRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenSortScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Find all alphanumeric tokens in the string and sort
    /// those tokens and then take ratio of resulting
    /// joined strings.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int PartialTokenSortRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<PartialTokenSortScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion

    #region TokenSetRatio
    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenSetRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenSetScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int TokenSetRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<TokenSetScorer>().Score(input1, input2, preprocessMode);
    }

    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenSetRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenSetScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes intersections and remainders
    /// between the tokens of the two strings.A comparison string is then
    /// built up and is compared using the simple ratio algorithm.
    /// Useful for strings where words appear redundantly.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int PartialTokenSetRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<PartialTokenSetScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion

    #region TokenDifferenceRatio
    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenDifferenceRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenDifferenceScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int TokenDifferenceRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<TokenDifferenceScorer>().Score(input1, input2, preprocessMode);
    }

    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenDifferenceRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenDifferenceScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
    /// but the strings themselves)
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int PartialTokenDifferenceRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<PartialTokenDifferenceScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion

    #region TokenInitialismRatio
    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenInitialismRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int TokenInitialismRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2, preprocessMode);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenInitialismRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenInitialismScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Splits longer string into tokens and takes the initialism and compares it to the shorter
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int PartialTokenInitialismRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<PartialTokenInitialismScorer>().Score(input1, input2);
    }
    #endregion

    #region TokenAbbreviationRatio
    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int TokenAbbreviationRatio(string input1, string input2)
    {
        return ScorerCache.Get<TokenAbbreviationScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int TokenAbbreviationRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<TokenAbbreviationScorer>().Score(input1, input2, preprocessMode);
    }

    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int PartialTokenAbbreviationRatio(string input1, string input2)
    {
        return ScorerCache.Get<PartialTokenAbbreviationScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
    /// of the other strings tokens. One string must have all its characters in order in the other string
    /// to even be considered.
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int PartialTokenAbbreviationRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<PartialTokenAbbreviationScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion

    #region WeightedRatio
    /// <summary>
    /// Calculates a weighted ratio between the different algorithms for best results
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public static int WeightedRatio(string input1, string input2)
    {
        return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2);
    }

    /// <summary>
    /// Calculates a weighted ratio between the different algorithms for best results
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <param name="preprocessMode"></param>
    /// <returns></returns>
    public static int WeightedRatio(string input1, string input2, PreprocessMode preprocessMode)
    {
        return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2, preprocessMode);
    }
    #endregion
}
```

FuzzySharp/FuzzySharp.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <FileVersion>3.0.8.0</FileVersion>
    <Version>3.0.8</Version>
    <PackageVersion>3.0.8</PackageVersion>
    <AssemblyVersion>4.0.0.0</AssemblyVersion>
    <FileVersion>4.0.0.0</FileVersion>
    <Version>4.0.0</Version>
    <PackageVersion>4.0.0</PackageVersion>
    <Authors>Jacob Bayer;Yevhen Cherkes</Authors>
    <Company />
    <Description>
        Bit-parallel accelerated Fuzzy string matcher based on FuzzyWuzzy algorithm from SeatGeek and RapidFuzz python library from Max Bachmann.
    </Description>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <IncludeSymbols>true</IncludeSymbols>
    <LangVersion>Latest</LangVersion>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/Raffinert/FuzzySharp</PackageProjectUrl>
    <PackageRequireLicenseAcceptance>false</PackageRequireLicenseAcceptance>
    <PackageTags>Fuzzy String Matching Comparison FuzzyWuzzy FuzzySharp</PackageTags>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <RepositoryUrl>https://github.com/Raffinert/FuzzySharp</RepositoryUrl>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
    <TargetFrameworks>netstandard2.0;netstandard2.1;netcoreapp3.1;net45;net46;net462;net472;net48;NET60;NET80;NET90;NET10.0</TargetFrameworks>
    <AssemblyName>Raffinert.$(MSBuildProjectName)</AssemblyName>
    <RootNamespace>Raffinert.$(MSBuildProjectName.Replace(" ", "_"))</RootNamespace>
    <PackageReadmeFile>README.md</PackageReadmeFile>
  </PropertyGroup>

  <ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.0' OR '$(TargetFramework)' == 'netcoreapp3.1' OR '$(TargetFramework)' == 'net45' OR '$(TargetFramework)' == 'net46' OR '$(TargetFramework)' == 'net462' OR '$(TargetFramework)' == 'net472' OR '$(TargetFramework)' == 'net48'">
    <PackageReference Include="IndexRange" Version="1.0.3" />
    <PackageReference Include="System.Memory" Version="4.5.5" />
  </ItemGroup>

    <ItemGroup>
        <None Include="../README.md" pack="true" PackagePath="." />
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Meziantou.Polyfill" Version="1.0.49">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
    </ItemGroup>

    <PropertyGroup>
        <MeziantouPolyfill_IncludedPolyfills>M:System.Collections.Generic.CollectionExtensions.GetValueOrDefault</MeziantouPolyfill_IncludedPolyfills>
    </PropertyGroup>

    <ItemGroup>
        <InternalsVisibleTo Include="Raffinert.FuzzySharp.Test" />
    </ItemGroup>

</Project>
```

FuzzySharp/FuzzySharp.csproj.DotSettings
```
﻿<wpf:ResourceDictionary xml:space="preserve" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:s="clr-namespace:System;assembly=mscorlib" xmlns:ss="urn:shemas-jetbrains-com:settings-storage-xaml" xmlns:wpf="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio/@EntryIndexedValue">False</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer/@EntryIndexedValue">False</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer_005Cstrategysensitive_005Csimple/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer_005Cstrategysensitive_005Ctokenabbreviation/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer_005Cstrategysensitive_005Ctokendifference/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer_005Cstrategysensitive_005Ctokeninitialism/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer_005Cstrategysensitive_005Ctokenset/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=similarityratio_005Cscorer_005Cstrategysensitive_005Ctokensort/@EntryIndexedValue">True</s:Boolean></wpf:ResourceDictionary>
```

FuzzySharp/Indel.Instance.cs
```
﻿using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class Indel(string source) : IDisposable
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(string value)
    {
        return DistanceImpl(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public double NormalizedSimilarityWith(string value)
    {
        return NormalizedSimilarityImpl(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}

public sealed class IndelT<T>(T[] source) : IDisposable where T : IEquatable<T>
{
    private readonly T[] _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly IPatternMatchVector<T> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(T[] value)
    {
        return Indel.DistanceImpl<T>(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public double NormalizedSimilarityWith(T[] value)
    {
        return Indel.NormalizedSimilarityImpl(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}
```

FuzzySharp/Indel.Static.cs
```
﻿using Raffinert.FuzzySharp.Utils;
using System;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Provides static methods for calculating the Indel (insertion-deletion) distance and similarity between sequences.
/// Implements algorithms inspired by RapidFuzz's Indel distance implementation.
/// </summary>
public sealed partial class Indel
{
    /// <summary>
    /// Computes the Indel distance using precomputed block data for the first sequence.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="block">Precomputed per-symbol bitmasks for s1.</param>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold. If the distance exceeds this value, returns scoreCutoff + 1.</param>
    /// <returns>The Indel distance between the two sequences.</returns>
    public static int BlockDistance<T>(
        IPatternMatchVector<T> block,
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var maximum = s1.Length + s2.Length;
        var lcsSim = LongestCommonSubsequence.BlockSimilarity(block, s1, s2);
        var dist = maximum - 2 * lcsSim;
        var result = scoreCutoff == null || dist <= scoreCutoff.Value
            ? dist
            : scoreCutoff.Value + 1;
        return result;
    }

    /// <summary>
    /// Computes the normalized Indel distance using precomputed block data for the first sequence.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="block">Precomputed per-symbol bitmasks for s1.</param>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold. If the distance exceeds this value, returns 1.</param>
    /// <returns>The normalized Indel distance between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double BlockNormalizedDistance<T>(
        IPatternMatchVector<T> block,
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var maximum = s1.Length + s2.Length;
        var dist = BlockDistance(block, s1, s2);
        var normDist = maximum == 0 ? 0 : dist / (double)maximum;
        var result = scoreCutoff == null || normDist <= scoreCutoff.Value
            ? normDist
            : 1;
        return result;
    }

    /// <summary>
    /// Computes the normalized Indel similarity using precomputed block data for the first sequence.
    /// This is defined as 1 - BlockNormalizedDistance(block, s1, s2).
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="block">Precomputed per-symbol bitmasks for s1.</param>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold. If the similarity is below this value, returns 0.</param>
    /// <returns>The normalized Indel similarity between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double BlockNormalizedSimilarity<T>(
        IPatternMatchVector<T> block,
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var normDist = BlockNormalizedDistance(block, s1, s2);
        var normSim = 1.0 - normDist;
        var result = scoreCutoff == null || normSim >= scoreCutoff.Value
            ? normSim
            : 0;
        return result;
    }

    /// <summary>
    /// Computes the Indel distance (minimum number of insertions and deletions) between two sequences.
    /// This is defined as len(s1) + len(s2) - 2 * LCS(s1, s2).
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold. If the distance exceeds this value, returns scoreCutoff + 1.</param>
    /// <returns>The Indel distance between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Distance<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        SequenceUtils.TrimCommonAffixAndSwapIfNeeded(ref s1, ref s2);

        return DistanceImpl(s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DistanceImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        using var patternMatchVector = PatternMatchVector.Create(s1);
        return DistanceImpl(s1, s2, patternMatchVector, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int DistanceImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var maximum = s1.Length + s2.Length;
        var lcsSim = LongestCommonSubsequence.SimilarityImpl(s1, s2, patternMatchVector);
        var dist = maximum - 2 * lcsSim;
        var result = scoreCutoff == null || dist <= scoreCutoff.Value
            ? dist
            : scoreCutoff.Value + 1;

        return result;
    }

    /// <summary>
    /// Computes the normalized Indel distance between two sequences, in the range [0, 1].
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold. If the distance exceeds this value, returns scoreCutoff + 1.</param>
    /// <returns>The normalized Indel distance between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NormalizedDistance<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        SequenceUtils.TrimCommonAffixAndSwapIfNeeded(ref s1, ref s2);

        return NormalizedDistanceImpl(s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizedDistanceImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var dist = DistanceImpl(s1, s2, patternMatchVector, scoreCutoff);
        return NormalizedDistanceImpl(s1.Length, s2.Length, dist, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizedDistanceImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var dist = Distance(s1, s2);
        return NormalizedDistanceImpl(s1.Length, s2.Length, dist, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double NormalizedDistanceImpl(int s1Length, int s2Length, int distance,
        int? scoreCutoff = null)
    {
        var maximum = s1Length + s2Length;
        var normDist = maximum == 0 ? 0 : distance / (double)maximum;
        var result = scoreCutoff == null || normDist <= scoreCutoff.Value
            ? normDist
            : scoreCutoff.Value + 1;
        return result;
    }
    /// <summary>
    /// Computes the normalized Indel similarity between two sequences, in the range [0, 1].
    /// This is defined as 1 - NormalizedDistance(s1, s2).
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold. If the similarity is below this value, returns 0.</param>
    /// <returns>The normalized Indel similarity between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NormalizedSimilarity<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        return NormalizedSimilarityImpl(s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double NormalizedSimilarityImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,

        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var normDist = NormalizedDistanceImpl(s1, s2, patternMatchVector, scoreCutoff);
        var normSim = 1 - normDist;
        var result = scoreCutoff == null || normSim >= scoreCutoff.Value
            ? normSim
            : 0;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizedSimilarityImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var normDist = NormalizedDistanceImpl(s1, s2);
        var normSim = 1 - normDist;
        var result = scoreCutoff == null || normSim >= scoreCutoff.Value
            ? normSim
            : 0;
        return result;
    }
}
```

FuzzySharp/Levenshtein.Instance.cs
```
﻿using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class Levenshtein(string source) : IDisposable
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(string value)
    {
        return Distance(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}
```

FuzzySharp/Levenshtein.Static.cs
```
﻿using Raffinert.FuzzySharp.Edits;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Provides static methods for computing the Levenshtein distance and similarity between sequences.
/// Implements bit-parallel and dynamic programming algorithms inspired by RapidFuzz's Levenshtein implementation.
/// </summary>
public sealed partial class Levenshtein
{
    /// <summary>
    /// Computes the Levenshtein distance between two strings with custom operation costs and optional cutoff.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The Levenshtein distance.</returns>
    public static int Distance(
        string source, string target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null)
        => Distance(source.AsSpan(), target.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the Levenshtein distance between two sequences with custom operation costs and optional cutoff.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="source">Source sequence.</param>
    /// <param name="target">Target sequence.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The Levenshtein distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Distance<T>(
        ReadOnlySpan<T> source, ReadOnlySpan<T> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        SequenceUtils.TrimCommonAffixAndSwapIfNeeded(ref source, ref target);

        if (insertCost != 1 && deleteCost != 1 && replaceCost is not (1 or 2))
        {
            GenericDistance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff);
        }

        using var patternMatchVector = PatternMatchVector.Create(source);

        if (replaceCost == 1)
        {
            return scoreCutoff.HasValue
                ? Distance(source, target, scoreCutoff.Value, patternMatchVector)
                : Distance(source, target, patternMatchVector);
        }

        return Indel.DistanceImpl(source, target, patternMatchVector, scoreCutoff);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Distance<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, int scoreCutoff, IPatternMatchVector<T> patternMatchVector) where T : IEquatable<T>
    {
        if (source.Length <= 64)
        {
            return DistanceSingleULong(source, target, scoreCutoff, patternMatchVector);
        }

        return DistanceMultipleULongs(source, target, scoreCutoff, patternMatchVector);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Distance<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, IPatternMatchVector<T> patternMatchVector) where T : IEquatable<T>
    {
        if (source.Length <= 64)
        {
            return DistanceSingleULong(source, target, patternMatchVector);
        }

        return DistanceMultipleULongs(source, target, null, patternMatchVector);
    }

    /// <summary>
    /// Returns a list of matching blocks (contiguous matching subsequences) between s1 and s2 (special case for char spans).
    /// </summary>
    /// <param name="s1">First string.</param>
    /// <param name="s2">Second string.</param>
    /// <returns>List of matching blocks.</returns>
    public static List<MatchingBlock> GetMatchingBlocks(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        var editOps = GetEditOps(s1, s2);
        var matchingBlocks = editOps.AsMatchingBlocks(s1.Length, s2.Length);
        return matchingBlocks;
    }

    /// <summary>
    /// Returns a list of matching blocks (contiguous matching subsequences) between s1 and s2.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <returns>List of matching blocks.</returns>
    public static List<MatchingBlock> GetMatchingBlocks<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        var editOps = GetEditOps(s1, s2);
        var matchingBlocks = editOps.AsMatchingBlocks(s1.Length, s2.Length);
        return matchingBlocks;
    }

    /// <summary>
    /// Computes the sequence of edit operations (insert, delete, replace) to transform s1 into s2 using Levenshtein distance.
    /// </summary>
    /// <param name="s1">Source string.</param>
    /// <param name="s2">Target string.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreHint">Optional score hint (ignored).</param>
    /// <returns>Array of edit operations (EditOp).</returns>
    public static EditOp[] GetEditOps(
        string s1,
        string s2,
        Func<string, string> processor = null,
        int? scoreHint = null
    )
    {
        if (processor != null)
        {
            s1 = processor(s1);
            s2 = processor(s2);
        }

        return GetEditOps(s1.AsSpan(), s2.AsSpan(), scoreHint: scoreHint);
    }

    /// <summary>
    /// Computes the sequence of edit operations (insert, delete, replace) to transform s1 into s2 using Levenshtein distance.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">Source sequence.</param>
    /// <param name="s2">Target sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreHint">Optional score hint (ignored).</param>
    /// <returns>Array of edit operations (EditOp).</returns>
    public static EditOp[] GetEditOps<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreHint = null
    ) where T : IEquatable<T>
    {
        // 1) Optional preprocessing
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        // 2) For strings, conv_sequences is identity
        //    (for more general sequences you'd map items to ints)
        // 3) Strip off common prefix+suffix
        var (prefixLen, suffixLen) = SequenceUtils.TrimCommonAffix(ref s1, ref s2);

        // 4) Run the bit-parallel matrix
        var (dist, VPblocks, VNblocks) = Matrix(s1, s2);

        // 5) Initialize backtracking
        var originalDist = dist;
        var opsArray = new EditOp[originalDist];
        var col = s1.Length;
        var row = s2.Length;
        var nextIndex = originalDist; // we’ll decrement before placing

        // 6) If no edits, we’re done
        if (originalDist == 0)
            return [];

        // 7) Backtrack
        while (row > 0 && col > 0)
        {
            // determine which block & bit offset holds the (col-1) bit
            var bitPos = col - 1;
            var blk = bitPos / 64;
            var off = bitPos % 64;
            var bit = 1UL << off;

            // deletion?
            if ((VPblocks[row - 1][blk] & bit) != 0)
            {
                nextIndex--;
                col--;
                opsArray[nextIndex] = new EditOp
                {
                    EditType = EditType.DELETE,
                    SourcePos = col + prefixLen,
                    DestPos = row + prefixLen
                };
            }
            else
            {
                row--;
                // insertion?
                if (row > 0 && (VNblocks[row - 1][blk] & bit) != 0)
                {
                    nextIndex--;
                    opsArray[nextIndex] = new EditOp
                    {
                        EditType = EditType.INSERT,
                        SourcePos = col + prefixLen,
                        DestPos = row + prefixLen
                    };
                }
                else
                {
                    // move diagonally
                    col--;
                    // replace?
                    if (!EqualityComparer<T>.Default.Equals(s1[col], s2[row]))
                    {
                        nextIndex--;
                        opsArray[nextIndex] = new EditOp
                        {
                            EditType = EditType.REPLACE,
                            SourcePos = col + prefixLen,
                            DestPos = row + prefixLen
                        };
                    }
                }
            }
        }

        // any remaining deletes
        while (col > 0)
        {
            nextIndex--;
            col--;
            opsArray[nextIndex] = new EditOp
            {
                EditType = EditType.DELETE,
                SourcePos = col + prefixLen,
                DestPos = row + prefixLen
            };
        }
        // any remaining inserts
        while (row > 0)
        {
            nextIndex--;
            row--;
            opsArray[nextIndex] = new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = col + prefixLen,
                DestPos = row + prefixLen
            };
        }

        return opsArray;
    }

    /// <summary>
    /// Computes the maximum possible Levenshtein distance between two sequences given the operation costs.
    /// </summary>
    /// <param name="len1">Length of the first sequence.</param>
    /// <param name="len2">Length of the second sequence.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <returns>The maximum possible Levenshtein distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LevenshteinMaximum(int len1, int len2, int insertCost, int deleteCost, int replaceCost)
    {
        // Cost of deleting all of s1 then inserting all of s2
        var totalDelIns = len1 * deleteCost + len2 * insertCost;

        // Cost of replacing common prefix and handling extra characters
        var common = len1 < len2 ? len1 : len2;
        var extraCost = len1 >= len2
            ? len1 - len2 * deleteCost
            : len2 - len1 * insertCost;

        var replaceAndDiff = common * replaceCost + extraCost;

        // Return the smaller of the two worst-case scenarios
        return totalDelIns < replaceAndDiff ? totalDelIns : replaceAndDiff;
    }

    /// <summary>
    /// Computes the Myers bit-parallel VP/VN matrices and final edit distance for patterns of arbitrary length.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="pattern">Pattern sequence (any length).</param>
    /// <param name="text">Text sequence.</param>
    /// <returns>Tuple of (distance, VP matrix, VN matrix).</returns>
    public static (int Distance, List<ulong[]> VP, List<ulong[]> VN) MatrixMultipleULongs<T>(ReadOnlySpan<T> pattern, ReadOnlySpan<T> text) where T : IEquatable<T>
    {
        var m = pattern.Length;
        if (m == 0)
            return (text.Length, new List<ulong[]>(), new List<ulong[]>());

        // Number of 64‐bit blocks needed to cover the pattern
        var blocks = (m + 63) / 64;

        // Initial VP = all 1s in those m bits, VN = 0
        var VP = new ulong[blocks];
        var VN = new ulong[blocks];
        for (var i = 0; i < blocks; i++)
        {
            // For all but the last block, fill with 0xFFFFFFFFFFFFFFFF
            // For the last block, only the low (m % 64) bits are 1
            if (i < blocks - 1 || m % 64 == 0)
                VP[i] = ulong.MaxValue;
            else
                VP[i] = (1UL << (m % 64)) - 1;
            VN[i] = 0UL;
        }

        // Mask to extract the highest‐order bit of the full m‐bit vector
        var lastBlk = blocks - 1;
        var topBitPos = (m - 1) % 64;
        var topBitMask = 1UL << topBitPos;

        // Build the “block” table: for each character, which bit(s) in each block it sets
        using var blockTable = PatternMatchVector.Create(pattern);

        var currDist = m;
        var matrixVP = new List<ulong[]>();
        var matrixVN = new List<ulong[]>();

        // Temporary arrays for per‐character computation
        var D0 = new ulong[blocks];
        var HP = new ulong[blocks];
        var HN = new ulong[blocks];
        var sum = new ulong[blocks];
        var HPs = new ulong[blocks];
        var HNs = new ulong[blocks];

        // Process each character of the text
        for (var i = 0; i < text.Length; i++)
        {
            // 1) Load the pattern‐mask for c, or zeros if not present
            var X = blockTable.GetOrZero(text[i]);

            // 2) Compute D0 = (((X & VP) + VP) ^ VP) | X | VN
            //    -> Must do a big‐integer add and carry across blocks
            ulong carry = 0;
            for (var b = 0; b < blocks; b++)
            {
                var Pv = VP[b];
                var XandVP = X[b] & Pv;
                // big‐integer add: XandVP + Pv + carry
                var t = unchecked(XandVP + Pv);
                var c1 = t < XandVP ? 1UL : 0UL;          // carry from first add
                var t2 = unchecked(t + carry);
                var c2 = t2 < carry ? 1UL : 0UL;           // carry from second add
                carry = c1 | c2;

                sum[b] = t2;
                D0[b] = (sum[b] ^ Pv) | X[b] | VN[b];
            }

            // 3) HP = VN | ~(D0 | VP),   HN = D0 & VP
            for (var b = 0; b < blocks; b++)
            {
                HP[b] = VN[b] | ~(D0[b] | VP[b]);
                HN[b] = D0[b] & VP[b];
            }

            // 4) Update distance by inspecting the highest bit
            if ((HP[lastBlk] & topBitMask) != 0) currDist++;
            if ((HN[lastBlk] & topBitMask) != 0) currDist--;

            // 5) Shift HP and HN left by one over the entire multi‐block vector
            //    and set the low bit of HP[0] to 1
            ulong carryHP = 1, carryHN = 0;
            for (var b = 0; b < blocks; b++)
            {
                ulong hpb = HP[b], hnb = HN[b];
                var newCarryHP = hpb >> 63;
                var newCarryHN = hnb >> 63;
                HPs[b] = (hpb << 1) | carryHP;
                HNs[b] = (hnb << 1) | carryHN;
                carryHP = newCarryHP;
                carryHN = newCarryHN;
            }

            // 6) Recompute VP, VN
            for (var b = 0; b < blocks; b++)
            {
                VP[b] = HNs[b] | ~(D0[b] | HPs[b]);
                VN[b] = HPs[b] & D0[b];
            }

            // 7) Keep a snapshot of VP/VN for this character
            matrixVP.Add((ulong[])VP.Clone());
            matrixVN.Add((ulong[])VN.Clone());
        }

        return (currDist, matrixVP, matrixVN);
    }

    /// <summary>
    /// Computes the Myers bit-parallel VP/VN matrices and final edit distance for patterns up to 64 elements.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">Pattern sequence (≤ 64 elements).</param>
    /// <param name="s2">Text sequence.</param>
    /// <returns>Tuple of (distance, VP matrix, VN matrix).</returns>
    public static (int Distance, List<ulong[]> VP, List<ulong[]> VN) MatrixSingleULong<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        if (s1.IsEmpty)
            return (s2.Length, [], []);

        if (s1.Length > 64)
            throw new ArgumentException("Pattern too long for 64-bit bit-parallel algorithm.", nameof(s1));

        // Initial bitmasks
        var VP = (1UL << s1.Length) - 1;
        ulong VN = 0;
        var currDist = s1.Length;
        var mask = 1UL << (s1.Length - 1);

        // Build the “block” table: for each character in s1, which bit(s) it sets
        using var blockTable = PatternMatchVector.Create(s1);

        var matrixVP = new List<ulong[]>();
        var matrixVN = new List<ulong[]>();

        for (var i = 0; i < s2.Length; i++)
        {
            var PMj = blockTable.GetOrZero(s2[i])[0];

            // Step 1: D0 = (((PMj & VP) + VP) ^ VP) | PMj | VN
            // Use unchecked so addition wraps modulo 2^64
            var X = PMj;
            var D0 = unchecked(((X & VP) + VP) ^ VP) | X | VN;

            // Step 2: HP = VN | ~(D0 | VP);  HN = D0 & VP
            var HP = VN | ~(D0 | VP);
            var HN = D0 & VP;

            // Step 3: adjust distance by looking at the high bit
            if ((HP & mask) != 0) currDist++;
            if ((HN & mask) != 0) currDist--;

            // Step 4: shift and recompute VP, VN
            HP = (HP << 1) | 1UL;
            HN <<= 1;
            VP = HN | ~(D0 | HP);
            VN = HP & D0;

            matrixVP.Add([VP]);
            matrixVN.Add([VN]);
        }

        return (currDist, matrixVP, matrixVN);
    }

    /// <summary>
    /// Computes the normalized Levenshtein distance in [0, 1].
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold.</param>
    /// <returns>Normalized distance (0 = identical, 1 = completely different).</returns>
    public static double NormalizedDistance(
        ReadOnlySpan<char> source, ReadOnlySpan<char> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
    {
        int len1 = source.Length, len2 = target.Length;
        if (len1 == 0 && len2 == 0) return 0.0;
        var maximum = LevenshteinMaximum(len1, len2, insertCost, deleteCost, replaceCost);
        if (maximum == 0) return 0.0;

        var dist = Distance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff.HasValue ? (int?)Math.Floor(scoreCutoff.Value * maximum) : null);
        var nd = dist / (double)maximum;
        return nd > scoreCutoff ? 1.0 : nd;
    }

    /// <summary>
    /// Computes the normalized Levenshtein distance for two strings.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold.</param>
    /// <returns>Normalized distance (0 = identical, 1 = completely different).</returns>
    public static double NormalizedDistance(
        string source, string target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
        => NormalizedDistance(source.AsSpan(), target.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the normalized Levenshtein similarity in [0, 1] (1 - normalized distance).
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum normalized similarity threshold.</param>
    /// <returns>Normalized similarity (1 = identical, 0 = completely different).</returns>
    public static double NormalizedSimilarity(
        ReadOnlySpan<char> source, ReadOnlySpan<char> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
    {
        var nd = NormalizedDistance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff);
        var ns = 1.0 - nd;

        return ns < scoreCutoff ? 0.0 : ns;
    }

    /// <summary>
    /// Computes the normalized Levenshtein similarity for two strings.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum normalized similarity threshold.</param>
    /// <returns>Normalized similarity (1 = identical, 0 = completely different).</returns>
    public static double NormalizedSimilarity(
        string source, string target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
        => NormalizedSimilarity(source.AsSpan(), target.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the Levenshtein similarity (maximum possible distance minus actual distance).
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold.</param>
    /// <returns>The Levenshtein similarity score.</returns>
    public static int Similarity(
        ReadOnlySpan<char> source, ReadOnlySpan<char> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null)
    {
        int len1 = source.Length, len2 = target.Length;
        var maximum = LevenshteinMaximum(len1, len2, insertCost, deleteCost, replaceCost);
        var dist = Distance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff);
        var sim = maximum - dist;
        return sim < scoreCutoff ? 0 : sim;
    }

    /// <summary>
    /// Computes the Levenshtein similarity for two strings.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="s2">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold.</param>
    /// <returns>The Levenshtein similarity score.</returns>
    public static int Similarity(
        string source, string s2,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null)
        => Similarity(source.AsSpan(), s2.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the Levenshtein distance between two sequences with custom operation costs using a dynamic programming approach.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="source">Source sequence.</param>
    /// <param name="target">Target sequence.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The Levenshtein distance.</returns>
    private static int GenericDistance<T>(
        ReadOnlySpan<T> source, ReadOnlySpan<T> target,
        int insertCost, int deleteCost, int replaceCost,
        int? scoreCutoff) where T : IEquatable<T>
    {
        var len1 = source.Length;
        // allocate a single row of len1+1
        Span<int> row = new int[len1 + 1];
        // initial: cost of deleting all of s1's prefix
        for (var i = 0; i <= len1; i++)
            row[i] = i * deleteCost;

        for (var index = 0; index < target.Length; index++)
        {
            var prev = row[0];
            row[0] += insertCost;
            for (var i = 0; i < len1; i++)
            {
                var curr = row[i + 1];
                var cost = prev;
                if (!EqualityComparer<T>.Default.Equals(source[i], target[index]))
                {
                    var del = row[i] + deleteCost;
                    var ins = row[i + 1] + insertCost;
                    var rep = prev + replaceCost;
                    cost = del < ins
                        ? del < rep ? del : rep
                        : ins < rep
                            ? ins
                            : rep;
                }

                prev = curr;
                row[i + 1] = cost;
            }

            if (scoreCutoff.HasValue && row[len1] > scoreCutoff.Value)
                return scoreCutoff.Value + 1;
        }

        return row[len1];
    }

    /// <summary>
    /// Computes the Myers bit-parallel VP/VN matrices and final edit distance, dispatching to the appropriate implementation.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">Pattern sequence.</param>
    /// <param name="s2">Text sequence.</param>
    /// <returns>Tuple of (distance, VP matrix, VN matrix).</returns>
    private static (int Distance, List<ulong[]> VP, List<ulong[]> VN) Matrix<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        return s1.Length <= 64
            ? MatrixSingleULong(s1, s2)
            : MatrixMultipleULongs(s1, s2);
    }

    /// <summary>
    /// Computes the Levenshtein distance (Myers’s bit‐parallel over >64 bits), with an optional cutoff.
    /// Uses a dictionary to store per‐character bitmasks rented from ArrayPool, and uses stackalloc if
    /// 6*blocks ≤ STACKALLOC_THRESHOLD_ULONGS; otherwise allocates a new ulong[] on the heap for the six lanes.
    /// </summary>
    private static int DistanceMultipleULongs<T>(
        ReadOnlySpan<T> source,
        ReadOnlySpan<T> target,
        int? scoreCutoff,
        IPatternMatchVector<T> patternMatchVector
    ) where T : IEquatable<T>
    {
        var m = source.Length;
        if (m == 0)
        {
            var d = target.Length;
            return d > scoreCutoff
                ? scoreCutoff.Value + 1
                : d;
        }

        // Number of 64‐bit blocks needed to cover pattern length m
        var blocks = (m + 63) >> 6;
        var totalScratch = 6 * blocks;
        var scratchArray = ArrayPool<ulong>.Shared.Rent(totalScratch);
        try
        {
            var result = DistanceMultipleULongsImpl(target, scoreCutoff, m, blocks, patternMatchVector, scratchArray);
            return result;
        }
        finally
        {
            ArrayPool<ulong>.Shared.Return(scratchArray);
        }
    }


    // ─────────────────────────────────────────────────────────────────────────────
    // Shared implementation that uses the precomputed dictionary of masks.
    // 'scratch' must have Length == 6 * blocks. It is partitioned into six lanes:
    //   scratch[0..blocks)       → VP
    //   scratch[blocks..2*blocks)→ VN
    //   scratch[2*blocks..3*blocks)→ X
    //   scratch[3*blocks..4*blocks)→ D0
    //   scratch[4*blocks..5*blocks)→ HP
    //   scratch[5*blocks..6*blocks)→ HN
    // ─────────────────────────────────────────────────────────────────────────────
    private static int DistanceMultipleULongsImpl<T>(ReadOnlySpan<T> target,
        int? scoreCutoff,
        int m,
        int blocks,
        IPatternMatchVector<T> patternMatchVector,
        Span<ulong> scratch
    ) where T : IEquatable<T>
    {
        // Partition scratch into six spans of length = blocks
        var VP = scratch.Slice(0 * blocks, blocks);
        var VN = scratch.Slice(1 * blocks, blocks);
        var X = scratch.Slice(2 * blocks, blocks);
        var D0 = scratch.Slice(3 * blocks, blocks);
        var HP = scratch.Slice(4 * blocks, blocks);
        var HN = scratch.Slice(5 * blocks, blocks);

        // Initialize VP (low m bits = 1) and VN = 0
        for (var b = 0; b < blocks; b++)
        {
            if (b < blocks - 1)
            {
                VP[b] = ulong.MaxValue;
            }
            else
            {
                var rem = m - ((blocks - 1) << 6);
                VP[b] = (rem == 64) ? ulong.MaxValue : ((1UL << rem) - 1);
            }
            VN[b] = 0UL;
        }

        var last = blocks - 1;
        var highestBitMask = 1UL << ((m - 1) & 63);
        var dist = m;

        for (var i = 0; i < target.Length; i++)
        {
            // Look up the precomputed bitmask array, or use zeroMask if not found
            var PMitem = patternMatchVector.GetOrZero(target[i]);

            // “D0‐loop” with carry across blocks
            var carry = 0UL;
            for (var b = 0; b < blocks; b++)
            {
                var pm = PMitem[b];
                var vp = VP[b];
                var vn = VN[b];
                var x = pm | vn;
                X[b] = x;
                var tmp = x & vp;

                // tmp + vp
                var sum1 = tmp + vp;
                var c1 = (sum1 < tmp) ? 1UL : 0UL;
                // sum1 + carry
                var sum = sum1 + carry;
                var c2o = (sum < sum1) ? 1UL : 0UL;
                carry = c1 | c2o;

                // D0 = (sum ^ vp) | x
                var d0 = (sum ^ vp) | x;
                D0[b] = d0;

                // HP = vn | ~(d0 | vp)
                // HN = d0 & vp
                HP[b] = vn | ~(d0 | vp);
                HN[b] = d0 & vp;
            }

            // Update distance by checking top bit of last block
            if ((HP[last] & highestBitMask) != 0UL) dist++;
            if ((HN[last] & highestBitMask) != 0UL) dist--;
            if (scoreCutoff.HasValue && dist > scoreCutoff.Value)
            {
                return scoreCutoff.Value + 1;
            }

            // Shift HP/HN left by 1 (with cross‐block carry), then compute new VP/VN
            var carryHP = 1UL;
            var carryHN = 0UL;
            for (var b = 0; b < blocks; b++)
            {
                var hp = HP[b];
                var hn = HN[b];

                var hpHigh = hp >> 63;
                var hnHigh = hn >> 63;

                hp = (hp << 1) | carryHP;
                hn = (hn << 1) | carryHN;

                var d0 = D0[b];
                VP[b] = hn | ~(d0 | hp);
                VN[b] = hp & d0;

                carryHP = hpHigh;
                carryHN = hnHigh;
            }
        }

        return dist;
    }

    private static int DistanceSingleULong<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, int scoreCutoff, IPatternMatchVector<T> patternMatchVector) where T : IEquatable<T>
    {
        var m = source.Length;
        if (m == 0) return target.Length;

        // initial bitmask: lower m bits set
        var VP = m < 64 ? (1UL << m) - 1 : ulong.MaxValue;
        ulong VN = 0;
        var highestBit = 1UL << (m - 1);
        var dist = m;

        for (var i = 0; i < target.Length; i++)
        {
            var PM = patternMatchVector.GetOrZero(target[i])[0];

            // Myers bit-parallel update
            var X = PM | VN;
            var D0 = (((X & VP) + VP) ^ VP) | X;
            D0 |= VN;
            var HP = VN | ~(D0 | VP);
            var HN = D0 & VP;

            if ((HP & highestBit) != 0) dist++;
            if ((HN & highestBit) != 0) dist--;

            if (dist > scoreCutoff)
                return scoreCutoff + 1;

            // shift in
            HP = (HP << 1) | 1;
            HN <<= 1;
            VP = HN | ~(D0 | HP);
            VN = HP & D0;
        }

        return dist;
    }

    private static int DistanceSingleULong<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, IPatternMatchVector<T> patternMatchVector) where T : IEquatable<T>
    {
        var m = source.Length;
        if (m == 0) return target.Length;

        // initial bitmask: lower m bits set
        var VP = m < 64 ? (1UL << m) - 1 : ulong.MaxValue;
        ulong VN = 0;
        var highestBit = 1UL << (m - 1);
        var dist = m;

        for (var i = 0; i < target.Length; i++)
        {
            var PM = patternMatchVector.GetOrZero(target[i])[0];

            // Myers bit-parallel update
            var X = PM | VN;
            var D0 = (((X & VP) + VP) ^ VP) | X;
            D0 |= VN;
            var HP = VN | ~(D0 | VP);
            var HN = D0 & VP;

            if ((HP & highestBit) != 0) dist++;
            if ((HN & highestBit) != 0) dist--;

            // shift in
            HP = (HP << 1) | 1;
            HN <<= 1;
            VP = HN | ~(D0 | HP);
            VN = HP & D0;
        }

        return dist;
    }
}
```

FuzzySharp/LongestCommonSubsequence.Instance.cs
```
﻿using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class LongestCommonSubsequence(string source) : IDisposable
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(string value)
    {
        return DistanceImpl(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}
```

FuzzySharp/LongestCommonSubsequence.Static.cs
```
﻿using Raffinert.FuzzySharp.Edits;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Provides static methods for computing the Longest Common Subsequence (LCS) and related similarity metrics.
/// Implements a bit-parallel LCS algorithm inspired by RapidFuzz's LCSseq implementation.
/// </summary>
public sealed partial class LongestCommonSubsequence
{
    /// <summary>
    /// Computes the LCS-based distance between two sequences.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The LCS distance (max(len1, len2) - LCS length).</returns>
    public static int Distance<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        using var patternMatchVector = PatternMatchVector.Create(s1);

        return DistanceImpl(s1, s2, patternMatchVector, scoreCutoff);
    }

    private static int DistanceImpl<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        int maximum = Math.Max(s1.Length, s2.Length);
        int sim = SimilarityImpl(s1, s2, patternMatchVector);
        int dist = maximum - sim;

        var result = scoreCutoff == null || dist <= scoreCutoff.Value
            ? dist
            : scoreCutoff.Value + 1;

        return result;
    }

    /// <summary>
    /// Computes the sequence of edit operations (insert, delete) to transform s1 into s2 using LCS.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>Array of edit operations (EditOp).</returns>
    public static EditOp[] GetEditOps<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        // strip any common prefix/suffix if you like
        var (prefixLen, _) = SequenceUtils.TrimCommonAffix(ref s1, ref s2);

        // now Matrix returns List<ulong[]> — one ulong[] per char of s2
        var (sim, matrix) = Matrix(s1, s2);

        int dist = s1.Length + s2.Length - 2 * sim;
        if (dist == 0)
            return [];

        var opsArray = new EditOp[dist];
        int nextIndex = dist;

        int row = s2.Length, col = s1.Length;

        while (row > 0 && col > 0)
        {
            // pick up the bit-mask vector for the previous row
            var bits = matrix[row - 1];

            // compute which block and which bit in that block is "col-1"
            int bitIndex = col - 1;
            int block = bitIndex / 64;
            int offset = bitIndex % 64;
            ulong mask = 1UL << offset;

            // if bit is set ⇒ this was a deletion in LCS fallback
            if ((bits[block] & mask) != 0)
            {
                nextIndex--;
                col--;
                opsArray[nextIndex] = new EditOp
                {
                    EditType = EditType.DELETE,
                    SourcePos = col + prefixLen,
                    DestPos = row + prefixLen
                };
            }
            else
            {
                // no deletion ⇒ move up a row
                row--;

                // but if still in-bounds and the bit is still zero ⇒ insertion
                if (row > 0)
                {
                    bits = matrix[row - 1];
                    if ((bits[block] & mask) == 0)
                    {
                        nextIndex--;
                        opsArray[nextIndex] = new EditOp
                        {
                            EditType = EditType.INSERT,
                            SourcePos = col + prefixLen,
                            DestPos = row + prefixLen
                        };
                        continue;
                    }
                }

                // otherwise it was a match/move-left in LCS
                col--;
            }
        }

        // any remaining deletes on the left edge
        while (col > 0)
        {
            nextIndex--;
            col--;
            opsArray[nextIndex] = new EditOp
            {
                EditType = EditType.DELETE,
                SourcePos = col + prefixLen,
                DestPos = row + prefixLen
            };
        }
        // any remaining inserts on the top edge
        while (row > 0)
        {
            nextIndex--;
            row--;
            opsArray[nextIndex] = new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = col + prefixLen,
                DestPos = row + prefixLen
            };
        }

        return opsArray;
    }

    /// <summary>
    /// Returns a list of matching blocks (contiguous matching subsequences) between s1 and s2.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>List of matching blocks.</returns>
    public static List<MatchingBlock> MatchingBlocks<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        var editOps = GetEditOps(s1, s2, processor);
        var matchingBlocks = editOps.AsMatchingBlocks(s1.Length, s2.Length);
        return matchingBlocks;
    }

    /// <summary>
    /// Computes the bit-parallel LCS matrix for the given sequences.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <returns>Tuple of (LCS length, matrix of bitmasks per row).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Sim, List<ulong[]> Matrix) Matrix<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        return s1.Length <= 64
            ? MatrixSingleULong(s1, s2)
            : MatrixMultipleULongs(s1, s2);
    }

    /// <summary>
    /// Computes the normalized LCS-based distance in [0, 1].
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold.</param>
    /// <returns>Normalized distance (0 = identical, 1 = completely different).</returns>
    public static double NormalizedDistance<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if ((s1.IsEmpty && !s2.IsEmpty) || (!s1.IsEmpty && s2.IsEmpty))
        {
            return 1;
        }

        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        if (s1.IsEmpty || s2.IsEmpty)
            return 0.0;

        int maximum = Math.Max(s1.Length, s2.Length);
        double normSim = Distance(s1, s2) / (double)maximum;

        var result = !scoreCutoff.HasValue || normSim <= scoreCutoff.Value
            ? normSim
            : 1.0;

        return result;
    }

    /// <summary>
    /// Computes the normalized LCS-based similarity in [0, 1].
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional minimum normalized similarity threshold.</param>
    /// <returns>Normalized similarity (1 = identical, 0 = completely different).</returns>
    public static double NormalizedSimilarity<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {

        if (s1.IsEmpty || s2.IsEmpty)
        {
            return 0;
        }

        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        double normSim = 1.0 - NormalizedDistance(s1, s2);

        var result = !scoreCutoff.HasValue || normSim >= scoreCutoff.Value
            ? normSim
            : 0.0;

        return result;
    }

    /// <summary>
    /// Returns a list of opcodes describing how to turn s1 into s2 using LCS.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>List of opcodes (OpCode).</returns>
    public static List<OpCode> Opcodes<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        var editOps = GetEditOps(s1, s2, processor);
        var opCodes = editOps.AsOpCodes(s1.Length, s2.Length);
        return opCodes;
    }

    /// <summary>
    /// Computes the length of the longest common subsequence (LCS) between two sequences.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold.</param>
    /// <returns>The length of the LCS, or 0 if below cutoff.</returns>
    public static int Similarity<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        using var patternMatchVector = PatternMatchVector.Create(s1);

        return SimilarityImpl(s1, s2, patternMatchVector, scoreCutoff);
    }

    internal static int SimilarityImpl<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var sim = s1.Length > 64
            ? BlockSimilarityMultipleULongs(patternMatchVector, s1, s2)
            : BlockSimilaritySingleULong(patternMatchVector, s1, s2);

        var result = scoreCutoff == null || sim >= scoreCutoff.Value
            ? sim
            : 0;

        return result;
    }

    /// <summary>
    /// Computes the LCS similarity using a bit-parallel algorithm for sequences that can be longer than 64 elements.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="block">Precomputed per-symbol bitmasks for s1.</param>
    /// <param name="s1">First sequence (pattern).</param>
    /// <param name="s2">Second sequence (text).</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold.</param>
    /// <returns>The length of the longest common subsequence, or 0 if below cutoff.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BlockSimilarity<T>(
        IPatternMatchVector<T> block,
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null
    ) where T : IEquatable<T>
    {
        return s1.Length <= 64
            ? BlockSimilaritySingleULong(block, s1, s2, scoreCutoff)
            : BlockSimilarityMultipleULongs(block, s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BlockSimilaritySingleULong<T>(
        IPatternMatchVector<T> block,
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null
    ) where T : IEquatable<T>
    {
        if (s1.IsEmpty)
            return 0;

        int len1 = s1.Length;
        ulong mask = len1 == 64 ? ulong.MaxValue : (1UL << len1) - 1UL;

        ulong S = mask;
        
        for (int i = 0; i < s2.Length; i++)
        {
            ulong M = block.GetOrZero(s2[i])[0];
            ulong u = S & M;
            unchecked
            {
                S = (S + u) | (S - u);
            }
        }

        int lcs = CountZeroBits(S, len1);
        return scoreCutoff == null || lcs >= scoreCutoff.Value
            ? lcs
            : 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BlockSimilarityMultipleULongs<T>(
        IPatternMatchVector<T> block,
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null
    ) where T : IEquatable<T>
    {
        if (s1.IsEmpty)
            return 0;

        int len1 = s1.Length;
        int segCount = (len1 + 63) / 64;

        var scratch = ArrayPool<ulong>.Shared.Rent(segCount * 4);
        try
        {
            var S = scratch.AsSpan(0, segCount);
            var u = scratch.AsSpan(segCount, segCount);
            var add = scratch.AsSpan(segCount * 2, segCount);
            var sub = scratch.AsSpan(segCount * 3, segCount);

            // --- 2) prepare the \"all-ones up to len1\" mask and state S ---
            S.Fill(ulong.MaxValue);
            int rem = len1 & 63;
            if (rem != 0)
                S[segCount - 1] = (1UL << rem) - 1;

            // --- 3) main bit-parallel loop: S = (S + u) | (S - u)  ---
            for (int chIdx = 0; chIdx < s2.Length; chIdx++)
            {
                var M = block.GetOrZero(s2[chIdx]);

                // u = S & M
                for (int i = 0; i < segCount; i++)
                    u[i] = S[i] & M[i];

                // add = S + u  (multi-precision)
                ulong carry = 0;
                for (int i = 0; i < segCount; i++)
                {
                    ulong sum = S[i] + u[i] + carry;
                    carry = sum < S[i] || (carry == 1 && sum == S[i]) ? 1UL : 0UL;
                    add[i] = sum;
                }

                // sub = S - u  (multi-precision)
                ulong borrow = 0;
                for (int i = 0; i < segCount; i++)
                {
                    ulong diff = S[i] - u[i] - borrow;
                    borrow = S[i] < u[i] + borrow ? 1UL : 0UL;
                    sub[i] = diff;
                }

                // new S = add | sub
                for (int i = 0; i < segCount; i++)
                    S[i] = add[i] | sub[i];
            }

            // --- 4) count zero bits in the lower len1 positions of S ---
            int lcs = CountZeroBits(S, len1);
            return scoreCutoff == null || lcs >= scoreCutoff.Value
                ? lcs
                : 0;
        }
        finally
        {
            ArrayPool<ulong>.Shared.Return(scratch);
        }
    }

    private static int CountZeroBits(ulong x, int length)
    {
        // invert and mask
        ulong inv = ~x & (length == 64 ? ulong.MaxValue : (1UL << length) - 1UL);
        return Polyfill.PopCount(inv);
    }

    private static int CountZeroBits(ReadOnlySpan<ulong> S, int length)
    {
        int fullBlocks = length / 64;
        int remBits = length % 64;
        int zeros = 0;

        // all full blocks
        for (int i = 0; i < fullBlocks; i++)
            zeros += Polyfill.PopCount(~S[i]);

        // last partial block
        if (remBits > 0)
        {
            ulong mask = (1UL << remBits) - 1;
            zeros += Polyfill.PopCount(~S[fullBlocks] & mask);
        }

        return zeros;
    }

    private static (int Sim, List<ulong[]> Matrix) MatrixMultipleULongs<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        int m = s1.Length;
        if (m == 0)
            return (0, new List<ulong[]>(s2.Length));

        int blocks = (m + 64 - 1) / 64;
        // initialize S[] = all-1 in low m bits
        var S = new ulong[blocks];
        for (int i = 0; i < blocks; i++)
        {
            if (i < blocks - 1 || m % 64 == 0)
                S[i] = ulong.MaxValue;
            else
                S[i] = (1UL << (m % 64)) - 1;
        }

        // build blockTable: element → bit-mask array
        using var blockTable = PatternMatchVector.Create(s1);

        var matrix = new List<ulong[]>(s2.Length);
        var Sum = new ulong[blocks];
        var Diff = new ulong[blocks];

        foreach (var y in s2)
        {
            // load mask for y
            var U = blockTable.GetOrZero(y);

            // big-integer add: Sum = S + U
            ulong carry = 0;
            for (int b = 0; b < blocks; b++)
            {
                ulong s = S[b], u = U[b];
                ulong t = unchecked(s + u);
                ulong c1 = t < s ? 1UL : 0UL;
                ulong t2 = unchecked(t + carry);
                ulong c2 = t2 < t ? 1UL : 0UL;
                Sum[b] = t2;
                carry = c1 | c2;
            }

            // big-integer subtract: Diff = S - U
            ulong borrow = 0;
            for (int b = 0; b < blocks; b++)
            {
                ulong s = S[b], u = U[b];
                ulong t1 = unchecked(s - u);
                ulong b1 = s < u ? 1UL : 0UL;
                ulong t2 = unchecked(t1 - borrow);
                ulong b2 = t1 < borrow ? 1UL : 0UL;
                Diff[b] = t2;
                borrow = b1 | b2;
            }

            // update S = Sum | Diff
            for (int b = 0; b < blocks; b++)
                S[b] = Sum[b] | Diff[b];

            // snapshot row
            matrix.Add((ulong[])S.Clone());
        }

        int sim = CountZeroBits(S, m);
        return (sim, matrix);
    }

    private static (int Sim, List<ulong[]> Matrix) MatrixSingleULong<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        if (s1.IsEmpty)
            return (0, new List<ulong[]>(s2.Length));

        int m = s1.Length;
        ulong S = m == 64 ? ulong.MaxValue : (1UL << m) - 1UL;

        // build bit-mask
        using var block = PatternMatchVector.Create(s1);

        var matrix = new List<ulong[]>(s2.Length);
        foreach (var y in s2)
        {
            var M = block.GetOrZero(y)[0];

            ulong u = S & M;

            unchecked { S = (S + u) | (S - u); }
            matrix.Add([S]);
        }

        int sim = CountZeroBits(S, m);
        return (sim, matrix);
    }
}
```

FuzzySharp/Process.cs
```
using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

public static class Process
{
    internal static readonly IRatioScorer DefaultScorer = ScorerCache.Get<WeightedRatioScorer>();
    internal static readonly Func<string, string> DefaultStringProcessor = StringPreprocessorFactory.GetPreprocessor(PreprocessMode.Full);

    /// <summary>
    /// Creates a new fluent builder for configuring a fuzzy string matching pipeline.
    /// Supports caching, parallel execution, and custom configuration.
    /// </summary>
    /// <returns>A new ProcessBuilder instance</returns>
    public static ProcessBuilder Configure() => new ProcessBuilder();

    #region ExtractAll
    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<string>> ExtractAll(
        string query, 
        IEnumerable<string> choices, 
        Func<string, string> processor = null, 
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query, 
        IEnumerable<T> choices,
        Func<T, string> processor,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }
    
    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    #endregion

    #region ExtractTop
    /// <summary>
    /// Creates a sorted list of ExtractedResult  which contain the
    /// top limit most similar choices
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="limit"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<string>> ExtractTop(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int limit = 5,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult  which contain the
    /// top limit most similar choices
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="limit"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        T query, 
        IEnumerable<T> choices,
        Func<T, string> processor,
        IRatioScorer scorer = null,
        int limit = 5, 
        int cutoff = 0)
    {
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    #endregion

    #region ExtractSorted
    
    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<string>> ExtractSorted(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    #endregion

    #region ExtractOne
    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static ExtractedResult<string> ExtractOne(
        string query, 
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static ExtractedResult<T> ExtractOne<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <returns></returns>
    public static ExtractedResult<string> ExtractOne(string query, params string[] choices)
    {
        return ResultExtractor.ExtractOne(query, choices, DefaultStringProcessor, DefaultScorer);
    }

    #endregion
}
```

FuzzySharp/ProcessBuilder.cs
```
using System;
using System.Threading.Tasks;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Fluent builder for configuring fuzzy string matching pipelines.
/// Call <see cref="Cached()"/> to enable auto-caching, or <see cref="Cached(ICachedRatioScorer)"/>
/// to provide an external cached scorer.
/// </summary>
public sealed class ProcessBuilder
{
    private bool _useParallel;
    private ParallelOptions _parallelOptions;
    private IRatioScorer _scorer;

    /// <summary>
    /// Enables caching mode with automatic scorer creation per extraction call.
    /// Returns a <see cref="CachedProcessBuilder"/> for further configuration.
    /// Order independent - can be called before or after Parallel().
    /// </summary>
    public CachedProcessBuilder Cached()
    {
        return new CachedProcessBuilder(_useParallel, _parallelOptions);
    }

    /// <summary>
    /// Enables caching mode with an external cached scorer instance for across-run caching.
    /// Returns a <see cref="CachedScorerProcessBuilder"/> for further configuration.
    /// When using with parallel execution, the caller is responsible for ensuring the scorer is thread-safe.
    /// Order independent - can be called before or after Parallel().
    /// </summary>
    /// <param name="scorer">The external cached scorer instance. Must not be null.</param>
    public CachedScorerProcessBuilder Cached(ICachedRatioScorer scorer)
    {
        if (scorer == null) throw new ArgumentNullException(nameof(scorer));
        return new CachedScorerProcessBuilder(scorer, _useParallel, _parallelOptions);
    }

    /// <summary>
    /// Enables parallel execution for improved performance on multi-core systems.
    /// Order independent - can be called before or after Cached().
    /// </summary>
    /// <param name="parallelOptions">Optional parallel execution options (max degree of parallelism, cancellation token, etc.)</param>
    public ProcessBuilder Parallel(ParallelOptions parallelOptions = null)
    {
        _useParallel = true;
        if (parallelOptions != null)
        {
            _parallelOptions = parallelOptions;
        }
        return this;
    }

    /// <summary>
    /// Configures parallel execution options (max degree of parallelism, cancellation token, etc.).
    /// Implicitly enables parallel mode.
    /// </summary>
    /// <param name="parallelOptions">Parallel execution options</param>
    public ProcessBuilder WithParallelOptions(ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions ?? throw new ArgumentNullException(nameof(parallelOptions));
        _useParallel = true;
        return this;
    }

    /// <summary>
    /// Sets the scoring algorithm for fuzzy matching.
    /// If not set, defaults to <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.WeightedRatioScorer"/>.
    /// </summary>
    /// <param name="scorer">The ratio scorer instance to use</param>
    public ProcessBuilder WithScorer(IRatioScorer scorer)
    {
        _scorer = scorer ?? throw new ArgumentNullException(nameof(scorer));
        return this;
    }

    /// <summary>
    /// Builds an immutable ProcessPipeline with the configured options.
    /// </summary>
    public ProcessPipeline Build()
    {
        return new ProcessPipeline(new ProcessOptions(_useParallel, _parallelOptions, _scorer));
    }
}

/// <summary>
/// Fluent builder for configuring cached fuzzy string matching pipelines with automatic scorer creation.
/// Produces a <see cref="ProcessPipeline"/> with caching enabled — a
/// <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.CachedWeightedRatioScorer"/>
/// is created automatically per extraction call.
/// </summary>
public sealed class CachedProcessBuilder
{
    private bool _useParallel;
    private ParallelOptions _parallelOptions;

    internal CachedProcessBuilder(bool useParallel, ParallelOptions parallelOptions)
    {
        _useParallel = useParallel;
        _parallelOptions = parallelOptions;
    }

    /// <summary>
    /// Enables parallel execution for improved performance on multi-core systems.
    /// </summary>
    /// <param name="parallelOptions">Optional parallel execution options (max degree of parallelism, cancellation token, etc.)</param>
    public CachedProcessBuilder Parallel(ParallelOptions parallelOptions = null)
    {
        _useParallel = true;
        if (parallelOptions != null)
        {
            _parallelOptions = parallelOptions;
        }
        return this;
    }

    /// <summary>
    /// Configures parallel execution options (max degree of parallelism, cancellation token, etc.).
    /// Implicitly enables parallel mode.
    /// </summary>
    /// <param name="parallelOptions">Parallel execution options</param>
    public CachedProcessBuilder WithParallelOptions(ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions ?? throw new ArgumentNullException(nameof(parallelOptions));
        _useParallel = true;
        return this;
    }

    /// <summary>
    /// Builds an immutable ProcessPipeline with caching enabled.
    /// </summary>
    public ProcessPipeline Build()
    {
        return new ProcessPipeline(new ProcessOptions(_useParallel, _parallelOptions, scorer: null, useCaching: true));
    }
}

/// <summary>
/// Fluent builder for configuring cached fuzzy string matching pipelines with an external scorer.
/// Produces a <see cref="CachedScorerProcessPipeline"/> that reuses the provided
/// <see cref="ICachedRatioScorer"/> across all extraction calls.
/// </summary>
public sealed class CachedScorerProcessBuilder
{
    private bool _useParallel;
    private ParallelOptions _parallelOptions;
    private readonly ICachedRatioScorer _cachedScorer;

    internal CachedScorerProcessBuilder(ICachedRatioScorer cachedScorer, bool useParallel, ParallelOptions parallelOptions)
    {
        _cachedScorer = cachedScorer;
        _useParallel = useParallel;
        _parallelOptions = parallelOptions;
    }

    /// <summary>
    /// Enables parallel execution for improved performance on multi-core systems.
    /// </summary>
    /// <param name="parallelOptions">Optional parallel execution options (max degree of parallelism, cancellation token, etc.)</param>
    public CachedScorerProcessBuilder Parallel(ParallelOptions parallelOptions = null)
    {
        _useParallel = true;
        if (parallelOptions != null)
        {
            _parallelOptions = parallelOptions;
        }
        return this;
    }

    /// <summary>
    /// Configures parallel execution options (max degree of parallelism, cancellation token, etc.).
    /// Implicitly enables parallel mode.
    /// </summary>
    /// <param name="parallelOptions">Parallel execution options</param>
    public CachedScorerProcessBuilder WithParallelOptions(ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions ?? throw new ArgumentNullException(nameof(parallelOptions));
        _useParallel = true;
        return this;
    }

    /// <summary>
    /// Builds an immutable CachedScorerProcessPipeline with the configured options.
    /// </summary>
    public CachedScorerProcessPipeline Build()
    {
        return new CachedScorerProcessPipeline(new CachedScorerProcessOptions(
            _useParallel,
            _parallelOptions,
            _cachedScorer
        ));
    }
}
```

FuzzySharp/ProcessExecutor.cs
```
using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Internal execution logic for Process operations.
/// When <see cref="ProcessOptions.UseCaching"/> is true, creates a
/// <see cref="CachedWeightedRatioScorer"/> per extraction call and delegates
/// to <see cref="CachedScorerProcessExecutor"/>.
/// Otherwise dispatches to sequential or parallel ResultExtractor paths.
/// </summary>
internal static class ProcessExecutor
{
    #region ExtractAll

    public static IEnumerable<ExtractedResult<string>> ExtractAll(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            return ExtractAllCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractAllCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractAllCached(query, choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractAllCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractAll(
            choices, processor, scorer, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    #endregion

    #region ExtractTop

    public static IEnumerable<ExtractedResult<string>> ExtractTop(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            return ExtractTopCached(processor(query), choices, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                query, choices, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractTopCached(processor(query), choices, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                query, choices, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractTopCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractTop(
            choices, processor, scorer, limit, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    #endregion

    #region ExtractSorted

    public static IEnumerable<ExtractedResult<string>> ExtractSorted(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            return ExtractSortedCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractSortedCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractSortedCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractSorted(
            choices, processor, scorer, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    #endregion

    #region ExtractOne

    public static ExtractedResult<string> ExtractOne(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            var processedQuery = processor(query);
            using var cachedScorer = new CachedWeightedRatioScorer(processedQuery);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            var processedQuery = processor(query);
            using var cachedScorer = new CachedWeightedRatioScorer(processedQuery);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    #endregion
}
```

FuzzySharp/ProcessOptions.cs
```
using System.Threading.Tasks;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Internal options for process execution.
/// When <see cref="UseCaching"/> is true, the executor creates a
/// <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.CachedWeightedRatioScorer"/>
/// per extraction call automatically.
/// </summary>
internal readonly struct ProcessOptions
{
    public ProcessOptions(bool useParallel, ParallelOptions parallelOptions, IRatioScorer scorer, bool useCaching = false)
    {
        UseParallel = useParallel;
        ParallelOptions = parallelOptions;
        Scorer = scorer;
        UseCaching = useCaching;
    }

    public bool UseParallel { get; }
    public ParallelOptions ParallelOptions { get; }
    public IRatioScorer Scorer { get; }
    public bool UseCaching { get; }
}

/// <summary>
/// Internal options for cached process execution with an external scorer.
/// </summary>
internal readonly struct CachedScorerProcessOptions
{
    public CachedScorerProcessOptions(bool useParallel, ParallelOptions parallelOptions, ICachedRatioScorer cachedScorer)
    {
        UseParallel = useParallel;
        ParallelOptions = parallelOptions;
        CachedScorer = cachedScorer;
    }

    public bool UseParallel { get; }
    public ParallelOptions ParallelOptions { get; }
    public ICachedRatioScorer CachedScorer { get; }
}
```

FuzzySharp/ProcessPipeline.cs
```
using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Immutable fuzzy string matching pipeline.
/// The scoring algorithm is configured at build time via <see cref="ProcessBuilder.WithScorer"/>.
/// When <see cref="ProcessOptions.UseCaching"/> is true, extraction methods automatically create
/// a cached scorer per call for improved performance.
/// </summary>
public readonly struct ProcessPipeline
{
    private readonly ProcessOptions _options;

    internal ProcessPipeline(ProcessOptions options)
    {
        _options = options;
    }

    #region ExtractAll

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractAll(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractAll(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractAll(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractAll(query, choices, processor, cutoff, _options);
    }

    #endregion

    #region ExtractTop

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractTop(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractTop(query, choices, processor, limit, cutoff, _options);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int limit = 5,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractTop(query, choices, processor, limit, cutoff, _options);
    }

    #endregion

    #region ExtractSorted

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractSorted(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractSorted(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractSorted(query, choices, processor, cutoff, _options);
    }

    #endregion

    #region ExtractOne

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractOne(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<T> ExtractOne<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractOne(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(string query, params string[] choices)
    {
        return ProcessExecutor.ExtractOne(query, choices, null, 0, _options);
    }

    #endregion
}

/// <summary>
/// Immutable cached fuzzy string matching pipeline with an external <see cref="ICachedRatioScorer"/>.
/// The scorer is provided at build time and reused across all extraction calls.
/// The caller owns the scorer's lifecycle (creation and disposal).
/// </summary>
public readonly struct CachedScorerProcessPipeline
{
    private readonly CachedScorerProcessOptions _options;

    internal CachedScorerProcessPipeline(CachedScorerProcessOptions options)
    {
        _options = options;
    }

    #region ExtractAll

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractAll(
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractAll(
            choices, processor ?? Process.DefaultStringProcessor, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));
        return CachedScorerProcessExecutor.ExtractAll(
            choices, processor, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion

    #region ExtractTop

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractTop(
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractTop(
            choices, processor ?? Process.DefaultStringProcessor, _options.CachedScorer, limit, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        int limit = 5,
        int cutoff = 0)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));
        return CachedScorerProcessExecutor.ExtractTop(
            choices, processor, _options.CachedScorer, limit, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion

    #region ExtractSorted

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractSorted(
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractSorted(
            choices, processor ?? Process.DefaultStringProcessor, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));
        return CachedScorerProcessExecutor.ExtractSorted(
            choices, processor, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion

    #region ExtractOne

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractOne(
            choices, processor ?? Process.DefaultStringProcessor, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<T> ExtractOne<T>(
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff = 0)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));
        return CachedScorerProcessExecutor.ExtractOne(
            choices, processor, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(params string[] choices)
    {
        return CachedScorerProcessExecutor.ExtractOne(
            choices, Process.DefaultStringProcessor, _options.CachedScorer, 0,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion
}
```

FuzzySharp/Edits/EditOp.cs
```
﻿namespace Raffinert.FuzzySharp.Edits;

public enum EditType
{
    DELETE = 0,
    EQUAL = 1,
    INSERT = 2,
    REPLACE = 3,
    KEEP = 4,
}

public record EditOp
{
    public EditType EditType { get; set; }
    public int SourcePos { get; set; }
    public int DestPos { get; set; }

    public override string ToString()
    {
        return $"{EditType}({SourcePos}, {DestPos})";
    }
}
```

FuzzySharp/Edits/MatchingBlock.cs
```
﻿namespace Raffinert.FuzzySharp.Edits;

public record MatchingBlock
{
    public int SourcePos { get; set; }
    public int DestPos { get; set; }
    public int Length { get; set; }

    public override string ToString() => $"({SourcePos},{DestPos},{Length})";
}
```

FuzzySharp/Edits/OpCode.cs
```
﻿namespace Raffinert.FuzzySharp.Edits;

public class OpCode
{
    public EditType EditType    { get; set; }
    public int      SourceBegin { get; set; }
    public int      SourceEnd   { get; set; }
    public int      DestBegin   { get; set; }
    public int      DestEnd     { get; set; }

    public override string ToString()
    {
        return $"{EditType}({SourceBegin},{SourceEnd},{DestBegin},{DestEnd})";
    }
}
```

FuzzySharp/Extensions/EditOpsExtensions.cs
```
﻿using Raffinert.FuzzySharp.Edits;
using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Extensions;

public static class EditOpsExtensions
{
    public static List<OpCode> AsOpCodes(this IEnumerable<EditOp> editOps, int srcLen, int destLen)
    {
        var opcodes = new List<OpCode>();
        int prevI = 0, prevJ = 0;
        foreach (var op in editOps)
        {
            int i = op.SourcePos;
            int j = op.DestPos;
            // equal segment
            if (prevI < i && prevJ < j)
                opcodes.Add(new OpCode { EditType = EditType.KEEP, SourceBegin = prevI, SourceEnd = i, DestBegin = prevJ, DestEnd = j });
            // delete
            if (op.EditType == EditType.DELETE)
            {
                opcodes.Add(new OpCode { EditType = EditType.DELETE, SourceBegin = i, SourceEnd = i + 1, DestBegin = j, DestEnd = j });
                prevI = i + 1;
                prevJ = j;
            }
            // insert
            else if (op.EditType == EditType.INSERT)
            {
                opcodes.Add(new OpCode { EditType = EditType.INSERT, SourceBegin = i, SourceEnd = i, DestBegin = j, DestEnd = j + 1 });
                prevI = i;
                prevJ = j + 1;
            }
        }

        // final equal segment
        if (prevI < srcLen && prevJ < destLen)
        {
            opcodes.Add(new OpCode
            {
                EditType = EditType.KEEP,
                SourceBegin = prevI,
                SourceEnd = srcLen,
                DestBegin = prevJ,
                DestEnd = destLen
            });
        }

        return opcodes;
    }

    public static List<MatchingBlock> AsMatchingBlocks(
        this IEnumerable<EditOp> ops,
        int srcLen,
        int destLen)
    {
        var blocks = new List<MatchingBlock>();
        int srcPos = 0;
        int destPos = 0;

        foreach (var op in ops)
        {
            // emit any "skipped" matching region before this op
            if (srcPos < op.SourcePos || destPos < op.DestPos)
            {
                int length = Math.Min(op.SourcePos - srcPos,
                    op.DestPos - destPos);
                if (length > 0)
                {
                    blocks.Add(new MatchingBlock
                    {
                        SourcePos = srcPos,
                        DestPos = destPos,
                        Length = length
                    });
                }

                srcPos = op.SourcePos;
                destPos = op.DestPos;
            }

            // consume the op
            switch (op.EditType)
            {
                case EditType.REPLACE:
                    srcPos++;
                    destPos++;
                    break;
                case EditType.DELETE:
                    srcPos++;
                    break;
                case EditType.INSERT:
                    destPos++;
                    break;
            }
        }

        // any trailing match after the last op
        if (srcPos < srcLen || destPos < destLen)
        {
            int length = Math.Min(srcLen - srcPos,
                destLen - destPos);
            if (length > 0)
            {
                blocks.Add(new MatchingBlock
                {
                    SourcePos = srcPos,
                    DestPos = destPos,
                    Length = length
                });
            }
        }

        // sentinel: zero-length block at the very end
        blocks.Add(new MatchingBlock
        {
            SourcePos = srcLen,
            DestPos = destLen,
            Length = 0
        });

        return blocks;
    }
}
```

FuzzySharp/Extensions/EnumerableExtensions.cs
```
﻿using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Utils;

namespace Raffinert.FuzzySharp.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> MaxN<T>(this IEnumerable<T> source, int n) where T : IComparable<T>
    {
        var comparer = Comparer<T>.Default;
        var queue = new MinHeap<T>(comparer);
        
        foreach (var item in source)
        {
            if (queue.Count < n)
            {
                queue.Add(item);
            }
            else if (comparer.Compare(item, queue.GetMin()) > 0)
            {
                queue.ExtractDominating();
                queue.Add(item);
            }
        }

        for (int i = 0; i < n && queue.Count > 0; i++)
        {
            yield return queue.ExtractDominating();
        }
    }

    public static IEnumerable<T> MaxNBy<T, TVal>(this IEnumerable<T> source, int n, Func<T, TVal> selector) where TVal : IComparable<TVal>
    {
        var valComparer = Comparer<TVal>.Default;
        var queue = new MinHeap<T>(Comparer<T>.Create((x, y) => valComparer.Compare(selector(x), selector(y))));
        
        foreach (var item in source)
        {
            if (queue.Count < n)
            {
                queue.Add(item);
            }
            else if (valComparer.Compare(selector(item), selector(queue.GetMin())) > 0)
            {
                queue.ExtractDominating();
                queue.Add(item);
            }
        }

        for (int i = 0; i < n && queue.Count > 0; i++)
        {
            yield return queue.ExtractDominating();
        }
    }
}
```

FuzzySharp/Extensions/StringExtensions.cs
```
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Raffinert.FuzzySharp.Extensions;

internal static class StringExtensions
{
    public static List<string> ExtractTokens(this string input)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(input))
            return result;

        var span = input.AsSpan();

        var start = 0;
        for (var i = 0; i < span.Length; i++)
        {
            if (char.IsLetter(span[i])) continue;

            if (i - start > 0)
            {
                result.Add(span[start..i].ToString());
            }

            start = i+1;
        }

        if (span.Length - start > 0)
            result.Add(span[start..].ToString());

        return result;
    }

    public static string GetInitials(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var span = input.AsSpan();

        var sb = new StringBuilder(span.Length);

        var takeNext = true;

        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];

            if (char.IsWhiteSpace(c))
            {
                takeNext = true;
            }
            else if (takeNext)
            {
                sb.Append(c);
                takeNext = false;
            }
        }

        return sb.ToString();
    }

    public static string[] SplitByAnySpace(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return [];

        var words = input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

        return words;
    }

    public static string[] GetSortedWords(this string input)
    {
        var words = SplitByAnySpace(input);

        Array.Sort(words);

        return words;
    }

    public static string NormalizeSpacesAndSort(this string input)
    {
        var words = GetSortedWords(input);

        return string.Join(" ", words);
    }
}
```

FuzzySharp/Extractor/ExtractedResult.cs
```
﻿using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Extractor;

public class ExtractedResult<T>(T value, int score, int index) : IComparable<ExtractedResult<T>>
{
    public readonly T Value = value;
    public readonly int Score = score;
    public readonly int Index = index;

    public ExtractedResult(T value, int score) : this(value, score, 0)
    { }

    public int CompareTo(ExtractedResult<T> other)
    {
        return Comparer<int>.Default.Compare(this.Score, other.Score);
    }

    public override string ToString()
    {
        if (typeof(T) == typeof(string))
        {
            return $"(string: {Value}, score: {Score}, index: {Index})";
        }
        return $"(value: {Value}, score: {Score}, index: {Index})";
    }
}
```

FuzzySharp/Extractor/ResultExtractor.Cached.cs
```
﻿using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    public static class Cached
    {
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            int index = 0;
            foreach (var choice in choices)
            {
                int score = scorer.Score(processor(choice));
                if (score >= cutoff)
                {
                    yield return new ExtractedResult<T>(choice, score, index);
                }
                index++;
            }
        }

        public static ExtractedResult<T> ExtractOne<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, processor, calculator, cutoff).Max();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, processor, calculator, cutoff).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int limit, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, processor, calculator, cutoff).MaxN(limit).Reverse();
        }
    }
}
```

FuzzySharp/Extractor/ResultExtractor.cs
```
﻿using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(string query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        int index = 0;
        foreach (var choice in choices)
        {
            int score = scorer.Score(query, processor(choice));
            if (score >= cutoff)
            {
                yield return new ExtractedResult<T>(choice, score, index);
            }
            index++;
        }
    }

    public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        var processedQuery = processor(query);
        return ExtractWithoutOrder(processedQuery, choices, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, processor, calculator, cutoff).Max();
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, processor, calculator, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int limit, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, processor, calculator, cutoff).MaxN(limit).Reverse();
    }
}
```

FuzzySharp/Extractor/ResultExtractor.Parallel.Cached.cs
```
﻿using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    public static partial class Parallel
    {
        public static class Cached
        {
            public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                var result = new ExtractedResult<T>[materializedChoices.Count];

                System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
                {
                    int score = scorer.Score(processor(choice));
                    if (score >= cutoff)
                    {
                        result[index] = new ExtractedResult<T>(choice, score, (int)index);
                    }
                });
                return result.Where(r => r != null);
            }

            public static ExtractedResult<T> ExtractOne<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).Max();
            }

            public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
            }

            public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
            }
        }
    }
}
```

FuzzySharp/Extractor/ResultExtractor.Parallel.cs
```
﻿using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    private static readonly ParallelOptions DefaultParallelOptions = new ParallelOptions();

    public static partial class Parallel
    {
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(string query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var result = new ExtractedResult<T>[materializedChoices.Count];

            System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
            {
                int score = scorer.Score(query, processor(choice));
                if (score >= cutoff)
                {
                    result[index] = new ExtractedResult<T>(choice, score, (int)index);
                }
            });
            return result.Where(r => r != null);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var processedQuery = processor(query);
            return ExtractWithoutOrder(processedQuery, choices, processor, scorer, cutoff);
        }

        public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).Max();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
        }
    }
}
```

FuzzySharp/PreProcess/PreprocessMode.cs
```
﻿namespace Raffinert.FuzzySharp.PreProcess;

public enum PreprocessMode
{
    Full = 0,
    None = 1,
}
```

FuzzySharp/PreProcess/StringPreprocessorFactory.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.PreProcess;

internal static class StringPreprocessorFactory
{
    private static string Default(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var result = new char[input.Length].AsSpan();

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            result[i] = char.IsLetterOrDigit(c) ? char.ToLower(c) : ' ';
        }

        return ((ReadOnlySpan<char>)result).Trim().ToString();
    }

    public static Func<string, string> GetPreprocessor(PreprocessMode mode)
    {
        return mode switch
        {
            PreprocessMode.Full => Default,
            PreprocessMode.None => static s => s,
            _ => throw new InvalidOperationException($"Invalid string preprocessor mode: {mode}")
        };
    }
}
```

FuzzySharp/SimilarityRatio/ScorerCache.cs
```
﻿using System.Runtime.CompilerServices;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp.SimilarityRatio;

public static class ScorerCache
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IRatioScorer Get<T>() where T : IRatioScorer, new() => GenericCache<T>.Instance;

    private static class GenericCache<T>
        where T : IRatioScorer, new()
    {
        public static readonly T Instance = new T();
    }
}
```

FuzzySharp/Utils/DictionarySlimPooled.cs
```
﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

// Adapted to use pooled arrays from the .NET Collections Extensions project
/// <summary>
/// A lightweight Dictionary with three principal differences compared to <see cref="Dictionary{TKey, TValue}"/>,
/// rewritten to use pooled arrays to avoid heap allocations after initial creation.
///
/// 1) It is possible to do "get or add" in a single lookup using <see cref="GetOrAddValueRef(TKey)"/>. For
///    values that are value types, this also saves a copy of the value.
/// 2) It assumes it is cheap to equate values.
/// 3) It assumes the keys implement <see cref="IEquatable{TKey}"/> or else Equals() and they are cheap and sufficient.
/// 4) Buckets and entries arrays are rented from <see cref="ArrayPool{T}"/>, and returned when cleared or resized,
///    minimizing allocations.
/// </summary>
[DebuggerTypeProxy(typeof(DictionarySlimPooledDebugView<,>))]
[DebuggerDisplay("Count = {Count}")]
internal sealed class DictionarySlimPooled<TKey, TValue> : IDisposable, IReadOnlyCollection<KeyValuePair<TKey, TValue>> where TKey : IEquatable<TKey>
{
    private static readonly Entry[] InitialEntries = new Entry[1];
    private static readonly int[] InitialBuckets = HashHelpers.SizeOneIntArray;

    private int _count;
    private int _freeList = -1; // 0-based index into _entries of head of free chain; -1 means empty
    private int[] _buckets;
    private Entry[] _entries;

    private readonly ArrayPool<int> _bucketsPool;
    private readonly ArrayPool<Entry> _entriesPool;
    private int _size;

    [DebuggerDisplay("({key}, {value})->{next}")]
    private struct Entry
    {
        public TKey key;
        public TValue value;
        // 0-based index of next entry in chain: -1 means end of chain.
        // If negative less than -1, it encodes that this entry is on the free list:
        //    next = -3 - previousFreeIndex. E.g. -3 means index 0 is free, -4 means index 1 is free, etc.
        public int next;
    }

    /// <summary>
    /// Construct with default capacity.
    /// </summary>
    public DictionarySlimPooled()
    {
        _bucketsPool = ArrayPool<int>.Shared;
        _entriesPool = ArrayPool<Entry>.Shared;
        _buckets = InitialBuckets;
        _entries = InitialEntries;
    }

    /// <summary>
    /// Construct with at least the specified capacity for
    /// entries before resizing must occur.
    /// </summary>
    /// <param name="capacity">Requested minimum capacity</param>
    public DictionarySlimPooled(int capacity)
    {
        if (capacity < 0)
            ThrowHelper.ThrowCapacityArgumentOutOfRangeException();

        if (capacity < 2)
            capacity = 2; // 1 would indicate the dummy array

        _size = HashHelpers.PowerOf2(capacity);

        _bucketsPool = ArrayPool<int>.Shared;
        _entriesPool = ArrayPool<Entry>.Shared;

        _buckets = _bucketsPool.Rent(_size);
        _entries = _entriesPool.Rent(_size);

        Array.Clear(_buckets, 0, _size);
        Array.Clear(_entries, 0, _size);
    }

    /// <summary>
    /// Count of entries in the dictionary.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Clears the dictionary. Note that this invalidates any active enumerators.
    /// Returns rented arrays to their pools.
    /// </summary>
    public void Clear()
    {
        if (_entries != InitialEntries)
        {
            // Return arrays to the pool before resetting to static empty
            _entriesPool.Return(_entries);
            _bucketsPool.Return(_buckets);

            _entries = InitialEntries;
            _buckets = InitialBuckets;
        }

        _count = 0;
        _freeList = -1;
    }

    /// <summary>
    /// Looks for the specified key in the dictionary.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>true if the key is present, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int collisionCount = 0;
        for (int i = buckets[hash] - 1; (uint)i < (uint)_size; i = entries[i].next)
        {
            if (key.Equals(entries[i].key))
                return true;

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        return false;
    }

    /// <summary>
    /// Gets the value if present for the specified key.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <param name="value">Value found, otherwise default(TValue)</param>
    /// <returns>true if the key is present, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int collisionCount = 0;
        for (int i = buckets[hash] - 1; (uint)i < (uint)_size; i = entries[i].next)
        {
            if (key.Equals(entries[i].key))
            {
                value = entries[i].value;
                return true;
            }

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Removes the entry if present with the specified key.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>true if the key is present, false if it is not</returns>
    public bool Remove(TKey key)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int last = -1;
        int i = buckets[hash] - 1;
        int collisionCount = 0;

        while (i != -1)
        {
            ref Entry candidate = ref entries[i];
            if (candidate.key.Equals(key))
            {
                if (last != -1)
                {
                    entries[last].next = candidate.next;
                }
                else
                {
                    buckets[hash] = candidate.next + 1;
                }

                // Clear entry and add to free list
                candidate = default;
                candidate.next = -3 - _freeList;
                _freeList = i;
                _count--;
                return true;
            }

            last = i;
            i = candidate.next;

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        return false;
    }

    /// <summary>
    /// Gets the value for the specified key, or, if the key is not present,
    /// adds an entry and returns the value by ref. This makes it possible to
    /// add or update a value in a single look up operation.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>Reference to the new or existing value</returns>
    public ref TValue GetOrAddValueRef(TKey key)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int collisionCount = 0;
        for (int i = buckets[hash] - 1; (uint)i < (uint)_size; i = entries[i].next)
        {
            if (key.Equals(entries[i].key))
                return ref entries[i].value;

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        return ref AddKey(key, hash);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private ref TValue AddKey(TKey key, int bucketIndex)
    {
        var entries = _entries;
        int entryIndex;

        if (_freeList != -1)
        {
            entryIndex = _freeList;
            _freeList = -3 - entries[_freeList].next;
        }
        else
        {
            if (_entries == InitialEntries || _count == _size)
            {
                entries = Resize();
                bucketIndex = key.GetHashCode() & (_size - 1);
                // entry indexes were not changed by Resize
            }

            entryIndex = _count;
        }

        entries[entryIndex].key = key;
        entries[entryIndex].next = _buckets[bucketIndex] - 1;
        _buckets[bucketIndex] = entryIndex + 1;
        _count++;
        return ref entries[entryIndex].value;
    }

    private Entry[] Resize()
    {
        int oldSize = _entries == InitialEntries ? 0 : _size;
        int newSize = oldSize == 0 ? 2 : oldSize * 2;
        if (unchecked((uint)newSize > (uint)int.MaxValue)) // uint cast handles overflow
        {
            throw new InvalidOperationException("DictionarySlimPooled: Capacity overflow.");
        }

        // Rent new arrays
        var newBuckets = _bucketsPool.Rent(newSize);
        var newEntries = _entriesPool.Rent(newSize);

        int count = _count;
        Array.Clear(newBuckets, 0, newSize);
        Array.Clear(newEntries, count, newSize - count);

        Array.Copy(_entries, 0, newEntries, 0, count);

        // Recompute buckets
        while (count-- > 0)
        {
            int bucketIndex = newEntries[count].key.GetHashCode() & (newSize - 1);
            newEntries[count].next = newBuckets[bucketIndex] - 1;
            newBuckets[bucketIndex] = count + 1;
        }

        // Return old arrays to pool
        _entriesPool.Return(_entries);
        _bucketsPool.Return(_buckets);

        _buckets = newBuckets;
        _entries = newEntries;

        _size = newSize;

        return _entries;
    }

    /// <summary>
    /// Gets an enumerator over the dictionary
    /// </summary>
    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    /// <summary>
    /// Enumerator
    /// </summary>
    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly DictionarySlimPooled<TKey, TValue> _dictionary;
        private int _index;
        private int _remaining; // remaining count to enumerate

        internal Enumerator(DictionarySlimPooled<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _index = 0;
            _remaining = dictionary._count;
            Current = default;
        }

        /// <summary>
        /// Move to next
        /// </summary>
        public bool MoveNext()
        {
            if (_remaining == 0)
            {
                Current = default;
                return false;
            }

            var entries = _dictionary._entries;
            while (_index < _dictionary._size && entries[_index].next < -1)
            {
                _index++;
            }

            if (_index >= _dictionary._size)
            {
                Current = default;
                return false;
            }

            Current = new KeyValuePair<TKey, TValue>(
                entries[_index].key,
                entries[_index].value);

            _index++;
            _remaining--;
            return true;
        }

        public KeyValuePair<TKey, TValue> Current { get; private set; }

        object IEnumerator.Current => Current;
        public void Reset()
        {
            _index = 0;
            _remaining = _dictionary._count;
            Current = default;
        }
        public void Dispose() { }
    }

    public void Dispose()
    {
        Clear();
    }
}

internal sealed class DictionarySlimPooledDebugView<K, V>(DictionarySlimPooled<K, V> dictionary)
    where K : IEquatable<K>
{
    private readonly DictionarySlimPooled<K, V> _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<K, V>[] Items => _dictionary.ToArray();
}
```

FuzzySharp/Utils/HashHelpers.cs
```
namespace Raffinert.FuzzySharp.Utils;

internal static class HashHelpers
{
    internal static int PowerOf2(int v)
    {
        if ((v & (v - 1)) == 0) return v;
        int i = 2;
        while (i < v) i <<= 1;
        return i;
    }

    // must never be written to
    internal static readonly int[] SizeOneIntArray = new int[1];
}
```

FuzzySharp/Utils/Heap.cs
```
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Utils;

public abstract class Heap<T> : IEnumerable<T>
{
    private const int InitialCapacity = 0;
    private const int GrowFactor      = 2;
    private const int MinGrow         = 1;

    private T[] _heap     = new T[InitialCapacity];

    public int Count { get; private set; }

    public int Capacity { get; private set; } = InitialCapacity;

    protected          Comparer<T> Comparer { get; }
    protected abstract bool        Dominates(T x, T y);

    protected Heap() : this(Comparer<T>.Default)
    {
    }

    protected Heap(Comparer<T> comparer) : this([], comparer)
    {
    }

    protected Heap(IEnumerable<T> collection)
        : this(collection, Comparer<T>.Default)
    {
    }

    protected Heap(IEnumerable<T> collection, Comparer<T> comparer)
    {
        Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        _ = collection ?? throw new ArgumentNullException(nameof(collection));

        foreach (var item in collection)
        {
            if (Count == Capacity)
                Grow();

            _heap[Count++] = item;
        }

        for (int i = Parent(Count - 1); i >= 0; i--)
            BubbleDown(i);
    }

    public void Add(T item)
    {
        if (Count == Capacity)
            Grow();

        _heap[Count++] = item;
        BubbleUp(Count - 1);
    }

    private void BubbleUp(int i)
    {
        while (true)
        {
            if (i == 0 || Dominates(_heap[Parent(i)], _heap[i])) return; //correct domination (or root)

            Swap(i, Parent(i));
            i = Parent(i);
        }
    }

    public T GetMin()
    {
        if (Count == 0) throw new InvalidOperationException("Heap is empty");
        return _heap[0];
    }

    public T ExtractDominating()
    {
        if (Count == 0) throw new InvalidOperationException("Heap is empty");
        T ret = _heap[0];
        Count--;
        Swap(Count, 0);
        BubbleDown(0);
        return ret;
    }

    private void BubbleDown(int i)
    {
        while (true)
        {
            var dominatingNode = Dominating(i);
            if (dominatingNode == i) return;
            Swap(i, dominatingNode);
            i = dominatingNode;
        }
    }

    private int Dominating(int i)
    {
        int dominatingNode = i;
        dominatingNode = GetDominating(YoungChild(i), dominatingNode);
        dominatingNode = GetDominating(OldChild(i),   dominatingNode);

        return dominatingNode;
    }

    private int GetDominating(int newNode, int dominatingNode)
    {
        if (newNode < Count && !Dominates(_heap[dominatingNode], _heap[newNode]))
            return newNode;

        return dominatingNode;
    }

    private void Swap(int i, int j)
    {
        (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
    }

    private static int Parent(int i)
    {
        return (i + 1) / 2 - 1;
    }

    private static int YoungChild(int i)
    {
        return (i + 1) * 2 - 1;
    }

    private static int OldChild(int i)
    {
        return YoungChild(i) + 1;
    }

    private void Grow()
    {
        int newCapacity = Capacity * GrowFactor + MinGrow;
        var newHeap     = new T[newCapacity];
        Array.Copy(_heap, newHeap, Capacity);
        _heap     = newHeap;
        Capacity = newCapacity;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _heap.Take(Count).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class MinHeap<T> : Heap<T>
{
    public MinHeap()
        : this(Comparer<T>.Default)
    {
    }

    public MinHeap(Comparer<T> comparer)
        : base(comparer)
    {
    }

    public MinHeap(IEnumerable<T> collection) : base(collection)
    {
    }

    public MinHeap(IEnumerable<T> collection, Comparer<T> comparer)
        : base(collection, comparer)
    {
    }

    protected override bool Dominates(T x, T y)
    {
        return Comparer.Compare(x, y) <= 0;
    }
}
```

FuzzySharp/Utils/PatternMatchVector.cs
```
﻿using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

public interface IPatternMatchVector<in TKey> : IDisposable where TKey : notnull, IEquatable<TKey>
{
    int Blocks { get; }
    ReadOnlySpan<ulong> GetOrZero(TKey key);

    bool ContainsKey(TKey key);
}

internal interface IPatternMatchVectorImpl<in TKey> : IPatternMatchVector<TKey> where TKey : IEquatable<TKey>
{
    void AddBit(TKey key, int position);
}

public sealed class PatternMatchVector
{
    public static IPatternMatchVector<T> Create<T>(ReadOnlySpan<T> source) where T : notnull, IEquatable<T>
    {
        var blocks = (source.Length + 63) >> 6;

        var pmv = typeof(T) == typeof(char)
            ? (IPatternMatchVectorImpl<T>)(object)new PatternMatchVectorChar(estimatedNonAsciiCharCount: 8, blocks: blocks)
            : new PatternMatchVector<T>(64, blocks);

        var i = 0;

        foreach (var item in source)
        {
            pmv.AddBit(item, i++);
        }

        return pmv;
    }
}

internal sealed class PatternMatchVector<T> : IPatternMatchVectorImpl<T> where T : notnull, IEquatable<T>
{
    private readonly ArrayPool<ulong> _pool;
    private readonly DictionarySlimPooled<T, int> _indexMap;
    private ulong[] _buffer;
    private int _capacity;
    private int _next;
    private readonly ulong[] _zeroMask;
    private bool _disposed;

    public PatternMatchVector(int estimatedCharCount, int blocks, ArrayPool<ulong> pool = null)
    {
        _pool = pool ?? ArrayPool<ulong>.Shared;
        Blocks = blocks;
        _capacity = estimatedCharCount;
        _buffer = _pool.Rent(_capacity * Blocks);
        _zeroMask = _pool.Rent(Blocks);
        Array.Clear(_zeroMask, 0, Blocks);
        _indexMap = new DictionarySlimPooled<T, int>(estimatedCharCount);
        _next = 0;
    }

    public int Blocks { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddBit(T key, int position)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVector<T>));

        ref var index = ref _indexMap.GetOrAddValueRef(key);

        if (index == 0)
        {
            if (_next >= _capacity)
            {
                GrowBuffer();
            }

            index = ++_next;

            Array.Clear(_buffer, (index - 1) * Blocks, Blocks);
        }

        int block = position >> 6;
        int offset = position & 63;

        _buffer[(index - 1) * Blocks + block] |= 1UL << offset;
    }

    private void GrowBuffer()
    {
        int newCapacity = _capacity * 2;
        ulong[] newBuffer = _pool.Rent(newCapacity * Blocks);

        // Copy existing masks
        Array.Copy(_buffer, 0, newBuffer, 0, _capacity * Blocks);

        // Return old buffer
        _pool.Return(_buffer);

        _buffer = newBuffer;
        _capacity = newCapacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(T key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVector<T>));

        return _indexMap.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetMask(T key, out ReadOnlySpan<ulong> mask)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVector<T>));

        if (_indexMap.TryGetValue(key, out var index))
        {
            mask = new ReadOnlySpan<ulong>(_buffer, (index - 1) * Blocks, Blocks);
            return true;
        }
        mask = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrZero(T key)
    {
        return TryGetMask(key, out var mask) ? mask : _zeroMask;
    }

    public ReadOnlySpan<ulong> GetOrDefault(T key, ReadOnlySpan<ulong> fallback)
    {
        return TryGetMask(key, out var mask) ? mask : fallback;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _indexMap.Dispose();
        _pool.Return(_buffer);
        _pool.Return(_zeroMask);

        _disposed = true;
    }
}
```

FuzzySharp/Utils/PatternMatchVectorChar.cs
```
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

/// <summary>
/// PatternMatchVector specialized for char with RapidFuzz-style fast path:
/// - ASCII/extended-ASCII chars (0..255) stored in a dense array (no hashing)
/// - Non-ASCII chars stored in a pooled dictionary + pooled dense buffer
///
/// Each char maps to a mask of length <see cref="Blocks"/> ulongs.
/// Bit position indicates where that char occurs in the pattern.
/// </summary>
internal sealed class PatternMatchVectorChar : IPatternMatchVectorImpl<char>
{
    private readonly ArrayPool<ulong> _pool;
    private readonly DictionarySlimPooled<char, int> _indexMap; // non-ASCII only (1-based)

    private readonly ulong[] _fixedData; // Single rental: [asciiMasks (256*blocks) | asciiPresence (4) | zeroMask (blocks)]
    private readonly int _asciiMasksOffset;
    private readonly int _asciiPresenceOffset;
    private readonly int _zeroMaskOffset;

    private ulong[] _buffer;     // capacity * blocks for non-ASCII (separate rental, can grow)

    private readonly int _blocks;
    private int _capacity;
    private int _next;

    private bool _disposed;

    public int Blocks => _blocks;

    public PatternMatchVectorChar(int estimatedNonAsciiCharCount, int blocks, ArrayPool<ulong>? pool = null)
    {
        if (blocks < 0) throw new ArgumentOutOfRangeException(nameof(blocks));
        if (estimatedNonAsciiCharCount < 0) throw new ArgumentOutOfRangeException(nameof(estimatedNonAsciiCharCount));

        _pool = pool ?? ArrayPool<ulong>.Shared;
        _blocks = blocks;

        // Single rental for all fixed-size data:
        // Layout: [asciiMasks (256*blocks) | asciiPresence (4) | zeroMask (blocks)]
        int totalFixedSize = (256 * _blocks) + 4 + _blocks;
        _fixedData = _pool.Rent(totalFixedSize);
        
        _asciiMasksOffset = 0;
        _asciiPresenceOffset = 256 * _blocks;
        _zeroMaskOffset = _asciiPresenceOffset + 4;

        // Clear all fixed data
        Array.Clear(_fixedData, 0, totalFixedSize);

        // Non-ASCII buffer (separate rental, can grow)
        _capacity = Math.Max(2, estimatedNonAsciiCharCount);
        _buffer = _pool.Rent(_capacity * _blocks);
        // Intentionally not clearing entire _buffer. Each new key slice is cleared once.

        _indexMap = new DictionarySlimPooled<char, int>(estimatedNonAsciiCharCount);
        _next = 0;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddBit(char key, int position)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        int block = position >> 6;
        int offset = position & 63;

        // Fast path: ASCII / extended ASCII
        if ((uint)key <= 255u)
        {
            _fixedData[_asciiMasksOffset + (key * _blocks) + block] |= 1UL << offset;
            
            // Update presence bitmap
            int presenceIndex = key >> 6;
            int presenceOffset = key & 63;
            _fixedData[_asciiPresenceOffset + presenceIndex] |= 1UL << presenceOffset;
            
            return;
        }

        // Non-ASCII: dictionary -> index -> buffer slice
        ref int index = ref _indexMap.GetOrAddValueRef(key);

        if (index == 0)
        {
            if (_next >= _capacity)
                GrowBuffer();

            index = ++_next;

            // Clear this character's slice once
            Array.Clear(_buffer, (index - 1) * _blocks, _blocks);
        }

        _buffer[(index - 1) * _blocks + block] |= 1UL << offset;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetMask(char key, out ReadOnlySpan<ulong> mask)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if ((uint)key <= 255u)
        {
            // Check presence bitmap instead of scanning the mask
            int presenceIndex = key >> 6;
            int presenceOffset = key & 63;

            if ((_fixedData[_asciiPresenceOffset + presenceIndex] & (1UL << presenceOffset)) != 0)
            {
                mask = new ReadOnlySpan<ulong>(_fixedData, _asciiMasksOffset + (key * _blocks), _blocks);
                return true;
            }

            mask = default;
            return false;
        }

        if (_indexMap.TryGetValue(key, out int index))
        {
            mask = new ReadOnlySpan<ulong>(_buffer, (index - 1) * _blocks, _blocks);
            return true;
        }

        mask = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrZero(char key)
    {
        return TryGetMask(key, out var mask) ? mask : new ReadOnlySpan<ulong>(_fixedData, _zeroMaskOffset, _blocks);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrDefault(char key, ReadOnlySpan<ulong> fallback)
    {
        return TryGetMask(key, out var mask) ? mask : fallback;
    }

    /// <summary>
    /// Useful helper if you want to check "known key".
    /// For ASCII we check the presence bitmap.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(char key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if ((uint)key <= 255u)
        {
            int presenceIndex = key >> 6;
            int presenceOffset = key & 63;
            return (_fixedData[_asciiPresenceOffset + presenceIndex] & (1UL << presenceOffset)) != 0;
        }

        return _indexMap.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBuffer()
    {
        int newCapacity = _capacity * 2;
        ulong[] newBuffer = _pool.Rent(newCapacity * _blocks);

        // Copy existing non-ASCII masks
        Array.Copy(_buffer, 0, newBuffer, 0, _capacity * _blocks);

        _pool.Return(_buffer);

        _buffer = newBuffer;
        _capacity = newCapacity;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _indexMap.Dispose();

        _pool.Return(_fixedData);
        _pool.Return(_buffer);

        _disposed = true;
    }
}
```

FuzzySharp/Utils/Permutation.cs
```
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Utils;

public class Permutor<T> where T : IComparable<T>
{
    private readonly List<T> _set;

    public Permutor(IEnumerable<T> set)
    {
        _set = set.ToList();
    }

    public List<T> PermutationAt(long i)
    {
        var set = new List<T>(_set.OrderBy(e => e));
        for (long j = 0; j < i - 1; j++)
        {
            NextPermutation(set);
        }
        return set;
    }

    public List<T> NextPermutation()
    {
        NextPermutation(_set);
        return _set;
    }

    public bool NextPermutation(List<T> set)
    {
        // Find non-increasing suffix
        int i = set.Count - 1;
        while (i > 0 && set[i - 1].CompareTo(set[i]) >= 0)
            i--;
        if (i <= 0)
            return false;

        // Find successor to pivot
        int j = set.Count - 1;
        while (set[j].CompareTo(set[i - 1]) <= 0)
            j--;
        T temp = set[i - 1];
        set[i - 1] = set[j];
        set[j] = temp;

        // Reverse suffix
        j = set.Count - 1;
        while (i < j)
        {
            temp = set[i];
            set[i] = set[j];
            set[j] = temp;
            i++;
            j--;
        }
        return true;
    }
}

public static class Permutation
{
    private static IEnumerable<List<T>> AllPermutations<T>(this IEnumerable<T> seed)
    {
        var set = new List<T>(seed);
        return Permute(set, 0, set.Count - 1);
    }

    public static IEnumerable<List<T>> PermutationsOfSize<T>(this List<T> seed, int size)
    {
        var result = seed.Count < size 
            ? [] 
            : seed.PermutationsOfSize([], size);

        return result;
    }

    private static IEnumerable<List<T>> PermutationsOfSize<T>(this List<T> seed, List<T> set, int size)
    {
        if (size == 0)
        {
            foreach (var permutation in set.AllPermutations())
            {
                yield return permutation;
            }

            yield break;
        }

        for (int i = 0; i < seed.Count; i++)
        {
            var newSet = new List<T>(set) { seed[i] };
            foreach (var permutation in seed.Skip(i + 1).ToList().PermutationsOfSize(newSet, size - 1))
            {
                yield return permutation;
            }
        }
    }

    private static IEnumerable<List<T>> Permute<T>(List<T> set, int start, int end)
    {
        if (start == end)
        {
            yield return [..set];
        }
        else
        {
            for (int i = start; i <= end; i++)
            {
                Swap(set, start, i);
                foreach (var v in Permute(set, start + 1, end))
                {
                    yield return v;
                }
                Swap(set, start, i);
            }
        }
    }

    private static void Swap<T>(List<T> set, int a, int b)
    {
        (set[a], set[b]) = (set[b], set[a]);
    }

    public static IEnumerable<List<T>> Cycles<T>(IEnumerable<T> seed)
    {
        var set = new LinkedList<T>(seed);
        for (int i = 0; i < set.Count; i++)
        {
            yield return [..set];
            var top = set.First!;
            set.RemoveFirst();
            set.AddLast(top);
        }
    }

    public static bool IsPermutationOf<T>(this IEnumerable<T> set, IEnumerable<T> other)
    {
        var hashedSet = new HashSet<T>(set);
        return hashedSet.SetEquals(other);
    }
}
```

FuzzySharp/Utils/Polyfill.cs
```
﻿using System;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

internal static class Polyfill
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ulong value)
    {

#if NET6_0_OR_GREATER
        return System.Numerics.BitOperations.PopCount(value);
#else
        value -= value >> 1 & 6148914691236517205UL /*0x5555555555555555*/;
        value = (ulong)(((long)value & 3689348814741910323L /*0x3333333333333333*/) + ((long)(value >> 2) & 3689348814741910323L /*0x3333333333333333*/));
        value = (ulong)(((long)value + (long)(value >> 4) & 1085102592571150095L) * 72340172838076673L >>> 56);
        return (int)value;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ArrayFill<T>(T[] array, T value, int startIndex, int count)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        Array.Fill(array, value, startIndex, count);
        return;
#endif

        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if ((uint)startIndex > (uint)array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if ((uint)count > (uint)(array.Length - startIndex))
            throw new ArgumentOutOfRangeException(nameof(count));

        for (var i = startIndex; i < startIndex + count; i++)
        {
            array[i] = value;
        }
    }
}
```

FuzzySharp/Utils/SequenceUtils.cs
```
﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

internal static class SequenceUtils
{
    public static int CommonPrefix<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        int prefixLength = 0;
        int minLength = Math.Min(s1.Length, s2.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (!s1[i].Equals(s2[i]))
            {
                break;
            }
            prefixLength++;
        }
        return prefixLength;
    }

    public static int CommonSuffix<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        int suffixLength = 0;
        int minLength = Math.Min(s1.Length, s2.Length);
        for (int i = 1; i <= minLength; i++)
        {
            if (!s1[^i].Equals(s2[^i]))
            {
                break;
            }
            suffixLength++;
        }
        return suffixLength;
    }

    public static (int PrefixLength, int SuffixLength) CommonAffix<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        int prefixLength = CommonPrefix(s1, s2);
        int suffixLength = CommonSuffix(s1[prefixLength..], s2[prefixLength..]);
        return (prefixLength, suffixLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TrimCommonAffixAndSwapIfNeeded<T>(ref ReadOnlySpan<T> source, ref ReadOnlySpan<T> target) where T : IEquatable<T>
    {
        _ = TrimCommonAffix(ref source, ref target);

        SwapIfSourceIsLonger(ref source, ref target);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int PrefixLength, int SuffixLength) TrimCommonAffix<T>(ref ReadOnlySpan<T> source, ref ReadOnlySpan<T> target) where T : IEquatable<T>
    {
        var startIndex = 0;
        var sourceEnd = source.Length;
        var targetEnd = target.Length;

        while (startIndex < sourceEnd && startIndex < targetEnd && EqualityComparer<T>.Default.Equals(source[startIndex], target[startIndex]))
        {
            startIndex++;
        }
        while (startIndex < sourceEnd && startIndex < targetEnd && EqualityComparer<T>.Default.Equals(source[sourceEnd - 1], target[targetEnd - 1]))
        {
            sourceEnd--;
            targetEnd--;
        }

        var sourceLength = sourceEnd - startIndex;
        var targetLength = targetEnd - startIndex;

        source = source.Slice(startIndex, sourceLength);
        target = target.Slice(startIndex, targetLength);

        return (startIndex, source.Length - sourceEnd);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SwapIfSourceIsLonger<T>(ref ReadOnlySpan<T> source, ref ReadOnlySpan<T> target)
    {
        if (source.Length <= target.Length)
        {
            return false;
        }

        var temp = source;
        source = target;
        target = temp;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SwapIfSourceIsLonger(ref string source, ref string target)
    {
        if (source.Length <= target.Length)
        {
            return false;
        }

        (source, target) = (target, source);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SwapIfSourceIsLonger<T>(ref List<T> collection1, ref List<T> collection2)
    {
        if (collection1.Count <= collection2.Count)
        {
            return false;
        }

        (collection1, collection2) = (collection2, collection1);

        return true;
    }
}
```

FuzzySharp/Utils/ThrowHelper.cs
```
﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

internal static class ThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported()
    {
        throw new InvalidOperationException("InvalidOperation_ConcurrentOperationsNotSupported");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowKeyArgumentNullException()
    {
        throw new ArgumentNullException("key");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowCapacityArgumentOutOfRangeException()
    {
        throw new ArgumentOutOfRangeException("capacity");
    }
}
```

FuzzySharp/SimilarityRatio/Scorer/CachedScorerBase.cs
```
﻿namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public abstract class CachedScorerBase : ICachedRatioScorer
{
    public abstract int Score(string input2);
    public abstract void Dispose();
}
```

FuzzySharp/SimilarityRatio/Scorer/ICachedRatioScorer.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface ICachedRatioScorer: IDisposable
{
    int Score(string input2);
}
```

FuzzySharp/SimilarityRatio/Scorer/IRatioScorer.cs
```
﻿using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface IRatioScorer
{
    int Score(string input1, string input2);
    int Score(string input1, string input2, PreprocessMode preprocessMode);
}
```

FuzzySharp/SimilarityRatio/Scorer/ScorerBase.cs
```
﻿using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public abstract class ScorerBase : IRatioScorer
{
    public abstract int Score(string input1, string input2);

    public int Score(string input1, string input2, PreprocessMode preprocessMode)
    {
        var preprocessor = StringPreprocessorFactory.GetPreprocessor(preprocessMode);
        input1 = preprocessor(input1);
        input2 = preprocessor(input2);
        return Score(input1, input2);
    }
}
```

FuzzySharp/SimilarityRatio/Strategy/CachedDefaultRatioStrategy.cs
```
﻿using System;
using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal class CachedDefaultRatioStrategy : ICachedStrategy
{
    private readonly Indel _indel;
    private readonly Func<string, string> _preprocessor;

    public CachedDefaultRatioStrategy(string input1, PreprocessMode preprocess = PreprocessMode.None)
    {
        _preprocessor = StringPreprocessorFactory.GetPreprocessor(preprocess);
        _indel = new Indel(_preprocessor(input1));
    }

    public int Calculate(string input2)
    {
        var  processedInput2 = _preprocessor(input2);
        if (processedInput2.Length == 0)
        {
            return 0;
        }

        return (int)Math.Round(100 * _indel.NormalizedSimilarityWith(processedInput2));
    }

    public void Dispose()
    {
        _indel.Dispose();
    }
}

public interface ICachedStrategy : IDisposable
{
    int Calculate(string input2);
}
```

FuzzySharp/SimilarityRatio/Strategy/DefaultRatioStrategy.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class DefaultRatioStrategy
{
    public static int Calculate(string input1, string input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var input1Span = input1.AsSpan();
        var input2Span = input2.AsSpan();

        return (int)Math.Round(100 * Indel.NormalizedSimilarity(input1Span, input2Span));
    }
}
```

FuzzySharp/SimilarityRatio/Strategy/PartialRatioStrategy.cs
```
﻿using System;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class PartialRatioStrategy
{
    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns the partial fuzz.ratio for that alignment, as a value in [0…100].
    /// </summary>
    public static int Calculate(string input1, string input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var score = PartialRatioStrategy<char>.Calculate(input1.AsSpan(), input2.AsSpan());

        return score;
    }
}
```

FuzzySharp/SimilarityRatio/Scorer/Composite/CachedWeightedRatioScorer.cs
```
﻿using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public sealed class CachedWeightedRatioScorer : CachedScorerBase
{
    private static readonly double UNBASE_SCALE = .95;
    private static readonly double PARTIAL_SCALE = .90;
    private static readonly bool TRY_PARTIALS = true;

    private readonly ICachedStrategy _strategy;
    private readonly CachedDefaultRatioScorer _baseRatioScorer;
    private readonly string _input1;
    private readonly CachedTokenSortScorer _tokenSortScorer;
    private readonly CachedTokenSetScorer _tokenSetScorer;

    public CachedWeightedRatioScorer(string input1)
    {
        _input1 = input1;
        _strategy = new CachedDefaultRatioStrategy(input1);
        _baseRatioScorer = new CachedDefaultRatioScorer(_strategy);
        _tokenSortScorer = new CachedTokenSortScorer(_strategy);
        _tokenSetScorer = new CachedTokenSetScorer(_input1);
    }

    public override int Score(string input2)
    {
        int len1 = _input1.Length;
        int len2 = input2.Length;

        if (len1 == 0 || len2 == 0)
        {
            return 0;
        }

        bool tryPartials = TRY_PARTIALS;
        double unbaseScale = UNBASE_SCALE;
        double partialScale = PARTIAL_SCALE;

        int baseRatio = _baseRatioScorer.Score(input2);
        double lenRatio = (double)Math.Max(len1, len2) / Math.Min(len1, len2);

        // if strings are similar length don't use partials
        if (lenRatio < 1.5) tryPartials = false;

        // if one string is much shorter than the other
        if (lenRatio > 8) partialScale = .6;

        if (tryPartials)
        {
            double partial = Fuzz.PartialRatio(_input1, input2) * partialScale;
            double partialSor = _tokenSortScorer.Score(input2) * unbaseScale * partialScale;
            double partialSet = _tokenSetScorer.Score(input2) * unbaseScale * partialScale;

            return (int)Math.Round(Math.Max(baseRatio, Math.Max(partial, Math.Max(partialSor, partialSet))));
        }

        double tokenSort = _tokenSortScorer.Score(input2) * unbaseScale;
        double tokenSet = _tokenSetScorer.Score(input2) * unbaseScale;
        return (int)Math.Round(Math.Max(baseRatio, Math.Max(tokenSort, tokenSet)));
    }

    public override void Dispose()
    {
        _strategy.Dispose();
    }
}
```

FuzzySharp/SimilarityRatio/Scorer/Composite/WeightedRatioScorer.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public class WeightedRatioScorer : ScorerBase
{
    private static readonly double UNBASE_SCALE = .95;
    private static readonly double PARTIAL_SCALE = .90;
    private static readonly bool TRY_PARTIALS = true;

    public override int Score(string input1, string input2)
    {
        int len1 = input1.Length;
        int len2 = input2.Length;

        if (len1 == 0 || len2 == 0)
        {
            return 0;
        }

        bool tryPartials = TRY_PARTIALS;
        double unbaseScale = UNBASE_SCALE;
        double partialScale = PARTIAL_SCALE;

        int baseRatio = Fuzz.Ratio(input1, input2);
        double lenRatio = (double)Math.Max(len1, len2) / Math.Min(len1, len2);

        // if strings are similar length don't use partials
        if (lenRatio < 1.5) tryPartials = false;

        // if one string is much shorter than the other
        if (lenRatio > 8) partialScale = .6;

        if (tryPartials)
        {
            double partial = Fuzz.PartialRatio(input1, input2) * partialScale;
            double partialSor = Fuzz.TokenSortRatio(input1, input2) * unbaseScale * partialScale;
            double partialSet = Fuzz.TokenSetRatio(input1, input2) * unbaseScale * partialScale;

            return (int)Math.Round(Math.Max(baseRatio, Math.Max(partial, Math.Max(partialSor, partialSet))));
        }

        double tokenSort = Fuzz.TokenSortRatio(input1, input2) * unbaseScale;
        double tokenSet = Fuzz.TokenSetRatio(input1, input2) * unbaseScale;
        return (int)Math.Round(Math.Max(baseRatio, Math.Max(tokenSort, tokenSet)));
    }
}
```

FuzzySharp/SimilarityRatio/Scorer/Generic/CachedScorerBase.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

public abstract class CachedScorerBase<T> : ICachedRatioScorer<T> where T : IEquatable<T>
{
    public abstract int Score(T[] input2);
}
```

FuzzySharp/SimilarityRatio/Scorer/Generic/IRatioScorer.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

public interface IRatioScorer<in T> where T : IEquatable<T>
{
    int Score(T[] input1, T[] input2);
}

public interface ICachedRatioScorer<in T> where T : IEquatable<T>
{
    int Score(T[] input2);
}
```

FuzzySharp/SimilarityRatio/Scorer/Generic/ScorerBase.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

public abstract class ScorerBase<T> : IRatioScorer<T> where T : IEquatable<T>
{
    public abstract int Score(T[] input1, T[] input2);
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/CachedStrategySensitiveScorerBase.cs
```
﻿namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedStrategySensitiveScorerBase : CachedScorerBase
{
    protected abstract CachedScorer Scorer { get; }
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/StrategySensitiveScorerBase.cs
```
﻿namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class StrategySensitiveScorerBase : ScorerBase
{
    protected abstract FuzzySharp.Scorer Scorer { get; }
}
```

FuzzySharp/SimilarityRatio/Strategy/Generic/CachedDefaultRatioStrategyT.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal class CachedDefaultRatioStrategy<T>(T[] input1) : IDisposable
    where T : IEquatable<T>
{
    private readonly IndelT<T> _indel = new(input1);

    public int Calculate(T[] input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var result = (int)Math.Round(100 * _indel.NormalizedSimilarityWith(input2));

        return result;
    }

    public void Dispose()
    {
        _indel.Dispose();
    }
}
```

FuzzySharp/SimilarityRatio/Strategy/Generic/DefaultRatioStrategyT.cs
```
﻿using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class DefaultRatioStrategy<T> where T : IEquatable<T>
{
    public static int Calculate(T[] input1, T[] input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }
            
        var result = (int)Math.Round(100 * Indel.NormalizedSimilarity((ReadOnlySpan<T>)input1, (ReadOnlySpan<T>)input2));

        return result;
    }
}
```

FuzzySharp/SimilarityRatio/Strategy/Generic/PartialRatioStrategyT.cs
```
﻿using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class PartialRatioStrategy<T> where T : IEquatable<T>
{
    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns the partial fuzz.ratio for that alignment, as a value in [0…100].
    /// </summary>
    public static int Calculate(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var alignment = PartialRatioAlignment(input1, input2);

        return (int)Math.Round(alignment.Score);
    }

    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns a ScoreAlignment (with a score in [0…100]) or null if below cutoff.
    /// </summary>
    internal static ScoreAlignment PartialRatioAlignment(
        ReadOnlySpan<T> shorter,
        ReadOnlySpan<T> longer,
        Processor<T> processor = null,
        double? scoreCutoff = null
    )
    {
        // 1) Optional preprocessing
        if (processor != null)
        {
            processor(ref shorter);
            processor(ref longer);
        }

        // 2) Normalize cutoff to 0…100
        double cutoff100 = scoreCutoff.GetValueOrDefault();

        // 3) Handle both empty → perfect match
        if (shorter.IsEmpty && longer.IsEmpty)
        {
            return new ScoreAlignment(100.0, 0, 0, 0, 0);
        }

        // 4) Determine shorter/longer
        var swapped = SequenceUtils.SwapIfSourceIsLonger(ref shorter, ref longer);

        // 5) Call the core PartialRatioImpl with cutoff in [0..1]
        double fracCutoff = cutoff100 / 100.0;
        var res = PartialRatioImpl(shorter, longer, fracCutoff);

        // 6) If same-length inputs and not perfect, try the other direction
        if (res.Score < 100.0 && shorter.Length == longer.Length)
        {
            // bump cutoff to whatever we got
            double newCutoff100 = Math.Max(cutoff100, res.Score);
            double newFracCutoff = newCutoff100 / 100.0;

            var res2 = PartialRatioImpl(longer, shorter, newFracCutoff);
            if (res2.Score > res.Score)
            {
                // swap src/dest
                res = new ScoreAlignment(
                    res2.Score,
                    SrcStart: res2.DestStart,
                    SrcEnd: res2.DestEnd,
                    DestStart: res2.SrcStart,
                    DestEnd: res2.SrcEnd
                );
            }
        }

        // 7) If below cutoff, return null
        if (res.Score < cutoff100)
            return res with { Score = 0 };

        // 8) If we swapped at step 4, swap back the src/dest in the result
        if (swapped)
        {
            res = new ScoreAlignment(
                res.Score,
                SrcStart: res.DestStart,
                SrcEnd: res.DestEnd,
                DestStart: res.SrcStart,
                DestEnd: res.SrcEnd
            );
        }

        return res;
    }

    /// <summary>
    /// C# equivalent of rapidfuzz.distance._partial_ratio_impl
    /// Assumes s1.Length <= s2.Length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ScoreAlignment PartialRatioImpl(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        double? scoreCutoff = null
    )
    {
        int len1 = s1.Length, len2 = s2.Length;
        if (len1 > len2)
            throw new ArgumentException("Requires s1.Length <= s2.Length");

        using var patternMatchVector = PatternMatchVector.Create(s1);
        return PartialRatioImpl(s1, s2, patternMatchVector, scoreCutoff);
    }

    /// <summary>
    /// C# equivalent of rapidfuzz.distance._partial_ratio_impl
    /// Assumes s1.Length <= s2.Length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ScoreAlignment PartialRatioImpl(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,
        double? scoreCutoff = null
    )
    {
        int len1 = s1.Length, len2 = s2.Length;
        if (len1 > len2)
            throw new ArgumentException("Requires s1.Length <= s2.Length");

        // Initial best covers s2[0..len1)
        var res = new ScoreAlignment(0, 0, len1, 0, len1);

        if (len1 == 0 || len2 == 0)
            return res;

        double cutoff = scoreCutoff ?? 0.0;

        if (len2 > len1)
        {
            int maximum = len1 + len1;
            int windowCount = len2 - len1;
            int cutoffDist = (int)Math.Ceiling(maximum * (1.0 - cutoff));
            int bestDist = int.MaxValue;
            int[] scores = ArrayPool<int>.Shared.Rent(windowCount);

            Polyfill.ArrayFill(scores, int.MaxValue, 0, windowCount);

            try
            {
                var windows = new List<(int First, int Second)>(4) { (0, windowCount - 1) };
                var newWindows = new List<(int First, int Second)>(4);

                while (windows.Count > 0)
                {
                    foreach (var window in windows)
                    {
                        int first = window.First;
                        int second = window.Second;

                        if (scores[first] == int.MaxValue)
                        {
                            int dist = Indel.DistanceImpl(s1, s2.Slice(first, len1), patternMatchVector);
                            scores[first] = dist;
                            if (dist < cutoffDist)
                            {
                                cutoffDist = bestDist = dist;
                                res.DestStart = first;
                                res.DestEnd = first + len1;
                                if (bestDist == 0)
                                {
                                    res.Score = 100.0;
                                    return res;
                                }
                            }
                        }

                        if (scores[second] == int.MaxValue)
                        {
                            int dist = Indel.DistanceImpl(s1, s2.Slice(second, len1), patternMatchVector);
                            scores[second] = dist;
                            if (dist < cutoffDist)
                            {
                                cutoffDist = bestDist = dist;
                                res.DestStart = second;
                                res.DestEnd = second + len1;
                                if (bestDist == 0)
                                {
                                    res.Score = 100.0;
                                    return res;
                                }
                            }
                        }

                        int cellDiff = second - first;
                        if (cellDiff == 1)
                            continue;

                        int knownEdits = Math.Abs(scores[first] - scores[second]);
                        int maxScoreImprovement = ((cellDiff - knownEdits / 2) / 2) * 2;
                        int minScore = Math.Min(scores[first], scores[second]) - maxScoreImprovement;
                        if (minScore < cutoffDist)
                        {
                            int center = cellDiff / 2;
                            newWindows.Add((first, first + center));
                            newWindows.Add((first + center, second));
                        }
                    }

                    if (newWindows.Count == 0)
                        break;

                    windows.Clear();
                    (windows, newWindows) = (newWindows, windows);
                }

                if (bestDist != int.MaxValue)
                {
                    double score = 1.0 - (bestDist / (double)maximum);
                    if (score >= cutoff)
                    {
                        cutoff = res.Score = score;
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(scores);
            }
        }

        // 1) Prefixes shorter than len1
        for (int i = 1; i < len1; i++)
        {
            if (!patternMatchVector.ContainsKey(s2[i - 1])) continue;
            var slice = s2[..i];
            double sim = Indel.BlockNormalizedSimilarity(patternMatchVector, s1, slice);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = 0;
                res.DestEnd = i;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        // 2) Suffixes up to len1 (includes the last full-width window)
        for (int i = len2 - len1; i < len2; i++)
        {
            if (!patternMatchVector.ContainsKey(s2[i])) continue;
            var tail = s2[i..];
            double sim = Indel.BlockNormalizedSimilarity(patternMatchVector, s1, tail);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = i;
                res.DestEnd = len2;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        res.Score *= 100.0;

        return res;
    }

    internal record struct ScoreAlignment(double Score, int SrcStart, int SrcEnd, int DestStart, int DestEnd);
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Generic/StrategySensitiveScorerBase.cs
```
﻿using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive.Generic;

public abstract class StrategySensitiveScorerBase<T> : ScorerBase<T> where T : IEquatable<T>
{
    protected abstract Func<T[], T[], int> Scorer { get; }
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/CachedDefaultRatioScorer.cs
```
﻿using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedDefaultRatioScorer : CachedSimpleRatioScorerBase
{
    private readonly ICachedStrategy _strategy;
    private readonly bool _isStrategyOwner;

    public CachedDefaultRatioScorer(string input1, PreprocessMode preprocessMode = PreprocessMode.None)
    {
        _strategy = new CachedDefaultRatioStrategy(input1, preprocessMode);
        _isStrategyOwner = true;
    }

    public CachedDefaultRatioScorer(ICachedStrategy strategy, bool isStrategyOwner = false)
    {
        _strategy = strategy;
        _isStrategyOwner = isStrategyOwner;
    }

    protected override CachedScorer Scorer => input2 => _strategy.Calculate(input2);
    public override void Dispose()
    {
        if (_isStrategyOwner)
        {
            _strategy.Dispose();
        }
    }
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/CachedSimpleRatioScorerBase.cs
```
﻿namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedSimpleRatioScorerBase : CachedStrategySensitiveScorerBase
{
    public override int Score(string input2)
    {
        return Scorer(input2);
    }
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/DefaultRatioScorer.cs
```
﻿using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class DefaultRatioScorer : SimpleRatioScorerBase
{
    protected override FuzzySharp.Scorer Scorer => DefaultRatioStrategy.Calculate;
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/PartialRatioScorer.cs
```
﻿using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class PartialRatioScorer : SimpleRatioScorerBase
{
    protected override FuzzySharp.Scorer Scorer => PartialRatioStrategy.Calculate;
}
```

FuzzySharp/SimilarityRatio/Scorer/StrategySensitive/Simple/SimpleRatioScorerBase.cs
```
﻿namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class SimpleRatioScorerBase : StrategySensitiveScorerBase
{
    public override int Score(string input1, string input2)
    {
        return Scorer(input1, input2);
    }
}
```

</source_code>