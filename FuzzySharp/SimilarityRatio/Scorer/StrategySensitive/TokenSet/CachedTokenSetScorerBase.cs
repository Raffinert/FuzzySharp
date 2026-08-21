using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extensions;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedTokenSetScorerBase(string input1) : ICachedRatioScorer
{
    private HashSet<string> Tokens1 { get; set; } = new HashSet<string>(input1.SplitByAnySpace());
    protected abstract FuzzySharp.Scorer Scorer { get; }

    public double Score(string input2)
    {
        var tokens2 = new HashSet<string>(input2.SplitByAnySpace());
        var tokens1 = new HashSet<string>(Tokens1);

        var intersection = TokenSetScorerHelpers.GetIntersectionAndExcept(tokens1, tokens2);

        intersection.Sort();

        var sortedIntersection = string.Join(" ", intersection);
        var sortedDiff1To2 = TokenSetScorerHelpers.JoinIntersectionAndDifference(sortedIntersection, tokens1);
        var sortedDiff2To1 = TokenSetScorerHelpers.JoinIntersectionAndDifference(sortedIntersection, tokens2);

        var score1 = Scorer(sortedIntersection, sortedDiff1To2);
        var score2 = Scorer(sortedIntersection, sortedDiff2To1);
        var score3 = Scorer(sortedDiff1To2, sortedDiff2To1);

        return Math.Max(score1, Math.Max(score2, score3));
    }

    public void Dispose()
    {
    }
}
