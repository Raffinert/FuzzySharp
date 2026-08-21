using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class WeightedRatioBenchmarks
{
    private ICachedRatioScorer _cachedWeightedScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedWeightedScorer = new CachedWeightedRatioScorer("The quick brown fox jimps ofver the small lazy dog");
    }

    [Benchmark]
    public double WeightedRatio()
    {
        return Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public double WeightedRatioClassic()
    {
        return Classic.Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public double WeightedRatioCached()
    {
        return new CachedWeightedRatioScorer("The quick brown fox jimps ofver the small lazy dog").Score("the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public double WeightedRatioAcrossRunsCached()
    {
        return _cachedWeightedScorer.Score("the quick brown fox jumps over the small lazy dog");
    }
}
