using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
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
