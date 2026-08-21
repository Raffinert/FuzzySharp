using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class TokenSetRatioBenchmarks
{
    private ICachedRatioScorer _cachedTokenSetScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedTokenSetScorer = new CachedTokenSetScorer("fuzzy was a bear");
    }

    [Benchmark]
    public double TokenSetRatio()
    {
        return Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public double TokenSetRatioClassic()
    {
        return Classic.Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public double TokenSetRatioCached()
    {
        return new CachedTokenSetScorer("fuzzy was a bear").Score("fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public double TokenSetRatioAcrossRunsCached()
    {
        return _cachedTokenSetScorer.Score("fuzzy fuzzy fuzzy bear");
    }
}
