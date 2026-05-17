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
    private ProcessPipeline _parallelPipeline;
    private ProcessPipeline _cachedPipeline;
    private ProcessPipeline _parallelCachedPipeline;
    private CachedScorerProcessPipeline _acrossRunsCachedPipeline;
    private CachedScorerProcessPipeline _acrossRunsParallelCachedPipeline;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _extractScorer = new CachedWeightedRatioScorer(Query[0]);
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4
        };

        _parallelPipeline = Process.Configure()
            .Parallel(_parallelOptions)
            .Build();

        _cachedPipeline = Process.Configure()
            .Cached()
            .Build();

        _parallelCachedPipeline = Process.Configure()
            .Cached()
            .Parallel(_parallelOptions)
            .Build();

        _acrossRunsCachedPipeline = Process.Configure()
            .Cached(_extractScorer)
            .Build();

        _acrossRunsParallelCachedPipeline = Process.Configure()
            .Cached(_extractScorer)
            .Parallel(_parallelOptions)
            .Build();
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneBy()
    {
        return Process.ExtractOneBy(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public Classic.Extractor.ExtractedResult<string[]> ExtractOneByClassic()
    {
        return Classic.Process.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneByParallel()
    {
        return _parallelPipeline.ExtractOneBy(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneByCached()
    {
        return _cachedPipeline.ExtractOneBy(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneByParallelCached()
    {
        return _parallelCachedPipeline.ExtractOneBy(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneByAcrossRunsCached()
    {
        return _acrossRunsCachedPipeline.ExtractOneBy(Events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneByAcrossRunsParallelCached()
    {
        return _acrossRunsParallelCachedPipeline.ExtractOneBy(Events, static strings => strings[0]);
    }
}
