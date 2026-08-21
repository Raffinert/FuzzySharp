using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class PartialRatioStrategy<T> where T : IEquatable<T>
{
    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns the partial fuzz.ratio for that alignment, as a value in [0…100].
    /// </summary>
    public static double Calculate(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var alignment = PartialRatioAlignment(input1, input2);

        return alignment.Score;
    }

    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns a ScoreAlignment with a score in [0…100].
    /// </summary>
    internal static ScoreAlignment PartialRatioAlignment(
        ReadOnlySpan<T> shorter,
        ReadOnlySpan<T> longer,
        Processor<T> processor = null
    )
    {
        // 1) Optional preprocessing
        if (processor != null)
        {
            processor(ref shorter);
            processor(ref longer);
        }

        // 2) Handle both empty → perfect match
        if (shorter.IsEmpty && longer.IsEmpty)
        {
            return new ScoreAlignment(100.0, 0, 0, 0, 0);
        }

        // 3) Determine shorter/longer
        var swapped = SequenceUtils.SwapIfSourceIsLonger(ref shorter, ref longer);

        // 4) Call the core PartialRatioImpl
        var res = PartialRatioImpl(shorter, longer);

        // 5) If same-length inputs and not perfect, try the other direction
        if (res.Score < 100.0 && shorter.Length == longer.Length)
        {
            var res2 = PartialRatioImpl(longer, shorter, res.Score / 100.0);
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

        // 6) If we swapped at step 3, swap back the src/dest in the result
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
        double cutoff = 0.0
    )
    {
        int len1 = s1.Length, len2 = s2.Length;
        if (len1 > len2)
            throw new ArgumentException("Requires s1.Length <= s2.Length");

        using var patternMatchVector = PatternMatchVector.Create(s1);
        return PartialRatioImpl(patternMatchVector, s2, cutoff);
    }

    /// <summary>
    /// C# equivalent of rapidfuzz.distance._partial_ratio_impl
    /// Assumes s1.Length <= s2.Length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ScoreAlignment PartialRatioImpl(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        double cutoff = 0.0)
    {
        int len1 = s1Vector.Length, len2 = s2.Length;
        if (len1 > len2)
            throw new ArgumentException("Requires s1.Length <= s2.Length");

        // Initial best covers s2[0..len1)
        var res = new ScoreAlignment(0, 0, len1, 0, len1);

        if (len1 == 0 || len2 == 0)
            return res;

        if (len2 > len1)
        {
            int maximum = len1 + len1;
            int windowCount = len2 - len1;
            int cutoffDist = (int)Math.Ceiling(maximum * (1.0 - cutoff));
            int bestDist = int.MaxValue;
            int[] scores = ArrayPool<int>.Shared.Rent(windowCount);

            Polyfill.ArrayFill(scores, int.MaxValue, 0, windowCount);

            try
            {
                var windows = new List<(int First, int Second)>(4) { (0, windowCount - 1) };
                var newWindows = new List<(int First, int Second)>(4);

                while (windows.Count > 0)
                {
                    foreach (var window in windows)
                    {
                        int first = window.First;
                        int second = window.Second;

                        if (scores[first] == int.MaxValue)
                        {
                            int dist = Indel.DistanceImpl(s1Vector, s2.Slice(first, len1));
                            scores[first] = dist;
                            if (dist < cutoffDist)
                            {
                                cutoffDist = bestDist = dist;
                                res.DestStart = first;
                                res.DestEnd = first + len1;
                                if (bestDist == 0)
                                {
                                    res.Score = 100.0;
                                    return res;
                                }
                            }
                        }

                        if (scores[second] == int.MaxValue)
                        {
                            int dist = Indel.DistanceImpl(s1Vector, s2.Slice(second, len1));
                            scores[second] = dist;
                            if (dist < cutoffDist)
                            {
                                cutoffDist = bestDist = dist;
                                res.DestStart = second;
                                res.DestEnd = second + len1;
                                if (bestDist == 0)
                                {
                                    res.Score = 100.0;
                                    return res;
                                }
                            }
                        }

                        int cellDiff = second - first;
                        if (cellDiff == 1)
                            continue;

                        int knownEdits = Math.Abs(scores[first] - scores[second]);
                        int maxScoreImprovement = ((cellDiff - knownEdits / 2) / 2) * 2;
                        int minScore = Math.Min(scores[first], scores[second]) - maxScoreImprovement;
                        if (minScore < cutoffDist)
                        {
                            int center = cellDiff / 2;
                            newWindows.Add((first, first + center));
                            newWindows.Add((first + center, second));
                        }
                    }

                    if (newWindows.Count == 0)
                        break;

                    windows.Clear();
                    (windows, newWindows) = (newWindows, windows);
                }

                if (bestDist != int.MaxValue)
                {
                    double score = 1.0 - (bestDist / (double)maximum);
                    if (score >= cutoff)
                    {
                        cutoff = res.Score = score;
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(scores);
            }
        }

        // 1) Prefixes shorter than len1
        for (int i = 1; i < len1; i++)
        {
            if (!s1Vector.ContainsKey(s2[i - 1])) continue;
            var slice = s2[..i];
            double sim = Indel.BlockNormalizedSimilarity(s1Vector, slice);
            if (sim > res.Score && sim >= cutoff)
            {
                res.Score = sim;
                cutoff = sim;
                res.DestStart = 0;
                res.DestEnd = i;
                if (sim >= .995) { res.Score = 100.0; return res; }
            }
        }

        // 2) Suffixes up to len1 (includes the last full-width window)
        for (int i = len2 - len1; i < len2; i++)
        {
            if (!s1Vector.ContainsKey(s2[i])) continue;
            var tail = s2[i..];
            double sim = Indel.BlockNormalizedSimilarity(s1Vector, tail);
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
