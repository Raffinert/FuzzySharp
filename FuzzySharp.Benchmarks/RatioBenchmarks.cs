using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class RatioBenchmarks
{
    private ICachedRatioScorer _cachedRatioScorer = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _cachedRatioScorer = new CachedDefaultRatioScorer("mysmilarstring");
    }

    [Benchmark]
    public double Ratio()
    {
        return Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public double RatioClassic()
    {
        return Classic.Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public double RatioCached()
    {
        using var scorer = new CachedDefaultRatioScorer("mysmilarstring");
        return scorer.Score("myawfullysimilarstirng");
    }

    [Benchmark]
    public double RatioAcrossRunsCached()
    {
        return _cachedRatioScorer.Score("myawfullysimilarstirng");
    }
}
