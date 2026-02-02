using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class TokenSetRatioBenchmarks
{
    private ICachedRatioScorer _cachedTokenSetScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedTokenSetScorer = new CachedTokenSetScorer("fuzzy was a bear");
    }

    [Benchmark]
    public int TokenSetRatio()
    {
        return Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int TokenSetRatioClassic()
    {
        return Classic.Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int TokenSetRatioCached()
    {
        return new CachedTokenSetScorer("fuzzy was a bear").Score("fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int TokenSetRatioAcrossRunsCached()
    {
        return _cachedTokenSetScorer.Score("fuzzy fuzzy fuzzy bear");
    }
}
