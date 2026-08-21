using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialTokenSetRatioBenchmarks
{
    private ICachedRatioScorer _cachedPartialTokenSetScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedPartialTokenSetScorer = new CachedPartialTokenSetScorer("fuzzy was a bear");
    }

    [Benchmark]
    public double PartialTokenSetRatio()
    {
        return Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public double PartialTokenSetRatioClassic()
    {
        return Classic.Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public double PartialTokenSetRatioCached()
    {
        return new CachedPartialTokenSetScorer("fuzzy was a bear").Score("fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public double PartialTokenSetRatioAcrossRunsCached()
    {
        return _cachedPartialTokenSetScorer.Score("fuzzy fuzzy fuzzy bear");
    }
}
