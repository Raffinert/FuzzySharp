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
    public int PartialTokenSetRatio()
    {
        return Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatioClassic()
    {
        return Classic.Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatioCached()
    {
        return new CachedPartialTokenSetScorer("fuzzy was a bear").Score("fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatioAcrossRunsCached()
    {
        return _cachedPartialTokenSetScorer.Score("fuzzy fuzzy fuzzy bear");
    }
}
