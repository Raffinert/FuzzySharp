using Raffinert.FuzzySharp.Extensions;
using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class TokenSetScorerBase : StrategySensitiveScorerBase
{
    public override double Score(string input1, string input2)
    {
        var tokens1 = new HashSet<string>(input1.SplitByAnySpace());
        var tokens2 = new HashSet<string>(input2.SplitByAnySpace());

        var intersection = GetIntersectionAndExcept(tokens1, tokens2);

        intersection.Sort();
        var sortedIntersection = string.Join(" ", intersection);
        var sortedDiff1To2 = TokenSetScorerHelpers.JoinIntersectionAndDifference(sortedIntersection, tokens1);
        var sortedDiff2To1 = TokenSetScorerHelpers.JoinIntersectionAndDifference(sortedIntersection, tokens2);

        var score1 = Scorer(sortedIntersection, sortedDiff1To2);
        var score2 = Scorer(sortedIntersection, sortedDiff2To1);
        var score3 = Scorer(sortedDiff1To2, sortedDiff2To1);

        return Math.Max(score1, Math.Max(score2, score3));
    }

    private static List<string> GetIntersectionAndExcept(HashSet<string> first, HashSet<string> second) =>
        TokenSetScorerHelpers.GetIntersectionAndExcept(first, second);
}

internal static class TokenSetScorerHelpers
{
    internal static List<string> GetIntersectionAndExcept(HashSet<string> first, HashSet<string> second)
    {
        var intersection = new List<string>(Math.Min(first.Count, second.Count));

        // It is safe to mutate the second set while enumerating the first.
        // Remove the matched items from the first set after enumeration, which
        // avoids allocating first.ToArray() on every score.
        foreach (var item in first)
        {
            if (second.Remove(item))
            {
                intersection.Add(item);
            }
        }

        first.ExceptWith(intersection);
        return intersection;
    }

    internal static string JoinIntersectionAndDifference(string sortedIntersection, HashSet<string> difference)
    {
        if (difference.Count == 0)
            return sortedIntersection;

        var sortedDifference = JoinSorted(difference);
        return sortedIntersection.Length == 0
            ? sortedDifference
            : string.Concat(sortedIntersection, " ", sortedDifference);
    }

    private static string JoinSorted(IEnumerable<string> tokens)
    {
        var sortedTokens = new List<string>(tokens);
        sortedTokens.Sort();
        return string.Join(" ", sortedTokens);
    }
}
