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

        return TokenSetScorerHelpers.Score(Scorer, UsesBuiltInIndelRatio, intersection, tokens1, tokens2);
    }

    /// <summary>
    /// True only when the scorer strategy is the built-in Indel ratio,
    /// which lets token-set scoring use exact length-based shortcuts.
    /// Custom and partial scorers keep calling their own strategy.
    /// </summary>
    internal virtual bool UsesBuiltInIndelRatio => false;

    private static List<string> GetIntersectionAndExcept(HashSet<string> first, HashSet<string> second) =>
        TokenSetScorerHelpers.GetIntersectionAndExcept(first, second);
}

internal static class TokenSetScorerHelpers
{
    /// <summary>
    /// Shared token-set scoring after the intersection and both differences are known.
    /// The collections are consumed: <paramref name="intersection"/> and list differences are sorted in place.
    /// </summary>
    internal static double Score(FuzzySharp.Scorer scorer, bool usesBuiltInIndelRatio,
        List<string> intersection, ICollection<string> difference1, ICollection<string> difference2)
    {
        // With the Indel ratio, a nonempty intersection compared with itself scores 100.
        if (usesBuiltInIndelRatio && intersection.Count != 0 && (difference1.Count == 0 || difference2.Count == 0))
            return 100;

        intersection.Sort();
        var sortedIntersection = string.Join(" ", intersection);
        var sortedDiff1To2 = JoinIntersectionAndDifference(sortedIntersection, difference1);
        var sortedDiff2To1 = JoinIntersectionAndDifference(sortedIntersection, difference2);

        double score1, score2;
        if (usesBuiltInIndelRatio)
        {
            score1 = PrefixRatio(sortedIntersection.Length, sortedDiff1To2.Length);
            score2 = PrefixRatio(sortedIntersection.Length, sortedDiff2To1.Length);
        }
        else
        {
            score1 = scorer(sortedIntersection, sortedDiff1To2);
            score2 = scorer(sortedIntersection, sortedDiff2To1);
        }

        var score3 = scorer(sortedDiff1To2, sortedDiff2To1);

        return Math.Max(score1, Math.Max(score2, score3));
    }

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

    internal static string JoinIntersectionAndDifference(string sortedIntersection, ICollection<string> difference)
    {
        if (difference.Count == 0)
            return sortedIntersection;

        // A list difference is owned by the caller's scoring pass and can be sorted in place.
        var sortedTokens = difference as List<string> ?? new List<string>(difference);
        sortedTokens.Sort();
        var sortedDifference = string.Join(" ", sortedTokens);
        return sortedIntersection.Length == 0
            ? sortedDifference
            : string.Concat(sortedIntersection, " ", sortedDifference);
    }
}
