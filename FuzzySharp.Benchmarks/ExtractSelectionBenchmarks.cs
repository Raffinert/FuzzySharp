using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using System.Threading.Tasks;

namespace Raffinert.FuzzySharp.Benchmarks;

public enum ExtractSelectionScorerKind
{
    Cheap,
    WeightedRatio
}

[MemoryDiagnoser]
[RankColumn]
public class ExtractSelectionOneBenchmarks
{
    private readonly ParallelOptions _parallelOptions = new() { MaxDegreeOfParallelism = 4 };
    private string[] _choices = null!;
    private IRatioScorer _scorer = null!;
    private ICachedRatioScorer _cachedScorer = null!;

    [Params(1_000, 10_000, 100_000)]
    public int ChoiceCount { get; set; }

    [Params(0, 95)]
    public int Cutoff { get; set; }

    [Params(ExtractSelectionScorerKind.Cheap, ExtractSelectionScorerKind.WeightedRatio)]
    public ExtractSelectionScorerKind ScorerKind { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _choices = CreateChoices(ChoiceCount);
        _scorer = ScorerKind == ExtractSelectionScorerKind.Cheap
            ? new CheapScorer()
            : ScorerCache.Get<WeightedRatioScorer>();
        _cachedScorer = ScorerKind == ExtractSelectionScorerKind.Cheap
            ? new CheapCachedScorer()
            : new CachedWeightedRatioScorer(Query);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _cachedScorer.Dispose();
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_ExtractOne()
    {
        return ResultExtractor.ExtractOne(Query, _choices, Identity, _scorer, Cutoff);
    }

    [Benchmark(Baseline = true)]
    public ExtractedResult<string> Legacy_ExtractOne()
    {
        return LegacyExtractWithoutOrder(Query, _choices, Identity, _scorer, Cutoff).Max()!;
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_Cached_ExtractOne()
    {
        return ResultExtractor.Cached.ExtractOne(_choices, Identity, _cachedScorer, Cutoff);
    }

    [Benchmark]
    public ExtractedResult<string> Legacy_Cached_ExtractOne()
    {
        return LegacyCachedExtractWithoutOrder(_choices, Identity, _cachedScorer, Cutoff).Max()!;
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_Parallel_ExtractOne()
    {
        return ResultExtractor.Parallel.ExtractOne(Query, _choices, Identity, _scorer, Cutoff, _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string> Legacy_Parallel_ExtractOne()
    {
        return LegacyParallelExtractWithoutOrder(Query, _choices, Identity, _scorer, Cutoff, _parallelOptions).Max()!;
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_ParallelCached_ExtractOne()
    {
        return ResultExtractor.Parallel.Cached.ExtractOne(_choices, Identity, _cachedScorer, Cutoff, _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string> Legacy_ParallelCached_ExtractOne()
    {
        return LegacyParallelCachedExtractWithoutOrder(_choices, Identity, _cachedScorer, Cutoff, _parallelOptions).Max()!;
    }

    internal const string Query = "choice 99999 chicago cubs vs new york mets";

    internal static readonly Func<string, string> Identity = static value => value;

    internal static string[] CreateChoices(int count)
    {
        var choices = new string[count];
        for (var i = 0; i < choices.Length; i++)
        {
            choices[i] = $"choice {i} chicago cubs vs new york mets";
        }

        return choices;
    }

    internal static IEnumerable<ExtractedResult<string>> LegacyExtractWithoutOrder(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        IRatioScorer scorer,
        int cutoff)
    {
        var index = 0;
        var processedQuery = processor(query);

        foreach (var choice in choices)
        {
            var score = scorer.Score(processedQuery, processor(choice));
            if (score >= cutoff)
            {
                yield return new ExtractedResult<string>(choice, score, index);
            }

            index++;
        }
    }

    internal static IEnumerable<ExtractedResult<string>> LegacyCachedExtractWithoutOrder(
        IEnumerable<string> choices,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        int cutoff)
    {
        var index = 0;

        foreach (var choice in choices)
        {
            var score = scorer.Score(processor(choice));
            if (score >= cutoff)
            {
                yield return new ExtractedResult<string>(choice, score, index);
            }

            index++;
        }
    }

    internal static IEnumerable<ExtractedResult<string>> LegacyParallelExtractWithoutOrder(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        IRatioScorer scorer,
        int cutoff,
        ParallelOptions parallelOptions)
    {
        var materializedChoices = choices.ToList();
        var result = new ExtractedResult<string>[materializedChoices.Count];
        var processedQuery = processor(query);

        Parallel.ForEach(materializedChoices, parallelOptions, (choice, _, index) =>
        {
            var score = scorer.Score(processedQuery, processor(choice));
            if (score >= cutoff)
            {
                result[index] = new ExtractedResult<string>(choice, score, (int)index);
            }
        });

        return result.Where(static result => result != null);
    }

    internal static IEnumerable<ExtractedResult<string>> LegacyParallelCachedExtractWithoutOrder(
        IEnumerable<string> choices,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        int cutoff,
        ParallelOptions parallelOptions)
    {
        var materializedChoices = choices.ToList();
        var result = new ExtractedResult<string>[materializedChoices.Count];

        Parallel.ForEach(materializedChoices, parallelOptions, (choice, _, index) =>
        {
            var score = scorer.Score(processor(choice));
            if (score >= cutoff)
            {
                result[index] = new ExtractedResult<string>(choice, score, (int)index);
            }
        });

        return result.Where(static result => result != null);
    }

    private sealed class CheapScorer : IRatioScorer
    {
        public double Score(string input1, string input2)
        {
            return ScoreFromChoice(input2);
        }

        public double Score(string input1, string input2, Func<string, string> preprocessor)
        {
            return Score(preprocessor(input1), preprocessor(input2));
        }
    }

    private sealed class CheapCachedScorer : ICachedRatioScorer
    {
        public double Score(string input2)
        {
            return ScoreFromChoice(input2);
        }

        public void Dispose()
        {
        }
    }

    private static int ScoreFromChoice(string value)
    {
        var start = "choice ".Length;
        var end = value.IndexOf(' ', start);
        return int.Parse(value.AsSpan(start, end - start)) % 100;
    }
}

[MemoryDiagnoser]
[RankColumn]
public class ExtractSelectionTopBenchmarks
{
    private readonly ParallelOptions _parallelOptions = new() { MaxDegreeOfParallelism = 4 };
    private string[] _choices = null!;
    private IRatioScorer _scorer = null!;
    private ICachedRatioScorer _cachedScorer = null!;

    [Params(1_000, 10_000, 100_000)]
    public int ChoiceCount { get; set; }

    [Params(1, 5, 10, 100)]
    public int Limit { get; set; }

    [Params(0, 95)]
    public int Cutoff { get; set; }

    [Params(ExtractSelectionScorerKind.Cheap, ExtractSelectionScorerKind.WeightedRatio)]
    public ExtractSelectionScorerKind ScorerKind { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _choices = ExtractSelectionOneBenchmarks.CreateChoices(ChoiceCount);
        _scorer = ScorerKind == ExtractSelectionScorerKind.Cheap
            ? new CheapScorer()
            : ScorerCache.Get<WeightedRatioScorer>();
        _cachedScorer = ScorerKind == ExtractSelectionScorerKind.Cheap
            ? new CheapCachedScorer()
            : new CachedWeightedRatioScorer(ExtractSelectionOneBenchmarks.Query);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _cachedScorer.Dispose();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_ExtractTop()
    {
        return ResultExtractor.ExtractTop(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Limit, Cutoff).ToList();
    }

    [Benchmark(Baseline = true)]
    public List<ExtractedResult<string>> Legacy_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyExtractWithoutOrder(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_Cached_ExtractTop()
    {
        return ResultExtractor.Cached.ExtractTop(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Limit, Cutoff).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_Cached_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyCachedExtractWithoutOrder(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_Parallel_ExtractTop()
    {
        return ResultExtractor.Parallel.ExtractTop(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Limit, Cutoff, _parallelOptions).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_Parallel_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyParallelExtractWithoutOrder(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff, _parallelOptions)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_ParallelCached_ExtractTop()
    {
        return ResultExtractor.Parallel.Cached.ExtractTop(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Limit, Cutoff, _parallelOptions).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_ParallelCached_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyParallelCachedExtractWithoutOrder(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff, _parallelOptions)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    private sealed class CheapScorer : IRatioScorer
    {
        public double Score(string input1, string input2)
        {
            return ScoreFromChoice(input2);
        }

        public double Score(string input1, string input2, Func<string, string> preprocessor)
        {
            return Score(preprocessor(input1), preprocessor(input2));
        }
    }

    private sealed class CheapCachedScorer : ICachedRatioScorer
    {
        public double Score(string input2)
        {
            return ScoreFromChoice(input2);
        }

        public void Dispose()
        {
        }
    }

    private static int ScoreFromChoice(string value)
    {
        var start = "choice ".Length;
        var end = value.IndexOf(' ', start);
        return int.Parse(value.AsSpan(start, end - start)) % 100;
    }
}

[MemoryDiagnoser]
[RankColumn]
public class ExtractSelectionFocusedBenchmarks
{
    private const int ChoiceCount = 100_000;
    private const int Limit = 10;

    private readonly ParallelOptions _parallelOptions = new() { MaxDegreeOfParallelism = 4 };
    private readonly IRatioScorer _scorer = new CheapScorer();
    private readonly ICachedRatioScorer _cachedScorer = new CheapCachedScorer();
    private string[] _choices = null!;

    [Params(0, 95)]
    public int Cutoff { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _choices = ExtractSelectionOneBenchmarks.CreateChoices(ChoiceCount);
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_ExtractOne()
    {
        return ResultExtractor.ExtractOne(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff);
    }

    [Benchmark(Baseline = true)]
    public ExtractedResult<string> Legacy_ExtractOne()
    {
        return ExtractSelectionOneBenchmarks.LegacyExtractWithoutOrder(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff).Max()!;
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_Cached_ExtractOne()
    {
        return ResultExtractor.Cached.ExtractOne(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff);
    }

    [Benchmark]
    public ExtractedResult<string> Legacy_Cached_ExtractOne()
    {
        return ExtractSelectionOneBenchmarks.LegacyCachedExtractWithoutOrder(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff).Max()!;
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_Parallel_ExtractOne()
    {
        return ResultExtractor.Parallel.ExtractOne(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff, _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string> Legacy_Parallel_ExtractOne()
    {
        return ExtractSelectionOneBenchmarks.LegacyParallelExtractWithoutOrder(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff, _parallelOptions).Max()!;
    }

    [Benchmark]
    public ExtractedResult<string> Optimized_ParallelCached_ExtractOne()
    {
        return ResultExtractor.Parallel.Cached.ExtractOne(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff, _parallelOptions);
    }

    [Benchmark]
    public ExtractedResult<string> Legacy_ParallelCached_ExtractOne()
    {
        return ExtractSelectionOneBenchmarks.LegacyParallelCachedExtractWithoutOrder(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff, _parallelOptions).Max()!;
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_ExtractTop()
    {
        return ResultExtractor.ExtractTop(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Limit, Cutoff).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyExtractWithoutOrder(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_Cached_ExtractTop()
    {
        return ResultExtractor.Cached.ExtractTop(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Limit, Cutoff).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_Cached_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyCachedExtractWithoutOrder(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_Parallel_ExtractTop()
    {
        return ResultExtractor.Parallel.ExtractTop(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Limit, Cutoff, _parallelOptions).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_Parallel_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyParallelExtractWithoutOrder(ExtractSelectionOneBenchmarks.Query, _choices, ExtractSelectionOneBenchmarks.Identity, _scorer, Cutoff, _parallelOptions)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Optimized_ParallelCached_ExtractTop()
    {
        return ResultExtractor.Parallel.Cached.ExtractTop(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Limit, Cutoff, _parallelOptions).ToList();
    }

    [Benchmark]
    public List<ExtractedResult<string>> Legacy_ParallelCached_ExtractTop()
    {
        return ExtractSelectionOneBenchmarks.LegacyParallelCachedExtractWithoutOrder(_choices, ExtractSelectionOneBenchmarks.Identity, _cachedScorer, Cutoff, _parallelOptions)
            .MaxN(Limit)
            .Reverse()
            .ToList();
    }

    private sealed class CheapScorer : IRatioScorer
    {
        public double Score(string input1, string input2)
        {
            return ScoreFromChoice(input2);
        }

        public double Score(string input1, string input2, Func<string, string> preprocessor)
        {
            return Score(preprocessor(input1), preprocessor(input2));
        }
    }

    private sealed class CheapCachedScorer : ICachedRatioScorer
    {
        public double Score(string input2)
        {
            return ScoreFromChoice(input2);
        }

        public void Dispose()
        {
        }
    }

    private static int ScoreFromChoice(string value)
    {
        var start = "choice ".Length;
        var end = value.IndexOf(' ', start);
        return int.Parse(value.AsSpan(start, end - start)) % 100;
    }
}
