using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class TokenSortRatioBenchmarks
{
    private ICachedRatioScorer _cachedTokenSortScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedTokenSortScorer = new CachedTokenSortScorer("order words out of");
    }

    [Benchmark]
    public int TokenSortRatio()
    {
        return Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSortRatioClassic()
    {
        return Classic.Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSortRatioCached()
    {
        return new CachedTokenSortScorer("order words out of").Score("  words out of order");
    }

    [Benchmark]
    public int TokenSortRatioAcrossRunsCached()
    {
        return _cachedTokenSortScorer.Score("  words out of order");
    }
}
