<source_code>
Directory.Build.props
```
<Project>
  <Import Project="build\SourceLink.props" Condition="'$(DisableSourceLink)' == ''" />
</Project>
```

FuzzySharp.sln
```
﻿
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.10.35122.118
MinimumVisualStudioVersion = 10.0.40219.1
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FuzzySharp", "FuzzySharp\FuzzySharp.csproj", "{348B90DA-DA44-45AD-B857-D3A69D05AE46}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FuzzySharp.Test", "FuzzySharp.Test\FuzzySharp.Test.csproj", "{48F4C7CB-E669-410C-A455-DE3330347807}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FuzzySharp.Benchmarks", "FuzzySharp.Benchmarks\FuzzySharp.Benchmarks.csproj", "{480CAE39-ACA7-411A-BF6B-72E61ED6E129}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{348B90DA-DA44-45AD-B857-D3A69D05AE46}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{348B90DA-DA44-45AD-B857-D3A69D05AE46}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{348B90DA-DA44-45AD-B857-D3A69D05AE46}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{348B90DA-DA44-45AD-B857-D3A69D05AE46}.Release|Any CPU.Build.0 = Release|Any CPU
		{48F4C7CB-E669-410C-A455-DE3330347807}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{48F4C7CB-E669-410C-A455-DE3330347807}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{48F4C7CB-E669-410C-A455-DE3330347807}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{48F4C7CB-E669-410C-A455-DE3330347807}.Release|Any CPU.Build.0 = Release|Any CPU
		{480CAE39-ACA7-411A-BF6B-72E61ED6E129}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{480CAE39-ACA7-411A-BF6B-72E61ED6E129}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{480CAE39-ACA7-411A-BF6B-72E61ED6E129}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{480CAE39-ACA7-411A-BF6B-72E61ED6E129}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {8B80F7B1-E7E5-4BDA-93E7-5596C608656F}
	EndGlobalSection
EndGlobal
```

.github/FUNDING.yml
```
github: [Raffinert, ycherkes]
custom: ["https://www.paypal.com/donate/?business=KXGF7CMW8Y8WJ"]
```

FuzzySharp.Benchmarks/ExtractAllBenchmarks.cs
```
using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class ExtractAllBenchmarks
{
    private static readonly string[][] Events =
    [
        ["chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm"],
        ["new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm"],
        ["atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm"]
    ];

    private static readonly string[] Query = ["new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm"];
    private ICachedRatioScorer _extractScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _extractScorer = new CachedWeightedRatioScorer(Query[0]);
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAll()
    {
        return Process.ExtractAll(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<global::FuzzySharp.Extractor.ExtractedResult<string[]>> ExtractAllClassic()
    {
        return Classic.Process.ExtractAll(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllCached()
    {
        return Process.Cached.ExtractAll(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllAcrossRunsCached()
    {
        return Process.Cached.ExtractAll(Query, Events, static strings => strings[0], _extractScorer).ToList();
    }
}
```

FuzzySharp.Benchmarks/ExtractOneBenchmarks.cs
```
using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class ExtractOneBenchmarks
{
    private static readonly string[][] Events =
    [
        ["chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm"],
        ["new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm"],
        ["atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm"]
    ];

    private static readonly string[] Query = ["new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm"];
    private ICachedRatioScorer _extractScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _extractScorer = new CachedWeightedRatioScorer(Query[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOne()
    {
        return Process.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public Classic.Extractor.ExtractedResult<string[]> ExtractOneClassic()
    {
        return Classic.Process.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneCached()
    {
        return Process.Cached.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneAcrossRunsCached()
    {
        return Process.Cached.ExtractOne(Query, Events, static strings => strings[0], _extractScorer);
    }
}
```

FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>NET10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AssemblyName>$(MSBuildProjectName)</AssemblyName>
    <RootNamespace>Raffinert.$(MSBuildProjectName.Replace(" ", "_"))</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" Version="0.15.2" />
    <PackageReference Include="Fastenshtein" Version="1.0.10" />
    <PackageReference Include="FuzzySharp" Version="2.0.2" />
    <PackageReference Include="Microsoft.VisualStudio.DiagnosticsHub.BenchmarkDotNetDiagnosers" Version="18.3.36812.1" />
    <PackageReference Include="Quickenshtein" Version="1.5.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\FuzzySharp\FuzzySharp.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="BenchmarkDotNet.Artifacts\results\" />
  </ItemGroup>

</Project>
```

FuzzySharp.Benchmarks/LevenshteinDistanceBenchmarks.cs
```
using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class LevenshteinDistanceBenchmarks
{
    private static readonly Levenshtein FuzzySharpLevenshtein = new("chicago cubs vs new york mets");
    private static readonly Fastenshtein.Levenshtein FastenLevenshtein = new("chicago cubs vs new york mets");

    [Benchmark]
    public int FuzzySharpDistance()
    {
        return Levenshtein.Distance("chicago cubs vs new york mets", "new york mets vs chicago cubs");
    }

    [Benchmark]
    public int FuzzySharpClassicDistance()
    {
        return Classic.Levenshtein.EditDistance("chicago cubs vs new york mets", "new york mets vs chicago cubs");
    }

    [Benchmark]
    public int FastenshteinDistance()
    {
        return Fastenshtein.Levenshtein.Distance("chicago cubs vs new york mets", "new york mets vs chicago cubs");
    }

    [Benchmark]
    public int QuickenshteinDistance()
    {
        return Quickenshtein.Levenshtein.GetDistance("chicago cubs vs new york mets", "new york mets vs chicago cubs");
    }

    [Benchmark]
    public int FuzzySharpDistanceFrom()
    {
        return FuzzySharpLevenshtein.DistanceFrom("new york mets vs chicago cubs");
    }

    [Benchmark]
    public int FastenshteinDistanceFrom()
    {
        return FastenLevenshtein.DistanceFrom("new york mets vs chicago cubs");
    }
}
```

FuzzySharp.Benchmarks/PartialRatioBenchmarks.cs
```
using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class PartialRatioBenchmarks
{
    [Benchmark]
    public int PartialRatio()
    {
        return Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public int PartialRatioClassic()
    {
        return Classic.Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }
}
```

FuzzySharp.Benchmarks/Program.cs
```
﻿using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.ShortRun);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
```

FuzzySharp.Benchmarks/RatioBenchmarks.cs
```
using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class RatioBenchmarks
{
    private ICachedRatioScorer _cachedRatioScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedRatioScorer = new CachedDefaultRatioScorer("mysmilarstring");
    }

    [Benchmark]
    public int Ratio()
    {
        return Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int RatioClassic()
    {
        return Classic.Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int RatioCached()
    {
        using var scorer = new CachedDefaultRatioScorer("mysmilarstring");
        return scorer.Score("myawfullysimilarstirng");
    }

    [Benchmark]
    public int RatioAcrossRunsCached()
    {
        return _cachedRatioScorer.Score("myawfullysimilarstirng");
    }
}
```

FuzzySharp.Benchmarks/WeightedRatioBenchmarks.cs
```
using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class WeightedRatioBenchmarks
{
    private ICachedRatioScorer _cachedWeightedScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedWeightedScorer = new CachedWeightedRatioScorer("The quick brown fox jimps ofver the small lazy dog");
    }

    [Benchmark]
    public int WeightedRatio()
    {
        return Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int WeightedRatioClassic()
    {
        return Classic.Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int WeightedRatioCached()
    {
        return new CachedWeightedRatioScorer("The quick brown fox jimps ofver the small lazy dog").Score("the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int WeightedRatioAcrossRunsCached()
    {
        return _cachedWeightedScorer.Score("the quick brown fox jumps over the small lazy dog");
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
        return NumericsPolyfill.PopCount(inv);
    }

    private static int CountZeroBits(ReadOnlySpan<ulong> S, int length)
    {
        int fullBlocks = length / 64;
        int remBits = length % 64;
        int zeros = 0;

        // all full blocks
        for (int i = 0; i < fullBlocks; i++)
            zeros += NumericsPolyfill.PopCount(~S[i]);

        // last partial block
        if (remBits > 0)
        {
            ulong mask = (1UL << remBits) - 1;
            zeros += NumericsPolyfill.PopCount(~S[fullBlocks] & mask);
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

FuzzySharp/Process.Cached.cs
```
﻿using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp;

public static partial class Process
{
    public static class Cached
    {
        public static IEnumerable<ExtractedResult<string>> ExtractAll(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            string query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(query);
            foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<string>> ExtractTop(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int limit = 5,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer, limit, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer1, limit, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int limit = 5,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer, limit, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer1, limit, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<string>> ExtractSorted(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static ExtractedResult<string> ExtractOne(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                return ResultExtractor.ExtractOne(choices, processor, scorer, cutoff);
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            return ResultExtractor.ExtractOne(choices, processor, scorer1, cutoff);
        }

        public static ExtractedResult<T> ExtractOne<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                return ResultExtractor.ExtractOne(choices, processor, scorer, cutoff);
            }
            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            return ResultExtractor.ExtractOne(choices, processor, scorer1, cutoff);
        }

        public static ExtractedResult<string> ExtractOne(string query, params string[] choices)
        {
            using var scorer = new CachedWeightedRatioScorer(DefaultStringProcessor(query));
            return ResultExtractor.ExtractOne(choices, DefaultStringProcessor, scorer);
        }
    }
}
```

FuzzySharp/Process.cs
```
﻿using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

public static partial class Process
{
    private static readonly IRatioScorer DefaultScorer = ScorerCache.Get<WeightedRatioScorer>();
    private static readonly Func<string, string> DefaultStringProcessor = StringPreprocessorFactory.GetPreprocessor(PreprocessMode.Full);

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

FuzzySharp.Test/DictionarySlimTests.cs
```
﻿using NUnit.Framework;
using Raffinert.FuzzySharp.Utils;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class DictionarySlimTests
{
    [Test, TestCaseSource(typeof(RandomWordPairs), nameof(RandomWordPairs.GetWordPairs))]
    public void DictionarySlim_And_Dictionary_ShouldHaveEqualResults(string s1, string s2)
    {
        var ds1 = new DictionarySlimPooled<char, int>(64);
        var d1 = new Dictionary<char, int>(64);

        for (var index = 0; index < s1.Length; index++)
        {
            var c = s1[index];
            ref var val = ref ds1.GetOrAddValueRef(c);
            val = index + 1;
            d1[c] = index + 1;
        }

        foreach (var c in s1)
        {
            Assert.True(ds1.TryGetValue(c, out var value));
            Assert.AreEqual(value, d1[c]);
        }

        var ds2 = new DictionarySlimPooled<char, int>(64);
        var d2 = new Dictionary<char, int>(64);

        for (var index = 0; index < s2.Length; index++)
        {
            var c = s2[index];
            ref var val = ref ds2.GetOrAddValueRef(c);
            val = index + 1;

            d2[c] = index + 1;
        }

        foreach (var c in s2)
        {
            Assert.True(ds2.TryGetValue(c, out var value));
            Assert.AreEqual(value, d2[c]);
        }
    }
}
```

FuzzySharp.Test/EditOpsTests.cs
```
﻿using NUnit.Framework;
using Raffinert.FuzzySharp.Edits;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class EditOpsTests
{
    [Test]
    public void GetEditOps_KittenToSitting_ReturnsExpectedEditOps()
    {
        // Arrange
        string source = "kitten";
        string target = "sitting";

        // Act
        var ops = Levenshtein.GetEditOps(source, target);

        // Assert
        Assert.IsNotNull(ops);
        Assert.AreEqual(3, ops.Length);

        Assert.AreEqual(EditType.REPLACE, ops[0].EditType);
        Assert.AreEqual(0, ops[0].SourcePos);
        Assert.AreEqual(0, ops[0].DestPos);

        Assert.AreEqual(EditType.REPLACE, ops[1].EditType);
        Assert.AreEqual(4, ops[1].SourcePos);
        Assert.AreEqual(4, ops[1].DestPos);

        Assert.AreEqual(EditType.INSERT, ops[2].EditType);
        Assert.AreEqual(6, ops[2].SourcePos);
        Assert.AreEqual(6, ops[2].DestPos);
    }

    [Test]
    public void GetEditOps_putinIsWarCriminal_ReturnsExpectedEditOps()
    {
        // Arrange
        string source = "putin";
        string target = "war criminal";

        // Act
        var ops = Levenshtein.GetEditOps(source, target);

        Assert.That(ops, Is.EquivalentTo(new[]
        {
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 0,
                DestPos = 0
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 0,
                DestPos = 1
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 0,
                DestPos = 2
            },
            new EditOp
            {
                EditType = EditType.REPLACE,
                SourcePos = 0,
                DestPos = 3
            },
            new EditOp
            {
                EditType = EditType.REPLACE,
                SourcePos = 1,
                DestPos = 4
            },
            new EditOp
            {
                EditType = EditType.REPLACE,
                SourcePos = 2,
                DestPos = 5
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 4,
                DestPos = 7
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 4,
                DestPos = 8
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 5,
                DestPos = 10
            },
            new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = 5,
                DestPos = 11
            }
        }));
    }
}
```

FuzzySharp.Test/FuzzySharp.Test.csproj
```
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>netframework4.6.2;netframework4.7.2;NET8.0;NET10.0</TargetFrameworks>
    <IsPackable>false</IsPackable>
	<LangVersion>12.0</LangVersion>
	<AssemblyName>Raffinert.$(MSBuildProjectName)</AssemblyName>
	<RootNamespace>Raffinert.$(MSBuildProjectName.Replace(" ", "_"))</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FuzzySharp" Version="2.0.2" />
    <PackageReference Include="nunit" Version="3.14.0" />
    <PackageReference Include="NUnit.Console" Version="3.20.1" />
    <PackageReference Include="NUnit3TestAdapter" Version="5.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="Quickenshtein" Version="1.5.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\FuzzySharp\FuzzySharp.csproj" />
  </ItemGroup>

</Project>
```

FuzzySharp.Test/LevenshteinTests.cs
```
﻿using NUnit.Framework;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class LevenshteinTests
{
    [Test]
    [TestCase(
    "I had two heart attacks, an abortion, did crack... while I was pregnant. Other than that, I'm fine.",
    "You couldn't even be a vegetable - even artichokes have a heart.",
    76)]
    [TestCase("", "", 0)]
    [TestCase("a", "", 1)]
    [TestCase("", "a", 1)]
    [TestCase("kitten", "kitten", 0)]
    [TestCase("kitten", "sitting", 3)]     // substitution + insertion + insertion
    [TestCase("flaw", "lawn", 2)]          // substitution + insertion
    [TestCase("gumbo", "gambol", 2)]       // insertion + substitution
    [TestCase("book", "back", 2)]          // two substitutions
    [TestCase("Sunday", "Saturday", 3)]    // insertion + substitution + insertion
    // Test a few boundary scenarios (longer strings, only one‐character difference)
    [TestCase("a", "b", 1)]
    [TestCase("ab", "ba", 2)]
    [TestCase("abcdef", "azced", 3)]
    [TestCase("distance", "difference", 5)]
    public void TestLevenshteinDistance(string s1, string s2, int expectedDistance)
    {
        int distance = Levenshtein.Distance(s1, s2);
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void TestLevenshteinDistance()
    {
        var wordA = new string('A', 4112);
        var wordB = new string('B', 4112);
        int maxDistance = Levenshtein.Distance(wordA, wordB);
        int zeroDistance = Levenshtein.Distance(wordA, wordA);
        Assert.AreEqual(4112, maxDistance);
        Assert.AreEqual(0, zeroDistance);
    }

    [Test, TestCaseSource(typeof(RandomWordPairs), nameof(RandomWordPairs.GetWordPairs))]
    public void Levenshtein_ShouldBeEqual(string s1, string s2)
    {
        var fd = Levenshtein.Distance(s1, s2);
        var eo = Levenshtein.GetEditOps(s1, s2);
        //var mb1 = eo.AsMatchingBlocks(s1.Length, s2.Length);
        //var mb2 = global::FuzzySharp.Levenshtein.GetMatchingBlocks(s1, s2);
        var qd = Quickenshtein.Levenshtein.GetDistance(s1, s2);

        Assert.That(fd, Is.EqualTo(qd));
        Assert.That(eo.Length, Is.EqualTo(qd));
        //Assert.That(mb, Is.EquivalentTo(mb2));
    }
}
```

FuzzySharp.Test/RandomWords.cs
```
﻿using NUnit.Framework;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Test;

public static class RandomWordPairs
{
    public static IEnumerable<TestCaseData> GetWordPairs()
    {
        var words = RandomWords.Create(50, 1024);

        var result = from word1 in words
                     from word2 in words
                     select new TestCaseData(word1, word2);

        return result;
    }
}

// original https://github.com/DanHarltey/Fastenshtein/blob/master/benchmarks/Fastenshtein.Benchmarking/RandomWords.cs
public static class RandomWords
{
    private static readonly char[] Chars = Enumerable.Range(char.MinValue, char.MaxValue + 1)
        .Select(c => (char)c)
        .Where(char.IsLetterOrDigit)
        .ToArray();


    public static string[] Create(int count, int maxWordSize)
    {
        var words = new string[count];

        // using a const seed to make sure runs of the performance tests are consistent.
        var random = new Random(37);

        for (var i = 0; i < words.Length; i++)
        {
            var wordSize = random.Next(3, maxWordSize);

            words[i] = StringCompat.Create(wordSize, random, static (word, r) =>
            {
                for (var j = 0; j < word.Length; j++)
                {
                    var index = r.Next(0, Chars.Length);
                    word[j] = Chars[index];
                }
            });
        }

        return words;
    }

    public static class StringCompat
    {
        public static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
        {
#if NETCOREAPP
            return string.Create(length, state, action);
#else

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var chars = new char[length];
            action(chars.AsSpan(), state);
            return new string(chars);
#endif
        }
    }
#if !NETCOREAPP
    public delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);
#endif
}
```

.github/workflows/development_package.yml
```
name: CI Build

on:
  push:
    branches: 
      - development

jobs:
  build:
    runs-on: windows-latest
    name: Build
    steps:
      - name: Checkout repository
        uses: actions/checkout@v1
        
      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.0.100
        
      - name: Build with dotnet
        run: dotnet build --configuration Release

      - name: Test with dotnet
        run: dotnet test
  deploy:
    needs: [Build]
    name: Package
    runs-on: [windows-latest]
    steps:
      - uses: actions/checkout@v1
      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.0.100
        
      - name: Build with dotnet
        run: dotnet build --configuration Release 

      - name: Pack nuget package
        run: dotnet pack --configuration Release --include-symbols -p:SymbolPackageFormat=snupkg
```

.github/workflows/master_package_and_publish.yml
```
name: Nuget Package Deploy

on:
  push:
    branches: 
      - master

jobs:
  build:
    runs-on: windows-latest
    name: Build
    steps:
      - name: Checkout repository
        uses: actions/checkout@v1
        
      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.0.100
        
      - name: Build with dotnet
        run: dotnet build --configuration Release

      - name: Test with dotnet
        run: dotnet test
  deploy:
    needs: [Build]
    name: Package and Publish
    runs-on: [windows-latest]
    steps:
      - uses: actions/checkout@v1
      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.0.100
        
      - name: Build with dotnet
        run: dotnet build --configuration Release

      - name: Pack nuget package
        run: dotnet pack --configuration Release --include-symbols -p:SymbolPackageFormat=snupkg

      - name: Push package to NuGet
        run: dotnet nuget push **/*.nupkg
              --skip-duplicate
              --api-key ${{ secrets.NUGET_DEPLOY_KEY }}
              --source https://api.nuget.org/v3/index.json
```

docs/projects/FuzzySharp.ai-context.md
```
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
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.ExtractAllBenchmarks-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                     | Mean      | Error     | StdDev    | Gen0   | Allocated |
|--------------------------- |----------:|----------:|----------:|-------:|----------:|
| ExtractAll                 |  6.280 μs | 1.4762 μs | 0.0809 μs | 0.7172 |  11.05 KB |
| ExtractAllClassic          | 13.620 μs | 3.7008 μs | 0.2029 μs | 1.7090 |  26.31 KB |
| ExtractAllCached           |  4.922 μs | 0.0016 μs | 0.0001 μs | 0.6027 |   9.34 KB |
| ExtractAllAcrossRunsCached |  4.638 μs | 0.8236 μs | 0.0451 μs | 0.5493 |   8.48 KB |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.ExtractAllBenchmarks-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Gen0,Allocated
ExtractAll,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,6.280 μs,1.4762 μs,0.0809 μs,0.7172,11.05 KB
ExtractAllClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,13.620 μs,3.7008 μs,0.2029 μs,1.7090,26.31 KB
ExtractAllCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,4.922 μs,0.0016 μs,0.0001 μs,0.6027,9.34 KB
ExtractAllAcrossRunsCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,4.638 μs,0.8236 μs,0.0451 μs,0.5493,8.48 KB
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.ExtractAllBenchmarks-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.ExtractAllBenchmarks-20260203-084535</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method              </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Gen0</th><th>Allocated</th>
</tr>
</thead><tbody><tr><td>ExtractAll</td><td>6.280 &mu;s</td><td>1.4762 &mu;s</td><td>0.0809 &mu;s</td><td>0.7172</td><td>11.05 KB</td>
</tr><tr><td>ExtractAllClassic</td><td>13.620 &mu;s</td><td>3.7008 &mu;s</td><td>0.2029 &mu;s</td><td>1.7090</td><td>26.31 KB</td>
</tr><tr><td>ExtractAllCached</td><td>4.922 &mu;s</td><td>0.0016 &mu;s</td><td>0.0001 &mu;s</td><td>0.6027</td><td>9.34 KB</td>
</tr><tr><td>ExtractAllAcrossRunsCached</td><td>4.638 &mu;s</td><td>0.8236 &mu;s</td><td>0.0451 &mu;s</td><td>0.5493</td><td>8.48 KB</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.ExtractOneBenchmarks-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                     | Mean      | Error     | StdDev    | Gen0   | Allocated |
|--------------------------- |----------:|----------:|----------:|-------:|----------:|
| ExtractOne                 |  6.245 μs | 1.6429 μs | 0.0901 μs | 0.7095 |  10.97 KB |
| ExtractOneClassic          | 13.661 μs | 1.5088 μs | 0.0827 μs | 1.7090 |  26.22 KB |
| ExtractOneCached           |  4.864 μs | 1.8295 μs | 0.1003 μs | 0.5951 |   9.17 KB |
| ExtractOneAcrossRunsCached |  4.327 μs | 0.8470 μs | 0.0464 μs | 0.5341 |   8.28 KB |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.ExtractOneBenchmarks-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Gen0,Allocated
ExtractOne,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,6.245 μs,1.6429 μs,0.0901 μs,0.7095,10.97 KB
ExtractOneClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,13.661 μs,1.5088 μs,0.0827 μs,1.7090,26.22 KB
ExtractOneCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,4.864 μs,1.8295 μs,0.1003 μs,0.5951,9.17 KB
ExtractOneAcrossRunsCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,4.327 μs,0.8470 μs,0.0464 μs,0.5341,8.28 KB
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.ExtractOneBenchmarks-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.ExtractOneBenchmarks-20260203-084606</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method              </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Gen0</th><th>Allocated</th>
</tr>
</thead><tbody><tr><td>ExtractOne</td><td>6.245 &mu;s</td><td>1.6429 &mu;s</td><td>0.0901 &mu;s</td><td>0.7095</td><td>10.97 KB</td>
</tr><tr><td>ExtractOneClassic</td><td>13.661 &mu;s</td><td>1.5088 &mu;s</td><td>0.0827 &mu;s</td><td>1.7090</td><td>26.22 KB</td>
</tr><tr><td>ExtractOneCached</td><td>4.864 &mu;s</td><td>1.8295 &mu;s</td><td>0.1003 &mu;s</td><td>0.5951</td><td>9.17 KB</td>
</tr><tr><td>ExtractOneAcrossRunsCached</td><td>4.327 &mu;s</td><td>0.8470 &mu;s</td><td>0.0464 &mu;s</td><td>0.5341</td><td>8.28 KB</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinLarge-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error      | StdDev    | Ratio | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|------------------ |-----------:|-----------:|----------:|------:|-----------:|-----------:|------------:|------------:|
| NaiveDp           | 170.446 ms | 19.8528 ms | 1.0882 ms |  1.00 | 17333.3333 | 12666.6667 | 275312720 B |       1.000 |
| FuzzySharpClassic | 111.699 ms | 10.6421 ms | 0.5833 ms |  0.66 |          - |          - |   1545632 B |       0.006 |
| Fastenshtein      |  92.259 ms |  6.2599 ms | 0.3431 ms |  0.54 |          - |          - |     33928 B |       0.000 |
| Quickenshtein     |   8.771 ms |  3.0122 ms | 0.1651 ms |  0.05 |          - |          - |           - |       0.000 |
| FuzzySharp        |   3.230 ms |  0.4890 ms | 0.0268 ms |  0.02 |          - |          - |      3520 B |       0.000 |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinLarge-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Ratio,Gen0,Gen1,Allocated,Alloc Ratio
NaiveDp,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,170.446 ms,19.8528 ms,1.0882 ms,1.00,17333.3333,12666.6667,275312720 B,1.000
FuzzySharpClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,111.699 ms,10.6421 ms,0.5833 ms,0.66,0.0000,0.0000,1545632 B,0.006
Fastenshtein,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,92.259 ms,6.2599 ms,0.3431 ms,0.54,0.0000,0.0000,33928 B,0.000
Quickenshtein,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,8.771 ms,3.0122 ms,0.1651 ms,0.05,0.0000,0.0000,0 B,0.000
FuzzySharp,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,3.230 ms,0.4890 ms,0.0268 ms,0.02,0.0000,0.0000,3520 B,0.000
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinLarge-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinLarge-20260203-085141</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method     </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Ratio</th><th>Gen0</th><th>Gen1</th><th>Allocated</th><th>Alloc Ratio</th>
</tr>
</thead><tbody><tr><td>NaiveDp</td><td>170.446 ms</td><td>19.8528 ms</td><td>1.0882 ms</td><td>1.00</td><td>17333.3333</td><td>12666.6667</td><td>275312720 B</td><td>1.000</td>
</tr><tr><td>FuzzySharpClassic</td><td>111.699 ms</td><td>10.6421 ms</td><td>0.5833 ms</td><td>0.66</td><td>-</td><td>-</td><td>1545632 B</td><td>0.006</td>
</tr><tr><td>Fastenshtein</td><td>92.259 ms</td><td>6.2599 ms</td><td>0.3431 ms</td><td>0.54</td><td>-</td><td>-</td><td>33928 B</td><td>0.000</td>
</tr><tr><td>Quickenshtein</td><td>8.771 ms</td><td>3.0122 ms</td><td>0.1651 ms</td><td>0.05</td><td>-</td><td>-</td><td>-</td><td>0.000</td>
</tr><tr><td>FuzzySharp</td><td>3.230 ms</td><td>0.4890 ms</td><td>0.0268 ms</td><td>0.02</td><td>-</td><td>-</td><td>3520 B</td><td>0.000</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinNormal-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error       | StdDev    | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
|------------------ |-----------:|------------:|----------:|------:|--------:|---------:|--------:|-----------:|------------:|
| NaiveDp           | 6,235.1 μs | 1,902.54 μs | 104.28 μs |  1.00 |    0.02 | 632.8125 | 78.1250 | 10012112 B |       1.000 |
| FuzzySharpClassic | 3,816.9 μs |   106.03 μs |   5.81 μs |  0.61 |    0.01 |  15.6250 |       - |   300048 B |       0.030 |
| Fastenshtein      | 3,024.2 μs |   204.81 μs |  11.23 μs |  0.49 |    0.01 |        - |       - |     7064 B |       0.001 |
| Quickenshtein     | 1,289.3 μs |   102.88 μs |   5.64 μs |  0.21 |    0.00 |        - |       - |          - |       0.000 |
| FuzzySharp        |   234.6 μs |    21.26 μs |   1.17 μs |  0.04 |    0.00 |        - |       - |     3520 B |       0.000 |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinNormal-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Ratio,RatioSD,Gen0,Gen1,Allocated,Alloc Ratio
NaiveDp,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,"6,235.1 μs","1,902.54 μs",104.28 μs,1.00,0.02,632.8125,78.1250,10012112 B,1.000
FuzzySharpClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,"3,816.9 μs",106.03 μs,5.81 μs,0.61,0.01,15.6250,0.0000,300048 B,0.030
Fastenshtein,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,"3,024.2 μs",204.81 μs,11.23 μs,0.49,0.01,0.0000,0.0000,7064 B,0.001
Quickenshtein,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,"1,289.3 μs",102.88 μs,5.64 μs,0.21,0.00,0.0000,0.0000,0 B,0.000
FuzzySharp,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,234.6 μs,21.26 μs,1.17 μs,0.04,0.00,0.0000,0.0000,3520 B,0.000
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinNormal-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinNormal-20260203-085211</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method     </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Ratio</th><th>RatioSD</th><th>Gen0</th><th>Gen1</th><th>Allocated</th><th>Alloc Ratio</th>
</tr>
</thead><tbody><tr><td>NaiveDp</td><td>6,235.1 &mu;s</td><td>1,902.54 &mu;s</td><td>104.28 &mu;s</td><td>1.00</td><td>0.02</td><td>632.8125</td><td>78.1250</td><td>10012112 B</td><td>1.000</td>
</tr><tr><td>FuzzySharpClassic</td><td>3,816.9 &mu;s</td><td>106.03 &mu;s</td><td>5.81 &mu;s</td><td>0.61</td><td>0.01</td><td>15.6250</td><td>-</td><td>300048 B</td><td>0.030</td>
</tr><tr><td>Fastenshtein</td><td>3,024.2 &mu;s</td><td>204.81 &mu;s</td><td>11.23 &mu;s</td><td>0.49</td><td>0.01</td><td>-</td><td>-</td><td>7064 B</td><td>0.001</td>
</tr><tr><td>Quickenshtein</td><td>1,289.3 &mu;s</td><td>102.88 &mu;s</td><td>5.64 &mu;s</td><td>0.21</td><td>0.00</td><td>-</td><td>-</td><td>-</td><td>0.000</td>
</tr><tr><td>FuzzySharp</td><td>234.6 &mu;s</td><td>21.26 &mu;s</td><td>1.17 &mu;s</td><td>0.04</td><td>0.00</td><td>-</td><td>-</td><td>3520 B</td><td>0.000</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinSmall-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean        | Error      | StdDev    | Ratio | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------------ |------------:|-----------:|----------:|------:|---------:|-------:|----------:|------------:|
| NaiveDp           | 1,292.34 μs | 237.510 μs | 13.019 μs |  1.00 | 148.4375 | 3.9063 | 2335168 B |       1.000 |
| FuzzySharpClassic |   792.32 μs |  35.923 μs |  1.969 μs |  0.61 |   8.7891 |      - |  149792 B |       0.064 |
| Fastenshtein      |   594.17 μs |  28.972 μs |  1.588 μs |  0.46 |        - |      - |    3728 B |       0.002 |
| Quickenshtein     |   412.39 μs |  12.342 μs |  0.677 μs |  0.32 |        - |      - |         - |       0.000 |
| FuzzySharp        |    40.24 μs |   2.757 μs |  0.151 μs |  0.03 |   0.1831 |      - |    3520 B |       0.002 |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinSmall-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Ratio,Gen0,Gen1,Allocated,Alloc Ratio
NaiveDp,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,"1,292.34 μs",237.510 μs,13.019 μs,1.00,148.4375,3.9063,2335168 B,1.000
FuzzySharpClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,792.32 μs,35.923 μs,1.969 μs,0.61,8.7891,0.0000,149792 B,0.064
Fastenshtein,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,594.17 μs,28.972 μs,1.588 μs,0.46,0.0000,0.0000,3728 B,0.002
Quickenshtein,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,412.39 μs,12.342 μs,0.677 μs,0.32,0.0000,0.0000,0 B,0.000
FuzzySharp,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,40.24 μs,2.757 μs,0.151 μs,0.03,0.1831,0.0000,3520 B,0.002
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinSmall-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinSmall-20260203-085251</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method     </th><th>Mean </th><th>Error</th><th>StdDev</th><th>Ratio</th><th>Gen0</th><th>Gen1</th><th>Allocated</th><th>Alloc Ratio</th>
</tr>
</thead><tbody><tr><td>NaiveDp</td><td>1,292.34 &mu;s</td><td>237.510 &mu;s</td><td>13.019 &mu;s</td><td>1.00</td><td>148.4375</td><td>3.9063</td><td>2335168 B</td><td>1.000</td>
</tr><tr><td>FuzzySharpClassic</td><td>792.32 &mu;s</td><td>35.923 &mu;s</td><td>1.969 &mu;s</td><td>0.61</td><td>8.7891</td><td>-</td><td>149792 B</td><td>0.064</td>
</tr><tr><td>Fastenshtein</td><td>594.17 &mu;s</td><td>28.972 &mu;s</td><td>1.588 &mu;s</td><td>0.46</td><td>-</td><td>-</td><td>3728 B</td><td>0.002</td>
</tr><tr><td>Quickenshtein</td><td>412.39 &mu;s</td><td>12.342 &mu;s</td><td>0.677 &mu;s</td><td>0.32</td><td>-</td><td>-</td><td>-</td><td>0.000</td>
</tr><tr><td>FuzzySharp</td><td>40.24 &mu;s</td><td>2.757 &mu;s</td><td>0.151 &mu;s</td><td>0.03</td><td>0.1831</td><td>-</td><td>3520 B</td><td>0.002</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistanceBenchmarks-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                    | Mean      | Error     | StdDev   | Gen0   | Allocated |
|-------------------------- |----------:|----------:|---------:|-------:|----------:|
| FuzzySharpDistance        | 180.47 ns | 32.622 ns | 1.788 ns | 0.0091 |     144 B |
| FuzzySharpClassicDistance | 526.36 ns | 55.133 ns | 3.022 ns | 0.0200 |     320 B |
| FastenshteinDistance      | 623.55 ns | 99.975 ns | 5.480 ns |      - |         - |
| QuickenshteinDistance     | 469.50 ns | 47.252 ns | 2.590 ns |      - |         - |
| FuzzySharpDistanceFrom    |  74.69 ns |  6.553 ns | 0.359 ns |      - |         - |
| FastenshteinDistanceFrom  | 523.82 ns | 30.887 ns | 1.693 ns |      - |         - |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistanceBenchmarks-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Gen0,Allocated
FuzzySharpDistance,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,180.47 ns,32.622 ns,1.788 ns,0.0091,144 B
FuzzySharpClassicDistance,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,526.36 ns,55.133 ns,3.022 ns,0.0200,320 B
FastenshteinDistance,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,623.55 ns,99.975 ns,5.480 ns,0.0000,0 B
QuickenshteinDistance,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,469.50 ns,47.252 ns,2.590 ns,0.0000,0 B
FuzzySharpDistanceFrom,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,74.69 ns,6.553 ns,0.359 ns,0.0000,0 B
FastenshteinDistanceFrom,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,523.82 ns,30.887 ns,1.693 ns,0.0000,0 B
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistanceBenchmarks-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.LevenshteinDistanceBenchmarks-20260203-084636</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method             </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Gen0</th><th>Allocated</th>
</tr>
</thead><tbody><tr><td>FuzzySharpDistance</td><td>180.47 ns</td><td>32.622 ns</td><td>1.788 ns</td><td>0.0091</td><td>144 B</td>
</tr><tr><td>FuzzySharpClassicDistance</td><td>526.36 ns</td><td>55.133 ns</td><td>3.022 ns</td><td>0.0200</td><td>320 B</td>
</tr><tr><td>FastenshteinDistance</td><td>623.55 ns</td><td>99.975 ns</td><td>5.480 ns</td><td>-</td><td>-</td>
</tr><tr><td>QuickenshteinDistance</td><td>469.50 ns</td><td>47.252 ns</td><td>2.590 ns</td><td>-</td><td>-</td>
</tr><tr><td>FuzzySharpDistanceFrom</td><td>74.69 ns</td><td>6.553 ns</td><td>0.359 ns</td><td>-</td><td>-</td>
</tr><tr><td>FastenshteinDistanceFrom</td><td>523.82 ns</td><td>30.887 ns</td><td>1.693 ns</td><td>-</td><td>-</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.PartialRatioBenchmarks-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method              | Mean     | Error     | StdDev   | Gen0   | Allocated |
|-------------------- |---------:|----------:|---------:|-------:|----------:|
| PartialRatio        | 296.6 ns |   8.83 ns |  0.48 ns | 0.0091 |     144 B |
| PartialRatioClassic | 642.8 ns | 457.17 ns | 25.06 ns | 0.2146 |    3368 B |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.PartialRatioBenchmarks-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Gen0,Allocated
PartialRatio,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,296.6 ns,8.83 ns,0.48 ns,0.0091,144 B
PartialRatioClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,642.8 ns,457.17 ns,25.06 ns,0.2146,3368 B
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.PartialRatioBenchmarks-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.PartialRatioBenchmarks-20260203-084718</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method       </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Gen0</th><th>Allocated</th>
</tr>
</thead><tbody><tr><td>PartialRatio</td><td>296.6 ns</td><td>8.83 ns</td><td>0.48 ns</td><td>0.0091</td><td>144 B</td>
</tr><tr><td>PartialRatioClassic</td><td>642.8 ns</td><td>457.17 ns</td><td>25.06 ns</td><td>0.2146</td><td>3368 B</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.RatioBenchmarks-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                | Mean      | Error     | StdDev   | Gen0   | Allocated |
|---------------------- |----------:|----------:|---------:|-------:|----------:|
| Ratio                 |  98.92 ns | 55.384 ns | 3.036 ns | 0.0092 |     144 B |
| RatioClassic          | 162.62 ns | 38.227 ns | 2.095 ns | 0.0203 |     320 B |
| RatioCached           | 130.74 ns | 62.343 ns | 3.417 ns | 0.0153 |     240 B |
| RatioAcrossRunsCached |  39.29 ns |  3.065 ns | 0.168 ns |      - |         - |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.RatioBenchmarks-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Gen0,Allocated
Ratio,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,98.92 ns,55.384 ns,3.036 ns,0.0092,144 B
RatioClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,162.62 ns,38.227 ns,2.095 ns,0.0203,320 B
RatioCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,130.74 ns,62.343 ns,3.417 ns,0.0153,240 B
RatioAcrossRunsCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,39.29 ns,3.065 ns,0.168 ns,0.0000,0 B
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.RatioBenchmarks-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.RatioBenchmarks-20260203-084844</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method         </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Gen0</th><th>Allocated</th>
</tr>
</thead><tbody><tr><td>Ratio</td><td>98.92 ns</td><td>55.384 ns</td><td>3.036 ns</td><td>0.0092</td><td>144 B</td>
</tr><tr><td>RatioClassic</td><td>162.62 ns</td><td>38.227 ns</td><td>2.095 ns</td><td>0.0203</td><td>320 B</td>
</tr><tr><td>RatioCached</td><td>130.74 ns</td><td>62.343 ns</td><td>3.417 ns</td><td>0.0153</td><td>240 B</td>
</tr><tr><td>RatioAcrossRunsCached</td><td>39.29 ns</td><td>3.065 ns</td><td>0.168 ns</td><td>-</td><td>-</td>
</tr></tbody></table>
</body>
</html>
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.WeightedRatioBenchmarks-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                        | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------ |---------:|----------:|----------:|-------:|-------:|----------:|
| WeightedRatio                 | 3.078 μs | 0.2422 μs | 0.0133 μs | 0.3014 |      - |   4.67 KB |
| WeightedRatioClassic          | 6.720 μs | 4.2698 μs | 0.2340 μs | 0.7477 |      - |  11.53 KB |
| WeightedRatioCached           | 2.613 μs | 0.2979 μs | 0.0163 μs | 0.5836 | 0.0114 |   8.99 KB |
| WeightedRatioAcrossRunsCached | 2.143 μs | 0.4156 μs | 0.0228 μs | 0.2213 |      - |   3.44 KB |
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.WeightedRatioBenchmarks-report.csv
```
Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Gen0,Gen1,Allocated
WeightedRatio,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,3.078 μs,0.2422 μs,0.0133 μs,0.3014,0.0000,4.67 KB
WeightedRatioClassic,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,6.720 μs,4.2698 μs,0.2340 μs,0.7477,0.0000,11.53 KB
WeightedRatioCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,2.613 μs,0.2979 μs,0.0163 μs,0.5836,0.0114,8.99 KB
WeightedRatioAcrossRunsCached,ShortRun,False,Default,Default,Default,Default,Default,Default,111111111111111111111111,Empty,RyuJit,Default,X64,8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c,.NET 10.0,False,True,False,True,Default,Default,False,False,False,Default,Default,Default,Default,Default,Default,Default,Default,3,Default,1,Default,Default,Default,Default,Default,Default,16,3,2.143 μs,0.4156 μs,0.0228 μs,0.2213,0.0000,3.44 KB
```

BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.WeightedRatioBenchmarks-report.html
```
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8' />
<title>Raffinert.FuzzySharp.Benchmarks.WeightedRatioBenchmarks-20260203-085111</title>

<style type="text/css">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; text-align: right; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>
</head>
<body>
<pre><code>
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7623)
12th Gen Intel Core i9-12900KF 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
</code></pre>
<pre><code>Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
</code></pre>

<table>
<thead><tr><th>Method                 </th><th>Mean</th><th>Error</th><th>StdDev</th><th>Gen0</th><th>Gen1</th><th>Allocated</th>
</tr>
</thead><tbody><tr><td>WeightedRatio</td><td>3.078 &mu;s</td><td>0.2422 &mu;s</td><td>0.0133 &mu;s</td><td>0.3014</td><td>-</td><td>4.67 KB</td>
</tr><tr><td>WeightedRatioClassic</td><td>6.720 &mu;s</td><td>4.2698 &mu;s</td><td>0.2340 &mu;s</td><td>0.7477</td><td>-</td><td>11.53 KB</td>
</tr><tr><td>WeightedRatioCached</td><td>2.613 &mu;s</td><td>0.2979 &mu;s</td><td>0.0163 &mu;s</td><td>0.5836</td><td>0.0114</td><td>8.99 KB</td>
</tr><tr><td>WeightedRatioAcrossRunsCached</td><td>2.143 &mu;s</td><td>0.4156 &mu;s</td><td>0.0228 &mu;s</td><td>0.2213</td><td>-</td><td>3.44 KB</td>
</tr></tbody></table>
</body>
</html>
```

FuzzySharp.Benchmarks/LevenshteinDistance/LevenshteinLarge.cs
```
﻿using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Benchmarks.Utils;
using FastLevenshtein = Fastenshtein.Levenshtein;
using FuzzLevenshtein = Raffinert.FuzzySharp.Levenshtein;
using FuzzLevenshteinClassic = FuzzySharp.Levenshtein;
using QuickLevenshtein = Quickenshtein.Levenshtein;

namespace Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance;

[MemoryDiagnoser]
public class LevenshteinLarge
{
    private string[] _words;

    [GlobalSetup]
    public void SetUp()
    {
        _words = RandomWords.Create(20, 1024);
    }

    [Benchmark(Baseline = true)]
    public void NaiveDp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                LevenshteinBaseline.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharpClassic()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                FuzzLevenshteinClassic.EditDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void Fastenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var levenshtein = new FastLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                levenshtein.DistanceFrom(_words[j]);
            }
        }
    }

    [Benchmark]
    public void Quickenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                QuickLevenshtein.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            using var lev = new FuzzLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                lev.DistanceFrom(_words[j]);
            }
        }
    }
}
```

FuzzySharp.Benchmarks/LevenshteinDistance/LevenshteinNormal.cs
```
﻿using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Benchmarks.Utils;
using FastLevenshtein = Fastenshtein.Levenshtein;
using FuzzLevenshtein = Raffinert.FuzzySharp.Levenshtein;
using FuzzLevenshteinClassic = FuzzySharp.Levenshtein;
using QuickLevenshtein = Quickenshtein.Levenshtein;


namespace Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance;

[MemoryDiagnoser]
public class LevenshteinNormal
{
    private string[] _words;

    [GlobalSetup]
    public void SetUp()
    {
        _words = RandomWords.Create(20, 128);
    }

    [Benchmark(Baseline = true)]
    public void NaiveDp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                LevenshteinBaseline.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharpClassic()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                FuzzLevenshteinClassic.EditDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void Fastenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var levenshtein = new FastLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                levenshtein.DistanceFrom(_words[j]);
            }
        }
    }

    [Benchmark]
    public void Quickenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                QuickLevenshtein.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            using var lev = new FuzzLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                lev.DistanceFrom(_words[j]);
            }
        }
    }
}
```

FuzzySharp.Benchmarks/LevenshteinDistance/LevenshteinSmall.cs
```
﻿using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Benchmarks.Utils;
using FastLevenshtein = Fastenshtein.Levenshtein;
using FuzzLevenshtein = Raffinert.FuzzySharp.Levenshtein;
using FuzzLevenshteinClassic = FuzzySharp.Levenshtein;
using QuickLevenshtein = Quickenshtein.Levenshtein;

namespace Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance;

[MemoryDiagnoser]
public class LevenshteinSmall
{
    private string[] _words;

    [GlobalSetup]
    public void SetUp()
    {
        _words = RandomWords.Create(20, 64);
    }

    [Benchmark(Baseline = true)]
    public void NaiveDp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                LevenshteinBaseline.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharpClassic()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                FuzzLevenshteinClassic.EditDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void Fastenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var levenshtein = new FastLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                levenshtein.DistanceFrom(_words[j]);
            }
        }
    }

    [Benchmark]
    public void Quickenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                QuickLevenshtein.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            using var lev = new FuzzLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                lev.DistanceFrom(_words[j]);
            }
        }
    }
}
```

FuzzySharp.Benchmarks/Utils/LevenshteinBaseline.cs
```
namespace Raffinert.FuzzySharp.Benchmarks.Utils;

public static class LevenshteinBaseline
{
    public static int GetDistance(string source, string target)
    {
        var costMatrix = Enumerable
            .Range(0, source.Length + 1)
            .Select(line => new int[target.Length + 1])
            .ToArray();

        for (var rowIndex = 1; rowIndex <= source.Length; rowIndex++)
        {
            costMatrix[rowIndex][0] = rowIndex;
        }

        for (var columnIndex = 1; columnIndex <= target.Length; columnIndex++)
        {
            costMatrix[0][columnIndex] = columnIndex;
        }

        for (var rowIndex = 1; rowIndex <= source.Length; rowIndex++)
        {
            for (var columnIndex = 1; columnIndex <= target.Length; columnIndex++)
            {
                var insertion = costMatrix[rowIndex][columnIndex - 1] + 1;
                var deletion = costMatrix[rowIndex - 1][columnIndex] + 1;
                var substitution = costMatrix[rowIndex - 1][columnIndex - 1] + (source[rowIndex - 1] == target[columnIndex - 1] ? 0 : 1);

                costMatrix[rowIndex][columnIndex] = Math.Min(Math.Min(insertion, deletion), substitution);
            }
        }

        return costMatrix[source.Length][target.Length];
    }
}
```

FuzzySharp.Benchmarks/Utils/RandomWords.cs
```
﻿namespace Raffinert.FuzzySharp.Benchmarks.Utils;

// original https://github.com/DanHarltey/Fastenshtein/blob/master/benchmarks/Fastenshtein.Benchmarking/RandomWords.cs
public static class RandomWords
{
    private static readonly char[] Letters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

    public static string[] Create(int count, int maxWordSize)
    {
        var words = new string[count];

        // using a const seed to make sure runs of the performance tests are consistent.
        var random = new Random(37);

        for (var i = 0; i < words.Length; i++)
        {
            var wordSize = random.Next(3, maxWordSize);

            words[i] = string.Create(wordSize, random, static (word, r) =>
            {
                for (var j = 0; j < word.Length; j++)
                {
                    var index = r.Next(0, Letters.Length);
                    word[j] = Letters[index];
                }
            });
        }

        return words;
    }
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
using System.Runtime.CompilerServices;
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

        var words = input.Split(EmptyArray<char>(), StringSplitOptions.RemoveEmptyEntries);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T[] EmptyArray<T>()
    {
        return [];
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

FuzzySharp/Extractor/ResultExtractor.cs
```
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp.Extractor;

public static class ResultExtractor
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

FuzzySharp/Utils/NumericsPolyfill.cs
```
﻿using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

internal static class NumericsPolyfill
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

FuzzySharp.Test/EvaluationTests/EvaluationTests.cs
```
﻿using NUnit.Framework;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp.Test.EvaluationTests;

[TestFixture]
public class EvaluationTests
{
    [Test]
    public void Evaluate()
    {
        var a1 = Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
        var a2 = Fuzz.Ratio("mysmilarstring", "mysimilarstring");

        var b1 = Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");

        var c1 = Fuzz.TokenSortRatio("order words out of", "  words out of order");
        var c2 = Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");

        var d1 = Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
        var d2 = Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");

        var e1 = Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");

        var f1 = Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
        var f2 = Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");

        var f3 = Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
        var f4 = Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");

        var g1 = Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
        var g2 = Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);



        var h1 = Process.ExtractOne("cowboys", ["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"]);
        var h11 = Process.Cached.ExtractOne("cowboys", ["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"]);
        var h2 = string.Join(", ", Process.ExtractTop("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], limit: 3));
        var h21 = string.Join(", ", Process.Cached.ExtractTop("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], limit: 3));
        var h3 = string.Join(", ", Process.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));
        var h31 = string.Join(", ", Process.Cached.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));
        var h4 = string.Join(", ", Process.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], cutoff: 40));
        var h41 = string.Join(", ", Process.Cached.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], cutoff: 40));
        var h5 = string.Join(", ", Process.ExtractSorted("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));
        var h51 = string.Join(", ", Process.Cached.ExtractSorted("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));

        var i1 = Process.ExtractOne("cowboys", ["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"], s => s, ScorerCache.Get<DefaultRatioScorer>());
        var i11 = Process.Cached.ExtractOne("cowboys",["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"], s => s, new CachedDefaultRatioScorer("cowboys"));

        string[][] events =
        [
            ["chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm"],
            ["new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm"],
            ["atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm"]
        ];
        var query = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };

        var best = Process.ExtractOne(query, events, strings => strings[0]);
        var best1 = Process.Cached.ExtractOne(query, events, strings => strings[0]);

        var ratio = ScorerCache.Get<DefaultRatioScorer>();
        var partial = ScorerCache.Get<PartialRatioScorer>();
        var tokenSet = ScorerCache.Get<TokenSetScorer>();
        var partialTokenSet = ScorerCache.Get<PartialTokenSetScorer>();
        var tokenSort = ScorerCache.Get<TokenSortScorer>();
        var partialTokenSort = ScorerCache.Get<PartialTokenSortScorer>();
        var tokenAbbreviation = ScorerCache.Get<TokenAbbreviationScorer>();
        var partialTokenAbbreviation = ScorerCache.Get<PartialTokenAbbreviationScorer>();
        var weighted = ScorerCache.Get<WeightedRatioScorer>();
    }

    [Test]
    public void TokenInitialismScorer_WhenGivenStringWithTrailingSpaces_DoesNotBreak()
    {
        // arrange
        var longer = "lusiki plaza share block ";
        var shorter = "jmft";

        // act
        var ratio = Fuzz.TokenInitialismRatio(shorter, longer);

        // assert
        Assert.IsTrue(ratio >= 0);
    }
}
```

FuzzySharp.Test/FuzzyTests/ProcessTests.cs
```
﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

[TestFixture]
public class ProcessTests
{
    private string   _s1;
    private string   _s1A;
    private string   _s2;
    private string   _s3;
    private string   _s4;
    private string   _s5;
    private string   _s6;
    private string[] _cirqueStrings;
    private string[] _baseballStrings;

    [SetUp]
    public void Setup()
    {
        _s1  = "new york mets";
        _s1A = "new york mets";
        _s2  = "new YORK mets";
        _s3  = "the wonderful new york mets";
        _s4  = "new york mets vs atlanta braves";
        _s5  = "atlanta braves vs new york mets";
        _s6  = "new york mets - atlanta braves";
        _cirqueStrings = new[]
        {
            "cirque du soleil - zarkana - las vegas",
            "cirque du soleil ",
            "cirque du soleil las vegas",
            "zarkana las vegas",
            "las vegas cirque du soleil at the bellagio",
            "zarakana - cirque du soleil - bellagio"
        };

        _baseballStrings = new[]
        {
            "new york mets vs chicago cubs",
            "chicago cubs vs chicago white sox",
            "philladelphia phillies vs atlanta braves",
            "braves vs mets",
        };
    }

    [Test]
    public void TestGetBestChoice1()
    {
        var query = "new york mets at atlanta braves";
        var best  = Process.ExtractOne(query, _baseballStrings);
        Assert.AreEqual(best.Value, "braves vs mets");

    }

    [Test]
    public void TestGetBestChoice2()
    {
        var query = "philadelphia phillies at atlanta braves";
        var best  = Process.ExtractOne(query, _baseballStrings);
        Assert.AreEqual(best.Value, _baseballStrings[2]);

    }

    [Test]
    public void TestGetBestChoice3()
    {
        var query = "atlanta braves at philadelphia phillies";
        var best  = Process.ExtractOne(query, _baseballStrings);
        Assert.AreEqual(best.Value, _baseballStrings[2]);

    }

    [Test]
    public void TestGetBestChoice4()
    {
        var query = "chicago cubs vs new york mets";
        var best  = Process.ExtractOne(query, _baseballStrings);
        Assert.AreEqual(best.Value, _baseballStrings[0]);

    }

    [Test]
    public void TestWithProcessor()
    {
        var events = new[]
        {
            new[] { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" },
            new[] { "new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm" },
            new[] { "atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm" },
        };
        var query = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };

        var best = Process.ExtractOne(query, events, strings => strings[0]);
        Assert.AreEqual(best.Value, events[0]);
    }

    [Test]
    public void TestWithScorer()
    {
        var choices = new[]
        {
            "new york mets vs chicago cubs",
            "chicago cubs at new york mets",
            "atlanta braves vs pittsbugh pirates",
            "new york yankees vs boston red sox"
        };

        var choicesDict = new Dictionary<int, string>
        {
            [1] = "new york mets vs chicago cubs",
            [2] = "chicago cubs vs chicago white sox",
            [3] = "philladelphia phillies vs atlanta braves",
            [4] = "braves vs mets"
        };

        // in this hypothetical example we care about ordering, so we use quick ratio
        var query = "new york mets at chicago cubs";

        // first, as an example, the normal way would select the "more
        // 'complete' match of choices[1]"

        var best = Process.ExtractOne(query, choices);
        Assert.AreEqual(best.Value, choices[1]);

        // now, use the custom scorer

        best = Process.ExtractOne(query, choices, null, ScorerCache.Get<DefaultRatioScorer>());
        Assert.AreEqual(best.Value, choices[0]);

        best = Process.ExtractOne(query, choicesDict.Select(k => k.Value));
        Assert.AreEqual(best.Value, choicesDict[1]);

    }

    [Test]
    public void TestWithCutoff()
    {
        var choices = new[]
        {
            "new york mets vs chicago cubs",
            "chicago cubs at new york mets",
            "atlanta braves vs pittsbugh pirates",
            "new york yankees vs boston red sox"
        };

        var query = "los angeles dodgers vs san francisco giants";

        // in this situation, this is an event that does not exist in the list
        // we don't want to randomly match to something, so we use a reasonable cutoff

        var best = Process.ExtractSorted(query, choices, cutoff: 50);
        Assert.IsTrue(!best.Any());
        // .assertIsNone(best) // unittest.TestCase did not have assertIsNone until Python 2.7

        // however if we had no cutoff, something would get returned

        // best = Process.ExtractOne(query, choices)
        // .assertIsNotNone(best)

    }

    [Test]
    public void TestWithCutoff2()
    {
        var choices = new[]
        {
            "new york mets vs chicago cubs",
            "chicago cubs at new york mets",
            "atlanta braves vs pittsbugh pirates",
            "new york yankees vs boston red sox"
        };

        var query = "new york mets vs chicago cubs";
        // Only find 100-score cases
        var res = Process.ExtractSorted(query, choices, cutoff: 100);
        Assert.IsTrue(res.Any());
        var bestMatch = res.First();
        Assert.IsTrue(bestMatch.Value == choices[0]);

    }

    [Test]
    public void TestEmptyStrings()
    {
        var choices = new[]
        {
            "",
            "new york mets vs chicago cubs",
            "new york yankees vs boston red sox",
            "",
            ""
        };

        var query = "new york mets at chicago cubs";

        var best = Process.ExtractOne(query, choices);
        Assert.AreEqual(best.Value, choices[1]);
    }

//[Test]
//public void  generate_choices() {
//            choices = ['a', 'Bb', 'CcC']
//            for choice in choices {
//                yield choice
//        search = 'aaa'
//        result = [(value, confidence) for value, confidence in
//                  Process.Extract(search, generate_choices())]
//        .assertTrue(len(result) > 0)

//    }

//[Test]
//public void  test_dict_like_Extract() {
//        """We should be able to use a dict-like object for choices, not only a
//        dict, and still get dict-like output.
//        """
//        try {
//            from UserDict import UserDict
//        except ImportError {
//            from collections import UserDict
//        choices = UserDict({ 'aa' { 'bb', 'a1' { None})
//        search = 'aaa'
//        result = Process.Extract(search, choices)
//        .assertTrue(len(result) > 0)
//        for value, confidence, key in result {
//            .assertTrue(value in choices.values())

//    }

//[Test]
//public void  test_dedupe() {
//        """We should be able to use a list-like object for contains_dupes
//        """
//        // Test 1
//        contains_dupes = ['Frodo Baggins', 'Tom Sawyer', 'Bilbo Baggin', 'Samuel L. Jackson', 'F. Baggins', 'Frody Baggins', 'Bilbo Baggins']

//        result = Process.dedupe(contains_dupes)
//        .assertTrue(len(result) < len(contains_dupes))

//        // Test 2
//        contains_dupes = ['Tom', 'Dick', 'Harry']

//// we should end up with the same list since no duplicates are contained in the list (e.g. original list is returned)
//        deduped_list = ['Tom', 'Dick', 'Harry']

//        result = Process.dedupe(contains_dupes)
//        Assert.AreEqual(result, deduped_list)

//    }

//[Test]
//public void  test_simplematch() {
//        basic_string = 'a, b'
//        match_strings = ['a, b']

//        result = Process.ExtractOne(basic_string, match_strings, scorer=fuzz.ratio)
//        part_result = Process.ExtractOne(basic_string, match_strings, scorer=fuzz.partial_ratio)

//        Assert.AreEqual(result, ('a, b', 100))
//        Assert.AreEqual(part_result, ('a, b', 100))

//    }
}
```

FuzzySharp.Test/FuzzyTests/RatioIssuesTests.cs
```
﻿using NUnit.Framework;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;
using System;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

// Original code https://github.com/rapidfuzz/RapidFuzz/blob/main/tests/test_fuzz.py
public class RatioIssuesTests
{
    [Test]
    public void Issue76()
    {
        Assert.That(Fuzz.PartialRatio("physics 2 vid", "study physics physics 2"), Is.EqualTo(82));
        Assert.That(Fuzz.PartialRatio("physics 2 vid", "study physics physics 2 video"), Is.EqualTo(100));
    }

    [Test]
    public void Issue90()
    {
        Assert.That(Fuzz.PartialRatio("ax b", "a b a c b"), Is.EqualTo(86));
    }

    [Test]
    public void Issue138()
    {
        var str1 = new string('a', 65);
        var str2 = "a" + (char)256 + new string('a', 63);
        Assert.That(Fuzz.PartialRatio(str1, str2), Is.EqualTo(99));
    }

    [Test]
    public void PartialRatioAlignment()
    {
        var a = "a certain string".AsSpan();
        var s = "certain".AsSpan();

        var align1 = PartialRatioStrategy<char>.PartialRatioAlignment(s, a);

        Assert.That(align1.Score, Is.EqualTo(100));
        Assert.That(align1.SrcStart, Is.EqualTo(0));
        Assert.That(align1.SrcEnd, Is.EqualTo(s.Length));
        Assert.That(align1.DestStart, Is.EqualTo(2));
        Assert.That(align1.DestEnd, Is.EqualTo(2 + s.Length));

        var align2 = PartialRatioStrategy<char>.PartialRatioAlignment(a, s);
        Assert.That(align2.Score, Is.EqualTo(100));
        Assert.That(align2.SrcStart, Is.EqualTo(2));
        Assert.That(align2.SrcEnd, Is.EqualTo(2 + s.Length));
        Assert.That(align2.DestStart, Is.EqualTo(0));
        Assert.That(align2.DestEnd, Is.EqualTo(s.Length));

        Assert.That(PartialRatioStrategy<char>.PartialRatioAlignment(null, "test".AsSpan()).Score, Is.EqualTo(0));
        Assert.That(PartialRatioStrategy<char>.PartialRatioAlignment("test".AsSpan(), null).Score, Is.EqualTo(0));
        Assert.That(PartialRatioStrategy<char>.PartialRatioAlignment("test".AsSpan(), "tesx".AsSpan(), scoreCutoff: 90).Score, Is.EqualTo(0));
    }

    [Test]
    public void Issue196()
    {
        Assert.That(Fuzz.WeightedRatio("South Korea", "North Korea"), Is.EqualTo(82));
    }

    [Test]
    public void Issue231()
    {
        var str1 = "er merkantilismus förderte handle und verkehr mit teils marktkonformen, teils dirigistischen maßnahmen.";
        var str2 = "ils marktkonformen, teils dirigistischen maßnahmen. an der schwelle zum 19. jahrhundert entstand ein neu";

        var alignment = PartialRatioStrategy<char>.PartialRatioAlignment(str1.AsSpan(), str2.AsSpan());

        Assert.That(alignment, Is.Not.Null);
        Assert.That(alignment.SrcStart, Is.EqualTo(0));
        Assert.That(alignment.SrcEnd, Is.EqualTo(103));
        Assert.That(alignment.DestStart, Is.EqualTo(0));
        Assert.That(alignment.DestEnd, Is.EqualTo(51));
    }
}
```

FuzzySharp.Test/FuzzyTests/RatioTests.cs
```
using NUnit.Framework;
using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

[TestFixture]
public class RatioTests
{
    #region Private Fields
    private string _s1,
        _s1A,
        _s2,
        _s3,
        _s4,
        _s5,
        _s6,
        _s7,
        _s8,
        _s8A,
        _s9,
        _s9A,
        _s10,
        _s10A;

    private string[] _cirqueStrings, _baseballStrings;
    #endregion

    [SetUp]
    public void Setup()
    {
        _s1  = "new york mets";
        _s1A = "new york mets";
        _s2  = "new YORK mets";
        _s3  = "the wonderful new york mets";
        _s4  = "new york mets vs atlanta braves";
        _s5  = "atlanta braves vs new york mets";
        _s6  = "new york mets - atlanta braves";
        _s7  = "new york city mets - atlanta braves";
        // Edge cases
        _s8   = "{";
        _s8A  = "{";
        _s9   = "{a";
        _s9A  = "{a";
        _s10  = "a{";
        _s10A = "{b";
    }

    [Test]
    public void Test_Equal()
    {
        Assert.AreEqual(Fuzz.Ratio(_s1, _s1A), 100);
        Assert.AreEqual(Fuzz.Ratio(_s8, _s8A), 100);
        Assert.AreEqual(Fuzz.Ratio(_s9, _s9A), 100);
    }

    [Test]
    public void Test_Case_Insensitive()
    {
        Assert.AreNotEqual(Fuzz.Ratio(_s1, _s2), 100);
        Assert.AreEqual(Fuzz.Ratio(_s1, _s2, PreprocessMode.Full), 100);
    }

    [Test]
    public void Test_Partial()
    {
        Assert.AreEqual(Fuzz.PartialRatio(_s1, _s3), 100);
    }

    [Test]
    public void TestTokenSortRatio()
    {
        Assert.AreEqual(Fuzz.TokenSortRatio(_s1, _s1A), 100);
    }

    [Test]
    public void TestPartialTokenSortRatio()
    {
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s1, _s1A, PreprocessMode.Full), 100);
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s4, _s5, PreprocessMode.Full), 100);
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s8, _s8A), 100);
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s9, _s9A, PreprocessMode.Full), 100);
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s9, _s9A), 100);

        //var al =  Fuzz1.PartialRatioAlignment("a certain string".AsSpan(), "cetain".AsSpan());
            
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s10, _s10A), 67);
        Assert.AreEqual(Fuzz.PartialTokenSortRatio(_s10, _s10A, PreprocessMode.Full), 0);
    }

    [Test]
    public void TestTokenSetRatio()
    {
        Assert.AreEqual(Fuzz.TokenSetRatio(_s4, _s5, PreprocessMode.Full), 100);
        Assert.AreEqual(Fuzz.TokenSetRatio(_s8, _s8A), 100);
        Assert.AreEqual(Fuzz.TokenSetRatio(_s9, _s9A, PreprocessMode.Full), 100);
        Assert.AreEqual(Fuzz.TokenSetRatio(_s9, _s9A), 100);
        Assert.AreEqual(Fuzz.TokenSetRatio(_s10, _s10A), 50);
    }

    [Test]
    public void TestTokenAbbreviationRatio()
    {
        Assert.AreEqual(Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full), 40);
        Assert.AreEqual(Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full), 67);
    }

    [Test]
    public void TestPartialTokenSetRatio()
    {
        Assert.AreEqual(Fuzz.PartialTokenSetRatio(_s4, _s7), 100);
    }

    [Test]
    public void TestWeightedRatioEqual()
    {
        Assert.AreEqual(Fuzz.WeightedRatio(_s1, _s1A), 100);
    }

    [Test]
    public void TestWeightedRatioCaseInsensitive()
    {
        Assert.AreEqual(Fuzz.WeightedRatio(_s1, _s2, PreprocessMode.Full), 100);
    }

    [Test]
    public void TestWeightedRatioPartialMatch()
    {
        Assert.AreEqual(Fuzz.WeightedRatio(_s1, _s3), 90);
    }

    [Test]
    public void TestWeightedRatioMisorderedMatch()
    {
        Assert.AreEqual(Fuzz.WeightedRatio(_s4, _s5), 95);
    }

    [Test]
    public void TestEmptyStringsScore0()
    {
        Assert.That(Fuzz.Ratio("test_string", ""), Is.EqualTo(0));
        Assert.That(Fuzz.PartialRatio("test_string", ""), Is.EqualTo(0));
        Assert.That(Fuzz.Ratio("", ""), Is.EqualTo(0));
        Assert.That(Fuzz.PartialRatio("", ""), Is.EqualTo(0));
    }

    [Test]
    public void TestIssueSeven()
    {
        _s1 = "HSINCHUANG";
        _s2 = "SINJHUAN";
        _s3 = "LSINJHUANG DISTRIC";
        _s4 = "SINJHUANG DISTRICT";

        Assert.IsTrue(Fuzz.PartialRatio(_s1, _s2) > 75);
        Assert.IsTrue(Fuzz.PartialRatio(_s1, _s3) > 75);
        Assert.IsTrue(Fuzz.PartialRatio(_s1, _s4) > 75);
    }

    [Test]
    public void TestIssueEight()
    {
        // https://github.com/JakeBayer/FuzzySharp/issues/8
        Assert.AreEqual(100, Fuzz.PartialRatio("Partnernummer", "Partne\nrnum\nmerASDFPartnernummerASDF")); // was 85 
        Assert.AreEqual(100, Fuzz.PartialRatio("Partnernummer", "PartnerrrrnummerASDFPartnernummerASDF"));  // was 77

        // https://github.com/xdrop/fuzzywuzzy/issues/39
        Assert.AreEqual(100, Fuzz.PartialRatio("kaution", "kdeffxxxiban:de1110010060046666666datum:16.11.17zeit:01:12uft0000899999tan076601testd.-20-maisonette-z4-jobas-hagkautionauszug")); // was 57

        // https://github.com/seatgeek/fuzzywuzzy/issues/79
        Assert.AreEqual(100, Fuzz.PartialRatio("this is a test", "is this is a not really thing this is a test!")); // was 92 (actually 93)

        // https://github.com/Raffinert/FuzzySharp/issues/2
        Assert.AreEqual(100, Fuzz.PartialRatio("sh", "Growing eshops without a popular platform", PreprocessMode.Full));
        Assert.AreEqual(100, Fuzz.PartialRatio("shop", "Growing eshops without a popular platform", PreprocessMode.Full));
    }

    [Test]
    public void MorePartialRatio()
    {
        Assert.AreEqual(100, Fuzz.PartialRatio("geeks for geeks", "geeks for geeks!"));
        Assert.AreEqual(71, Fuzz.PartialRatio("geeks for geeks", "geeks geeks"));
        Assert.AreEqual(100, Fuzz.TokenSortRatio("geeks for geeks", "for geeks geeks"));
    }

    [Test]
    public void TestPartialRatioUnicodeString()
    {
        _s1 = "\u00C1";
        _s2 = "ABCD";
        var score = Fuzz.PartialRatio(_s1, _s2);
        Assert.AreEqual(0, score);
    }

    [Test]
    public void TestZeroRatio()
    {
        var ratio = Fuzz.PartialTokenSortRatio("abc", "def");

        Assert.True(ratio == 0);
    }

    [Test]
    public void Test03()
    {
        var ratio = Fuzz.PartialTokenSortRatio("new york mets", "atlanta braves vs new york mets");

        Assert.True(ratio == 77);
    }

    [Test]
    public void TestRatioUnicodeString()
    {
        _s1 = "\u00C1";
        _s2 = "ABCD";
        var score = Fuzz.WeightedRatio(_s1, _s2);
        Assert.AreEqual(0, score);

        // Cyrillic.
        _s1   = "\u043f\u0441\u0438\u0445\u043e\u043b\u043e\u0433";
        _s2   = "\u043f\u0441\u0438\u0445\u043e\u0442\u0435\u0440\u0430\u043f\u0435\u0432\u0442";
        score = Fuzz.WeightedRatio(_s1, _s2);
        Assert.AreNotEqual(0, score);

        // Chinese.
        _s1   = "\u6211\u4e86\u89e3\u6570\u5b66";
        _s2   = "\u6211\u5b66\u6570\u5b66";
        score = Fuzz.WeightedRatio(_s1, _s2);
        Assert.AreNotEqual(0, score);
    }
}
```

FuzzySharp.Test/FuzzyTests/RegressionTests.cs
```
﻿using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

[TestFixture]
public class RegressionTests
{

    /// <summary>
    /// Test to ensure that all IRatioScorer implementations handle scoring empty strings & whitespace strings
    /// </summary>
    [Test]
    public void TestScoringEmptyString()
    {
        var scorerType = typeof(IRatioScorer);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var types = assemblies.SelectMany(s =>
        {
            try
            {
                return s.GetTypes();
            }
            catch {}
            return [];
        }).ToList();
        var scorerTypes = types.Where(t => scorerType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass).ToList();
            
        string nullString = null;  //Null doesn't seem to be handled by any scorer
        string emptyString = "";
        string whitespaceString = " ";

        string[] nullOrWhitespaceStrings = [emptyString, whitespaceString];
        MethodInfo getScorerCacheMethodInfo = typeof(ScorerCache).GetMethod("Get");

        foreach (var t in scorerTypes)
        {
            System.Diagnostics.Debug.WriteLine($"Testing {t.Name}");
            MethodInfo m = getScorerCacheMethodInfo.MakeGenericMethod(t);
            IRatioScorer scorer = m.Invoke(this, []) as IRatioScorer;

            foreach(string s in nullOrWhitespaceStrings)
            {
                System.Diagnostics.Debug.WriteLine($"Testing string '{s}'");
                try
                {
                    scorer.Score(s, "TEST");
                }
                catch (InvalidOperationException e)
                {
                    Assert.Fail($"{t.Name}.score failed with empty string as first parameter");
                }
                try
                {
                    scorer.Score("TEST", s);
                } catch (InvalidOperationException e)
                {
                    Assert.Fail($"{t.Name}.score failed with empty string as second parameter");
                }
                try
                {
                    scorer.Score(s, s);
                }
                catch (InvalidOperationException e)
                {
                    Assert.Fail($"{t.Name}.score failed with empty string as both parameters");
                }

            }

        }

    }
        
}
```

FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.BenchmarkAll-report-github.md
```
```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22621.6060/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.102
  [Host]   : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean        | Error        | StdDev      | Gen0   | Gen1   | Allocated |
|------------------------------------- |------------:|-------------:|------------:|-------:|-------:|----------:|
| Ratio                                |    215.9 ns |     74.36 ns |     4.08 ns | 0.0215 |      - |     136 B |
| PartialRatio                         |    535.3 ns |    177.34 ns |     9.72 ns | 0.0210 |      - |     136 B |
| TokenSortRatio                       |    689.3 ns |    206.17 ns |    11.30 ns | 0.1116 |      - |     704 B |
| PartialTokenSortRatio                |  1,396.4 ns |    169.09 ns |     9.27 ns | 0.1106 |      - |     704 B |
| TokenSetRatio                        |    989.6 ns |    516.62 ns |    28.32 ns | 0.3443 |      - |    2160 B |
| PartialTokenSetRatio                 |  1,978.6 ns |  1,130.70 ns |    61.98 ns | 0.3433 |      - |    2160 B |
| WeightedRatio                        |  5,205.4 ns |    638.30 ns |    34.99 ns | 0.7553 |      - |    4744 B |
| TokenInitialismRatio1                |    115.3 ns |     24.10 ns |     1.32 ns | 0.0535 |      - |     336 B |
| TokenInitialismRatio2                |    111.8 ns |     81.31 ns |     4.46 ns | 0.0522 |      - |     328 B |
| TokenInitialismRatio3                |    169.9 ns |    256.42 ns |    14.06 ns | 0.0713 |      - |     448 B |
| PartialTokenInitialismRatio          |    324.6 ns |    133.84 ns |     7.34 ns | 0.0710 |      - |     448 B |
| TokenAbbreviationRatio               |    740.9 ns |    100.13 ns |     5.49 ns | 0.2766 |      - |    1736 B |
| PartialTokenAbbreviationRatio        |    788.4 ns |    308.15 ns |    16.89 ns | 0.2766 |      - |    1736 B |
| RatioClassic                         |    241.3 ns |    243.95 ns |    13.37 ns | 0.0508 |      - |     320 B |
| PartialRatioClassic                  |  1,031.1 ns |    966.09 ns |    52.95 ns | 0.5360 | 0.0019 |    3368 B |
| TokenSortRatioClassic                |  1,447.5 ns |     29.62 ns |     1.62 ns | 0.3166 |      - |    1992 B |
| PartialTokenSortRatioClassic         |  1,610.8 ns |  1,316.67 ns |    72.17 ns | 0.3719 |      - |    2344 B |
| TokenSetRatioClassic                 |  1,973.8 ns |      6.18 ns |     0.34 ns | 0.6523 |      - |    4096 B |
| PartialTokenSetRatioClassic          |  2,295.7 ns |    108.17 ns |     5.93 ns | 0.8888 |      - |    5584 B |
| WeightedRatioClassic                 | 10,215.0 ns |  1,556.91 ns |    85.34 ns | 1.8768 |      - |   11810 B |
| TokenInitialismRatio1Classic         |    517.4 ns |    331.45 ns |    18.17 ns | 0.1440 |      - |     904 B |
| TokenInitialismRatio2Classic         |    422.7 ns |     89.43 ns |     4.90 ns | 0.1173 |      - |     736 B |
| TokenInitialismRatio3Classic         |    984.3 ns |    108.68 ns |     5.96 ns | 0.2460 |      - |    1552 B |
| PartialTokenInitialismRatioClassic   |  1,161.2 ns |     65.47 ns |     3.59 ns | 0.3414 |      - |    2144 B |
| TokenAbbreviationRatioClassic        |  1,280.0 ns |  1,027.15 ns |    56.30 ns | 0.4749 |      - |    2984 B |
| PartialTokenAbbreviationRatioClassic |  1,477.3 ns |     44.82 ns |     2.46 ns | 0.6199 |      - |    3896 B |
| ExtractOne                           | 11,357.7 ns | 25,407.46 ns | 1,392.67 ns | 1.7700 |      - |   11112 B |
| ExtractOneClassic                    | 21,436.8 ns | 12,252.42 ns |   671.60 ns | 4.2725 |      - |   26851 B |
| FuzzySharpClassicDistance            |    816.4 ns |    236.72 ns |    12.98 ns | 0.0505 |      - |     320 B |
| FuzzySharpDistance                   |    326.8 ns |     58.85 ns |     3.23 ns | 0.0215 |      - |     136 B |
| FastenshteinDistance                 |    995.6 ns |  2,066.03 ns |   113.25 ns |      - |      - |         - |
| FuzzySharpDistanceFrom               |    134.4 ns |    160.56 ns |     8.80 ns |      - |      - |         - |
| FastenshteinDistanceFrom             |    787.2 ns |     79.49 ns |     4.36 ns |      - |      - |         - |
| QuickenshteinDistance                |    582.8 ns |    279.24 ns |    15.31 ns |      - |      - |         - |
```

FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinLarge-report-github.md
```
```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-1255U 2.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.301
  [Host]   : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error      | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Allocated   | Alloc Ratio |
|------------------ |-----------:|-----------:|----------:|------:|--------:|-----------:|-----------:|------------:|------------:|
| NaiveDp           | 231.563 ms | 57.5403 ms | 3.1540 ms |  1.00 |    0.02 | 43500.0000 | 34500.0000 | 275312920 B |       1.000 |
| FuzzySharpClassic | 141.820 ms |  4.0905 ms | 0.2242 ms |  0.61 |    0.01 |          - |          - |   1545732 B |       0.006 |
| Fastenshtein      | 123.356 ms | 13.0959 ms | 0.7178 ms |  0.53 |    0.01 |          - |          - |     34028 B |       0.000 |
| Quickenshtein     |  12.918 ms | 12.8046 ms | 0.7019 ms |  0.06 |    0.00 |          - |          - |        12 B |       0.000 |
| FuzzySharp        |   4.970 ms |  0.3311 ms | 0.0181 ms |  0.02 |    0.00 |          - |          - |      3051 B |       0.000 |
```

FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinNormal-report-github.md
```
```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-1255U 2.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.301
  [Host]   : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error       | StdDev    | Ratio | RatioSD | Gen0      | Gen1     | Allocated  | Alloc Ratio |
|------------------ |-----------:|------------:|----------:|------:|--------:|----------:|---------:|-----------:|------------:|
| NaiveDp           | 8,613.1 μs | 4,977.60 μs | 272.84 μs |  1.00 |    0.04 | 1593.7500 | 203.1250 | 10012124 B |       1.000 |
| FuzzySharpClassic | 4,866.5 μs |   866.89 μs |  47.52 μs |  0.57 |    0.02 |   46.8750 |        - |   300051 B |       0.030 |
| Fastenshtein      | 4,076.7 μs | 1,265.24 μs |  69.35 μs |  0.47 |    0.01 |         - |        - |     7070 B |       0.001 |
| Quickenshtein     | 1,330.2 μs |   111.30 μs |   6.10 μs |  0.15 |    0.00 |         - |        - |        2 B |       0.000 |
| FuzzySharp        |   588.2 μs |    83.65 μs |   4.59 μs |  0.07 |    0.00 |         - |        - |     3041 B |       0.000 |
```

FuzzySharp.Benchmarks/BenchmarkDotNet.Artifacts/results/Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance.LevenshteinSmall-report-github.md
```
```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-1255U 2.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.301
  [Host]   : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method            | Mean       | Error     | StdDev   | Ratio | RatioSD | Gen0     | Gen1   | Allocated | Alloc Ratio |
|------------------ |-----------:|----------:|---------:|------:|--------:|---------:|-------:|----------:|------------:|
| NaiveDp           | 1,841.4 μs | 753.15 μs | 41.28 μs |  1.00 |    0.03 | 371.0938 | 9.7656 | 2335169 B |       1.000 |
| FuzzySharpClassic | 1,090.0 μs |  23.48 μs |  1.29 μs |  0.59 |    0.01 |  23.4375 |      - |  149793 B |       0.064 |
| Fastenshtein      |   860.4 μs |  80.93 μs |  4.44 μs |  0.47 |    0.01 |        - |      - |    3728 B |       0.002 |
| Quickenshtein     |   531.9 μs |  52.00 μs |  2.85 μs |  0.29 |    0.01 |        - |      - |       1 B |       0.000 |
| FuzzySharp        |   117.7 μs |  11.88 μs |  0.65 μs |  0.06 |    0.00 |   0.3662 |      - |    3040 B |       0.001 |
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
        double cutoff100 = scoreCutoff.GetValueOrDefault(0.0);

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

        // 2) Full-width windows of length len1
        for (int i = 0; i <= len2 - len1; i++)
        {
            if (!patternMatchVector.ContainsKey(s2[i + len1 - 1])) continue;
            var window = s2[i..(i + len1)];
            double sim = Indel.BlockNormalizedSimilarity(patternMatchVector, s1, window);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = i;
                res.DestEnd = i + len1;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        // 3) Suffixes shorter than len1
        for (int i = len2 - len1 + 1; i < len2; i++)
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