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
    private ParallelOptions _parallelOptions = null!;
    private CachedScorerProcessPipeline _acrossRunsCachedPipeline;
    private CachedScorerProcessPipeline _acrossRunsParallelCachedPipeline;
    private ProcessPipeline _parallelPipeline;
    private ProcessPipeline _cachedPipeline;
    private ProcessPipeline _parallelCachedPipeline;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _extractScorer = new CachedWeightedRatioScorer(Query[0]);
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4
        };

        _acrossRunsCachedPipeline = Process.Configure()
            .Cached(_extractScorer)
            .Build();

        _acrossRunsParallelCachedPipeline = Process.Configure()
            .Cached(_extractScorer)
            .Parallel(_parallelOptions)
            .Build();

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
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllBy()
    {
        return Process.ExtractAllBy(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<global::FuzzySharp.Extractor.ExtractedResult<string[]>> ExtractAllByClassic()
    {
        return Classic.Process.ExtractAll(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllByParallel()
    {
        return _parallelPipeline.ExtractAllBy(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllByCached()
    {
        return _cachedPipeline.ExtractAllBy(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllByParallelCached()
    {
        return _parallelCachedPipeline.ExtractAllBy(Query, Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllByAcrossRunsCached()
    {
        return _acrossRunsCachedPipeline.ExtractAllBy( Events, static strings => strings[0]).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string[]>> ExtractAllByAcrossRunsParallelCached()
    {
        return _acrossRunsParallelCachedPipeline.ExtractAllBy(Events, static strings => strings[0]).ToList();
    }
}
