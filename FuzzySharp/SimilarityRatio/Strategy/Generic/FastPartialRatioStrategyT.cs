using System;
using System.Runtime.CompilerServices;
using Raffinert.FuzzySharp;
using Raffinert.FuzzySharp.Utils;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class FastPartialRatioStrategyT<T> where T : IEquatable<T>
{
    public static int Calculate(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var shorter = input1;
        var longer = input2;

        SequenceUtils.SwapIfSourceIsLonger(ref shorter, ref longer);

        using var charMask = CharMask.Create(shorter);

        var maxScore = ComputeMaxScore(shorter, longer, charMask);

        return (int)Math.Round(100 * maxScore);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double ComputeMaxScore(
        ReadOnlySpan<T> shorter,
        ReadOnlySpan<T> longer,
        CharMaskBuffer<T> charMask)
    {
        double maxScore = 0;
        var len1 = shorter.Length;
        var len2 = longer.Length;

        // Reuse the matching-block candidate generation to avoid scanning every window.
        var matchingBlocks = Levenshtein.GetMatchingBlocks(shorter, longer);
        foreach (var block in matchingBlocks)
        {
            // Offset between source and destination tells us where the shorter string could align.
            var dist = block.DestPos - block.SourcePos;
            var windowStart = dist > 0 ? dist : 0;
            var windowEnd = windowStart + len1;
            if (windowEnd > len2)
            {
                windowEnd = len2;
            }

            var window = longer.Slice(windowStart, windowEnd - windowStart);
            var ratio = Indel.BlockNormalizedSimilarity(charMask, shorter, window);

            if (ratio > maxScore)
            {
                maxScore = ratio;
                if (ratio >= 0.995)
                {
                    return 1.0;
                }
            }
        }

        return maxScore;
    }
}
