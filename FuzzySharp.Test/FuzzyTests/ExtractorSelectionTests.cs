using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class ExtractorSelectionTests
{
    private static readonly Func<string, string> IdentityProcessor = value => value;
    private static readonly ParallelOptions TestParallelOptions = new() { MaxDegreeOfParallelism = 2 };

    [Fact]
    public void ExtractOne_EmptyChoices_ReturnsNull()
    {
        var scorer = new MapScorer();

        Assert.Null(ResultExtractor.ExtractOne("query", [], IdentityProcessor, scorer));
    }

    [Fact]
    public void ExtractOne_AllChoicesBelowCutoff_ReturnsNull()
    {
        var scorer = new MapScorer(("a", 10), ("b", 20));

        Assert.Null(ResultExtractor.ExtractOne("query", ["a", "b"], IdentityProcessor, scorer, cutoff: 30));
    }

    [Fact]
    public void ExtractOne_SelectsBest_UsesCutoffInclusively_AndKeepsOriginalIndex()
    {
        var choices = new[] { "below", "winner", "also-winner", "lower" };
        var scorer = new MapScorer(("below", 10), ("winner", 80), ("also-winner", 80), ("lower", 70));

        var result = ResultExtractor.ExtractOne("query", choices, IdentityProcessor, scorer, cutoff: 80);

        AssertResult(result, "winner", 80, 1);
    }

    [Fact]
    public void ExtractOne_GenericValueChoices_WithStringQuery_UsesExtractor()
    {
        var choices = new[] { new Choice("below"), new Choice("winner"), new Choice("lower") };
        var scorer = new MapScorer(("below", 20), ("winner", 90), ("lower", 80));

        var result = ResultExtractor.ExtractOne("query", choices, static choice => choice.Name, IdentityProcessor, scorer, cutoff: 50);

        Assert.Same(choices[1], result.Value);
        Assert.Equal(90, result.Score);
        Assert.Equal(1, result.Index);
    }

    [Fact]
    public void ExtractOne_GenericQueryAndChoices_UsesExtractor()
    {
        var query = new Choice("query");
        var choices = new[] { new Choice("lower"), new Choice("winner") };
        var scorer = new MapScorer(("lower", 60), ("winner", 95));

        var result = ResultExtractor.ExtractOne(query, choices, static choice => choice.Name, IdentityProcessor, scorer);

        Assert.Same(choices[1], result.Value);
        Assert.Equal(95, result.Score);
        Assert.Equal(1, result.Index);
    }

    [Fact]
    public void ExtractOne_CachedSequential_Parallel_AndParallelCached_PreserveBestTieAndIndex()
    {
        var choices = new[] { "below", "first-best", "lower", "second-best" };
        var scorer = new MapScorer(("below", 10), ("first-best", 88), ("lower", 70), ("second-best", 88));
        var cachedScorer = new CachedMapScorer(("below", 10), ("first-best", 88), ("lower", 70), ("second-best", 88));

        AssertResult(ResultExtractor.Cached.ExtractOne(choices, IdentityProcessor, cachedScorer), "first-best", 88, 1);
        AssertResult(ResultExtractor.Parallel.ExtractOne("query", choices, IdentityProcessor, scorer, parallelOptions: TestParallelOptions), "first-best", 88, 1);
        AssertResult(ResultExtractor.Parallel.Cached.ExtractOne(choices, IdentityProcessor, cachedScorer, parallelOptions: TestParallelOptions), "first-best", 88, 1);
    }

    [Fact]
    public void ExtractOne_OneShotEnumerable_IsEnumeratedOnce()
    {
        var choices = new OneShotEnumerable<string>(["a", "winner", "b"]);
        var scorer = new MapScorer(("a", 10), ("winner", 90), ("b", 20));

        var result = ResultExtractor.ExtractOne("query", choices, IdentityProcessor, scorer);

        AssertResult(result, "winner", 90, 1);
    }

    [Fact]
    public void ExtractOne_InvokesProcessorExtractorAndScorerOncePerInput()
    {
        var query = new Choice("query");
        var choices = new[] { new Choice("a"), new Choice("winner"), new Choice("b") };
        var scorer = new MapScorer(("a", 10), ("winner", 90), ("b", 20));
        var extractorCalls = 0;
        var processorCalls = 0;

        var result = ResultExtractor.ExtractOne(
            query,
            choices,
            choice =>
            {
                Interlocked.Increment(ref extractorCalls);
                return choice.Name;
            },
            value =>
            {
                Interlocked.Increment(ref processorCalls);
                return value;
            },
            scorer);

        Assert.Same(choices[1], result.Value);
        Assert.Equal(4, extractorCalls);
        Assert.Equal(4, processorCalls);
        Assert.Equal(3, scorer.ScoreCalls);
    }

    [Fact]
    public void ExtractTop_EmptyAndNoAccepted_ReturnEmpty()
    {
        var scorer = new MapScorer(("a", 10), ("b", 20));

        Assert.Empty(ResultExtractor.ExtractTop("query", [], IdentityProcessor, scorer, limit: 5));
        Assert.Empty(ResultExtractor.ExtractTop("query", ["a", "b"], IdentityProcessor, scorer, limit: 5, cutoff: 30));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    public void ExtractTop_SequentialMatchesLegacyTopNIncludingTies(int limit)
    {
        var choices = new[] { "below", "tie-a", "mid", "tie-b", "edge", "top", "tie-c" };
        var scorer = new MapScorer(("below", 10), ("tie-a", 80), ("mid", 70), ("tie-b", 80), ("edge", 60), ("top", 95), ("tie-c", 80));

        var expected = LegacyTop("query", choices, IdentityProcessor, scorer, limit, cutoff: 60);
        var actual = ResultExtractor.ExtractTop("query", choices, IdentityProcessor, scorer, limit, cutoff: 60);

        AssertSameResults(expected, actual);
    }

    [Fact]
    public void ExtractTop_GenericCachedParallelVariantsMatchLegacyTopN()
    {
        var choices = new[] { new Choice("below"), new Choice("tie-a"), new Choice("mid"), new Choice("tie-b"), new Choice("top") };
        var scorer = new MapScorer(("below", 10), ("tie-a", 80), ("mid", 70), ("tie-b", 80), ("top", 95));
        var cachedScorer = new CachedMapScorer(("below", 10), ("tie-a", 80), ("mid", 70), ("tie-b", 80), ("top", 95));
        var expected = LegacyTop("query", choices, static choice => choice.Name, IdentityProcessor, scorer, limit: 3, cutoff: 70);

        AssertSameResults(expected, ResultExtractor.ExtractTop("query", choices, static choice => choice.Name, IdentityProcessor, scorer, limit: 3, cutoff: 70));
        AssertSameResults(expected, ResultExtractor.Cached.ExtractTop(choices, static choice => choice.Name, IdentityProcessor, cachedScorer, limit: 3, cutoff: 70));
        AssertSameResults(expected, ResultExtractor.Parallel.ExtractTop("query", choices, static choice => choice.Name, IdentityProcessor, scorer, limit: 3, cutoff: 70, parallelOptions: TestParallelOptions));
        AssertSameResults(expected, ResultExtractor.Parallel.Cached.ExtractTop(choices, static choice => choice.Name, IdentityProcessor, cachedScorer, limit: 3, cutoff: 70, parallelOptions: TestParallelOptions));
    }

    [Fact]
    public void ExtractTop_LimitZeroAndNegative_PreserveLegacyBehavior()
    {
        var scorer = new MapScorer(("accepted", 50));
        var choices = new[] { "accepted" };

        Assert.Throws<InvalidOperationException>(() =>
            ResultExtractor.ExtractTop("query", choices, IdentityProcessor, scorer, limit: 0).ToList());
        Assert.Throws<InvalidOperationException>(() =>
            ResultExtractor.ExtractTop("query", choices, IdentityProcessor, scorer, limit: -1).ToList());
        Assert.Empty(ResultExtractor.ExtractTop("query", choices, IdentityProcessor, scorer, limit: 0, cutoff: 90));
    }

    [Fact]
    public void ExtractTop_SequentialIsDeferredAndRepeatedEnumerationScoresAgain()
    {
        var choices = new[] { "a", "winner", "b" };
        var scorer = new MapScorer(("a", 10), ("winner", 90), ("b", 20));
        var processorCalls = 0;
        string Processor(string value)
        {
            processorCalls++;
            return value;
        }

        var results = ResultExtractor.ExtractTop("query", choices, Processor, scorer, limit: 2);

        Assert.Equal(0, processorCalls);
        Assert.Equal(0, scorer.ScoreCalls);

        Assert.Equal(2, results.ToList().Count);
        Assert.Equal(4, processorCalls);
        Assert.Equal(3, scorer.ScoreCalls);

        Assert.Equal(2, results.ToList().Count);
        Assert.Equal(8, processorCalls);
        Assert.Equal(6, scorer.ScoreCalls);
    }

    [Fact]
    public void ExtractTop_ParallelScoresEagerlyButSelectsOnEnumeration()
    {
        var choices = new[] { "a", "winner", "b" };
        var scorer = new MapScorer(("a", 10), ("winner", 90), ("b", 20));
        var processorCalls = 0;
        string Processor(string value)
        {
            Interlocked.Increment(ref processorCalls);
            return value;
        }

        var results = ResultExtractor.Parallel.ExtractTop("query", choices, Processor, scorer, limit: 2, parallelOptions: TestParallelOptions);

        Assert.Equal(4, processorCalls);
        Assert.Equal(3, scorer.ScoreCalls);
        Assert.Equal(2, results.ToList().Count);
    }

    private static List<ExtractedResult<string>> LegacyTop(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        IRatioScorer scorer,
        int limit,
        int cutoff)
    {
        return LegacyWithoutOrder(query, choices, static value => value, processor, scorer, cutoff)
            .MaxN(limit)
            .Reverse()
            .ToList();
    }

    private static List<ExtractedResult<T>> LegacyTop<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        IRatioScorer scorer,
        int limit,
        int cutoff)
    {
        return LegacyWithoutOrder(query, choices, extractor, processor, scorer, cutoff)
            .MaxN(limit)
            .Reverse()
            .ToList();
    }

    private static IEnumerable<ExtractedResult<T>> LegacyWithoutOrder<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        IRatioScorer scorer,
        int cutoff)
    {
        var index = 0;
        var processedQuery = processor(query);

        foreach (var choice in choices)
        {
            var score = scorer.Score(processedQuery, processor(extractor(choice)));
            if (score >= cutoff)
            {
                yield return new ExtractedResult<T>(choice, score, index);
            }

            index++;
        }
    }

    private static void AssertSameResults<T>(IEnumerable<ExtractedResult<T>> expected, IEnumerable<ExtractedResult<T>> actual)
    {
        var expectedList = expected.ToList();
        var actualList = actual.ToList();

        Assert.Equal(expectedList.Count, actualList.Count);
        for (var i = 0; i < expectedList.Count; i++)
        {
            Assert.Equal(expectedList[i].Value, actualList[i].Value);
            Assert.Equal(expectedList[i].Score, actualList[i].Score);
            Assert.Equal(expectedList[i].Index, actualList[i].Index);
        }
    }

    private static void AssertResult(ExtractedResult<string> result, string value, int score, int index)
    {
        Assert.Equal(value, result.Value);
        Assert.Equal(score, result.Score);
        Assert.Equal(index, result.Index);
    }

    private sealed class Choice
    {
        public Choice(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class MapScorer : IRatioScorer
    {
        private readonly Dictionary<string, int> _scores;
        private int _scoreCalls;

        public MapScorer()
            : this([])
        {
        }

        public MapScorer(params (string Value, int Score)[] scores)
        {
            _scores = scores.ToDictionary(score => score.Value, score => score.Score);
        }

        public int ScoreCalls => _scoreCalls;

        public int Score(string input1, string input2)
        {
            Interlocked.Increment(ref _scoreCalls);
            return _scores.TryGetValue(input2, out var score) ? score : 0;
        }

        public int Score(string input1, string input2, Func<string, string> preprocessor)
        {
            return Score(preprocessor(input1), preprocessor(input2));
        }
    }

    private sealed class CachedMapScorer : ICachedRatioScorer
    {
        private readonly Dictionary<string, int> _scores;

        public CachedMapScorer(params (string Value, int Score)[] scores)
        {
            _scores = scores.ToDictionary(score => score.Value, score => score.Score);
        }

        public int Score(string input2)
        {
            return _scores.TryGetValue(input2, out var score) ? score : 0;
        }

        public void Dispose()
        {
        }
    }

    private sealed class OneShotEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _items;
        private int _enumerated;

        public OneShotEnumerable(IEnumerable<T> items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Interlocked.Exchange(ref _enumerated, 1) == 1)
            {
                throw new InvalidOperationException("Sequence was enumerated more than once.");
            }

            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
