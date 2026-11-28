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

        if (tokens1.Count == 0 || tokens2.Count == 0)
        {
            return 0;
        }

        var intersection = GetIntersectionAndExcept(tokens1, tokens2);

        if (intersection.Count != 0 && (tokens1.Count == 0 || tokens2.Count == 0) && CanReturnPerfectScore)
            return 100;

        intersection.Sort();
        var sortedIntersection = string.Join(" ", intersection);
        var sortedDiff1To2 = TokenSetScorerHelpers.JoinIntersectionAndDifference(sortedIntersection, tokens1);
        var sortedDiff2To1 = TokenSetScorerHelpers.JoinIntersectionAndDifference(sortedIntersection, tokens2);

        var score1 = ScorePrefix(sortedIntersection, sortedDiff1To2);
        var score2 = ScorePrefix(sortedIntersection, sortedDiff2To1);
        var score3 = Scorer(sortedDiff1To2, sortedDiff2To1);

        return Math.Max(score1, Math.Max(score2, score3));
    }

    // Only the built-in Indel scorer can replace scoring with a length formula.
    // Custom and partial scorers must continue to call their original strategy.
    internal virtual double ScorePrefix(string prefix, string combined) => Scorer(prefix, combined);

    internal virtual bool CanReturnPerfectScore => false;

    private static List<string> GetIntersectionAndExcept(HashSet<string> first, HashSet<string> second) =>
        TokenSetScorerHelpers.GetIntersectionAndExcept(first, second);
}

internal static class TokenSetScorerHelpers
{
    internal static double PrefixRatio(int prefixLength, int combinedLength)
    {
        // DefaultRatioStrategy returns zero when either input is empty.
        if (prefixLength == 0 || combinedLength == 0)
            return 0;

        // The intersection is a prefix of each combined string, so the
        // Indel distance is exactly the difference in their lengths.
        // Keep the same arithmetic order as the normal Indel ratio.
        return 100 * (1.0 - (combinedLength - prefixLength) /
            (double)(combinedLength + prefixLength));
    }

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

    internal static string JoinIntersectionAndDifference(string sortedIntersection, List<string> difference)
    {
        if (difference.Count == 0)
            return sortedIntersection;

        difference.Sort();
        var sortedDifference = string.Join(" ", difference);
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
