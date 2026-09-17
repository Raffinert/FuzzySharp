using System;
using System.Linq;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Xunit;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class CachedScorerTests
{
    public static TheoryData<string, string> WeightedRatioCases => new()
    {
        { "b a", "a b" },
        { "invoice number", "number invoice" },
        { "Acme 123", "123 Acme Ltd" },
        { "red red blue", "blue red red" },
        { "Acme,   123", "123 Acme" },
    };

    [Theory]
    [MemberData(nameof(WeightedRatioCases))]
    public void CachedWeightedRatioMatchesUncached(string query, string candidate)
    {
        using var cached = new CachedWeightedRatioScorer(query);

        Assert.Equal(Fuzz.WeightedRatio(query, candidate), cached.Score(candidate), precision: 10);
    }

    [Fact]
    public void CachedPipelineMatchesNormalPipelineForTokenReordering()
    {
        var query = "b a";
        var choices = new[] { "a b", "c d", "b a" };

        var normal = Process.Configure().Build()
            .ExtractTop(query, choices, StringPreprocessor.None, limit: 3, cutoff: 30)
            .ToList();
        var cached = Process.Configure().Cached().Build()
            .ExtractTop(query, choices, StringPreprocessor.None, limit: 3, cutoff: 30)
            .ToList();

        Assert.Equal(normal.Count, cached.Count);
        for (var i = 0; i < normal.Count; i++)
        {
            Assert.Equal(normal[i].Value, cached[i].Value);
            Assert.Equal(normal[i].Score, cached[i].Score, precision: 10);
            Assert.Equal(normal[i].Index, cached[i].Index);
        }
    }

    [Fact]
    public void CachedPartialTokenSet_EmptyTokenCollectionScoresZero()
    {
        using var emptyQuery = new CachedPartialTokenSetScorer("");
        Assert.Equal(0, emptyQuery.Score(""));
        Assert.Equal(0, emptyQuery.Score("x"));

        using var nonEmptyQuery = new CachedPartialTokenSetScorer("x");
        Assert.Equal(0, nonEmptyQuery.Score(""));
    }
}
