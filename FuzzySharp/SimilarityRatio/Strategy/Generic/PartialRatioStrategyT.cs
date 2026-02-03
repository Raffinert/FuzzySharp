using Raffinert.FuzzySharp.Utils;
using System;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class PartialRatioStrategy<T> where T : IEquatable<T>
{
    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns the partial fuzz.ratio for that alignment, as a value in [0…100].
    /// </summary>
    public static int Calculate(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var alignment = PartialRatioAlignment(input1, input2);

        return (int)Math.Round(alignment.Score);
    }

    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns a ScoreAlignment (with a score in [0…100]) or null if below cutoff.
    /// </summary>
    internal static ScoreAlignment PartialRatioAlignment(
        ReadOnlySpan<T> shorter,
        ReadOnlySpan<T> longer,
        Processor<T> processor = null,
        double? scoreCutoff = null
    )
    {
        // 1) Optional preprocessing
        if (processor != null)
        {
            processor(ref shorter);
            processor(ref longer);
        }

        // 2) Normalize cutoff to 0…100
        double cutoff100 = scoreCutoff.GetValueOrDefault(0.0);

        // 3) Handle both empty → perfect match
        if (shorter.IsEmpty && longer.IsEmpty)
        {
            return new ScoreAlignment(100.0, 0, 0, 0, 0);
        }

        // 4) Determine shorter/longer
        var swapped = SequenceUtils.SwapIfSourceIsLonger(ref shorter, ref longer);

        // 5) Call the core PartialRatioImpl with cutoff in [0..1]
        double fracCutoff = cutoff100 / 100.0;
        var res = PartialRatioImpl(shorter, longer, fracCutoff);

        // 6) If same-length inputs and not perfect, try the other direction
        if (res.Score < 100.0 && shorter.Length == longer.Length)
        {
            // bump cutoff to whatever we got
            double newCutoff100 = Math.Max(cutoff100, res.Score);
            double newFracCutoff = newCutoff100 / 100.0;

            var res2 = PartialRatioImpl(longer, shorter, newFracCutoff);
            if (res2.Score > res.Score)
            {
                // swap src/dest
                res = new ScoreAlignment(
                    res2.Score,
                    SrcStart: res2.DestStart,
                    SrcEnd: res2.DestEnd,
                    DestStart: res2.SrcStart,
                    DestEnd: res2.SrcEnd
                );
            }
        }

        // 7) If below cutoff, return null
        if (res.Score < cutoff100)
            return res with { Score = 0 };

        // 8) If we swapped at step 4, swap back the src/dest in the result
        if (swapped)
        {
            res = new ScoreAlignment(
                res.Score,
                SrcStart: res.DestStart,
                SrcEnd: res.DestEnd,
                DestStart: res.SrcStart,
                DestEnd: res.SrcEnd
            );
        }

        return res;
    }

    /// <summary>
    /// C# equivalent of rapidfuzz.distance._partial_ratio_impl
    /// Assumes s1.Length <= s2.Length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ScoreAlignment PartialRatioImpl(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        double? scoreCutoff = null
    )
    {
        int len1 = s1.Length, len2 = s2.Length;
        if (len1 > len2)
            throw new ArgumentException("Requires s1.Length <= s2.Length");

        using var patternMatchVector = PatternMatchVector.Create(s1);
        return PartialRatioImpl(s1, s2, patternMatchVector, scoreCutoff);
    }

    /// <summary>
    /// C# equivalent of rapidfuzz.distance._partial_ratio_impl
    /// Assumes s1.Length <= s2.Length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ScoreAlignment PartialRatioImpl(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        IPatternMatchVector<T> patternMatchVector,
        double? scoreCutoff = null
    )
    {
        int len1 = s1.Length, len2 = s2.Length;
        if (len1 > len2)
            throw new ArgumentException("Requires s1.Length <= s2.Length");

        // Initial best covers s2[0..len1)
        var res = new ScoreAlignment(0, 0, len1, 0, len1);

        if (len1 == 0 || len2 == 0)
            return res;

        double cutoff = scoreCutoff ?? 0.0;

        // 1) Prefixes shorter than len1
        for (int i = 1; i < len1; i++)
        {
            if (!patternMatchVector.ContainsKey(s2[i - 1])) continue;
            var slice = s2[..i];
            double sim = Indel.BlockNormalizedSimilarity(patternMatchVector, s1, slice);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = 0;
                res.DestEnd = i;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        // 2) Full-width windows of length len1
        for (int i = 0; i <= len2 - len1; i++)
        {
            if (!patternMatchVector.ContainsKey(s2[i + len1 - 1])) continue;
            var window = s2[i..(i + len1)];
            double sim = Indel.BlockNormalizedSimilarity(patternMatchVector, s1, window);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = i;
                res.DestEnd = i + len1;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        // 3) Suffixes shorter than len1
        for (int i = len2 - len1 + 1; i < len2; i++)
        {
            if (!patternMatchVector.ContainsKey(s2[i])) continue;
            var tail = s2[i..];
            double sim = Indel.BlockNormalizedSimilarity(patternMatchVector, s1, tail);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = i;
                res.DestEnd = len2;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        res.Score *= 100.0;

        return res;
    }

    internal record struct ScoreAlignment(double Score, int SrcStart, int SrcEnd, int DestStart, int DestEnd);
}
