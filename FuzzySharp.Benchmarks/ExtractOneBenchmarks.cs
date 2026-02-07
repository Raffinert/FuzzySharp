using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class ExtractOneBenchmarks
{
    private static readonly string[][] Events =
    [
        ["chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm"],
        ["new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm"],
        ["atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm"]
    ];

    private static readonly string[] Query = ["new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm"];
    private ICachedRatioScorer _extractScorer = null!;
    private ParallelOptions _parallelOptions = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _extractScorer = new CachedWeightedRatioScorer(Query[0]);
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4
        };
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOne()
    {
        return Process.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public Classic.Extractor.ExtractedResult<string[]> ExtractOneClassic()
    {
        return Classic.Process.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneParallel()
    {
        return Process.Parallel.ExtractOne(Query, Events, static strings => strings[0], parallelOptions: _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneCached()
    {
        return Process.Cached.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneParallelCached()
    {
        return Process.Parallel.Cached.ExtractOne(Query, Events, static strings => strings[0], parallelOptions: _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneAcrossRunsCached()
    {
        return Process.Cached.ExtractOne(Query, Events, static strings => strings[0], _extractScorer);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneAcrossRunsParallelCached()
    {
        return Process.Parallel.Cached.ExtractOne(Query, Events, static strings => strings[0], _extractScorer, parallelOptions: _parallelOptions);
    }
}
