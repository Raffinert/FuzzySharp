using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class TokenSortRatioBenchmarks
{
    private ICachedRatioScorer _cachedTokenSortScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedTokenSortScorer = new CachedTokenSortScorer("order words out of");
    }

    [Benchmark]
    public double TokenSortRatio()
    {
        return Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public double TokenSortRatioClassic()
    {
        return Classic.Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public double TokenSortRatioCached()
    {
        return new CachedTokenSortScorer("order words out of").Score("  words out of order");
    }

    [Benchmark]
    public double TokenSortRatioAcrossRunsCached()
    {
        return _cachedTokenSortScorer.Score("  words out of order");
    }
}
