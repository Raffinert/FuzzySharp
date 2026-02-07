using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class ExtractOneLargeChoicesBenchmarks
{
    private static readonly string[] Teams =
    [
        "new york mets", "chicago cubs", "new york yankees", "boston red sox", "atlanta braves", "pittsburgh pirates",
        "los angeles dodgers", "san francisco giants", "st louis cardinals", "philadelphia phillies", "houston astros",
        "texas rangers", "seattle mariners", "san diego padres", "toronto blue jays", "tampa bay rays"
    ];

    private static readonly string[] Venues =
    [
        "CitiField", "Fenway Park", "PNC Park", "Dodger Stadium", "Oracle Park", "Busch Stadium", "Minute Maid Park", "T-Mobile Park"
    ];

    private static readonly string[] Dates =
    [
        "2017-03-19", "2017-03-20", "2017-03-21", "2017-03-22", "2017-03-23", "2017-03-24"
    ];

    private static readonly string[] Times =
    [
        "1pm", "4pm", "7pm", "8pm", "9pm"
    ];

    private ICachedRatioScorer _extractScorer = null!;
    private ParallelOptions _parallelOptions = null!;
    private string[][] _events = null!;
    private string[] _query = null!;

    [Params(64, 256, 1024)]
    public int ChoiceCount { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _query = ["new york mets vs chicago cubs game 7 postseason night matchup", "CitiField", "2017-03-19", "8pm"];
        _events = BuildEvents(ChoiceCount);
        _extractScorer = new CachedWeightedRatioScorer(_query[0]);
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOne()
    {
        return Process.ExtractOne(_query, _events, static strings => strings[0]);
    }

    [Benchmark]
    public Classic.Extractor.ExtractedResult<string[]> ExtractOneClassic()
    {
        return Classic.Process.ExtractOne(_query, _events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneParallel()
    {
        return Process.Parallel.ExtractOne(_query, _events, static strings => strings[0], parallelOptions: _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneCached()
    {
        return Process.Cached.ExtractOne(_query, _events, static strings => strings[0]);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneParallelCached()
    {
        return Process.Parallel.Cached.ExtractOne(_query, _events, static strings => strings[0], parallelOptions: _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneAcrossRunsCached()
    {
        return Process.Cached.ExtractOne(_query, _events, static strings => strings[0], _extractScorer);
    }

    [Benchmark]
    public ExtractedResult<string[]> ExtractOneAcrossRunsParallelCached()
    {
        return Process.Parallel.Cached.ExtractOne(_query, _events, static strings => strings[0], _extractScorer, parallelOptions: _parallelOptions);
    }

    private static string[][] BuildEvents(int count)
    {
        var events = new string[count][];
        for (var i = 0; i < count; i++)
        {
            var teamA = Teams[i % Teams.Length];
            var teamB = Teams[(i * 7 + 3) % Teams.Length];
            var venue = Venues[(i * 5 + 1) % Venues.Length];
            var date = Dates[(i * 3 + 2) % Dates.Length];
            var time = Times[(i * 11 + 4) % Times.Length];
            events[i] = [$"{teamA} vs {teamB} regular season series game {(i % 7) + 1}", venue, date, time];
        }

        events[count / 3] = ["new york mets vs chicago cubs game 7 postseason night matchup", "CitiField", "2017-03-19", "8pm"];
        events[(count * 2) / 3] = ["chicago cubs vs new york mets game 7 postseason night matchup", "CitiField", "2017-03-19", "8pm"];
        return events;
    }
}
