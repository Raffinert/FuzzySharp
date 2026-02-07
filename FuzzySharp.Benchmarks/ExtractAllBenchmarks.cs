using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
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
