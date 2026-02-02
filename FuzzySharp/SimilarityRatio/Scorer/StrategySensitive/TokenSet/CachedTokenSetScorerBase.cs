using System;
using System.Collections.Generic;
using System.Linq;
using Raffinert.FuzzySharp.Extensions;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedTokenSetScorerBase(string input1) : CachedScorerBase
{
    private HashSet<string> Tokens1 { get; set; } = new HashSet<string>(input1.SplitByAnySpace());
    protected abstract FuzzySharp.Scorer Scorer { get; }

    public override int Score(string input2)
    {
        var tokens2 = new HashSet<string>(input2.SplitByAnySpace());
        var tokens1 = new HashSet<string>(Tokens1);

        var intersection = GetIntersectionAndExcept(tokens1, tokens2);

        intersection.Sort();

        var sortedIntersection = string.Join(" ", intersection);
        var sortedDiff1To2 = (sortedIntersection + " " + string.Join(" ", tokens1.OrderBy(s => s))).Trim();
        var sortedDiff2To1 = (sortedIntersection + " " + string.Join(" ", tokens2.OrderBy(s => s))).Trim();

        var score1 = Scorer(sortedIntersection, sortedDiff1To2);
        var score2 = Scorer(sortedIntersection, sortedDiff2To1);
        var score3 = Scorer(sortedDiff1To2, sortedDiff2To1);

        return Math.Max(score1, Math.Max(score2, score3));
    }

    private static List<T> GetIntersectionAndExcept<T>(HashSet<T> first, HashSet<T> second)
    {
        List<T> intersection = [];

        foreach (var item in first.ToArray())
        {
            if (second.Remove(item))
            {
                first.Remove(item);
                intersection.Add(item);
            }
        }

        return intersection;
    }
}