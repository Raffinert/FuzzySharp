using Raffinert.FuzzySharp.Utils;
using System;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Provides static methods for calculating the Indel (insertion-deletion) distance and similarity between sequences.
/// Implements algorithms inspired by RapidFuzz's Indel distance implementation.
/// </summary>
public sealed partial class Indel
{
    /// <summary>
    /// Computes the Indel distance using precomputed block data for the first sequence.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1Vector">Precomputed per-symbol bitmasks for s1.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold. If the distance exceeds this value, returns scoreCutoff + 1.</param>
    /// <returns>The Indel distance between the two sequences.</returns>
    public static int BlockDistance<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var maximum = s1Vector.Length + s2.Length;
        var lcsSim = LongestCommonSubsequence.BlockSimilarity(s1Vector, s2);
        var dist = maximum - 2 * lcsSim;
        var result = scoreCutoff == null || dist <= scoreCutoff.Value
            ? dist
            : scoreCutoff.Value + 1;
        return result;
    }

    /// <summary>
    /// Computes the normalized Indel distance using precomputed block data for the first sequence.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1Vector"></param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold. If the distance exceeds this value, returns 1.</param>
    /// <returns>The normalized Indel distance between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double BlockNormalizedDistance<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var maximum = s1Vector.Length + s2.Length;
        var dist = BlockDistance(s1Vector, s2);
        var normDist = maximum == 0 ? 0 : dist / (double)maximum;
        var result = scoreCutoff == null || normDist <= scoreCutoff.Value
            ? normDist
            : 1;
        return result;
    }

    /// <summary>
    /// Computes the normalized Indel similarity using precomputed block data for the first sequence.
    /// This is defined as 1 - BlockNormalizedDistance(block, s1, s2).
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1Vector"></param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold. If the similarity is below this value, returns 0.</param>
    /// <returns>The normalized Indel similarity between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double BlockNormalizedSimilarity<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var normDist = BlockNormalizedDistance(s1Vector, s2);
        var normSim = 1.0 - normDist;
        var result = scoreCutoff == null || normSim >= scoreCutoff.Value
            ? normSim
            : 0;
        return result;
    }

    /// <summary>
    /// Computes the Indel distance (minimum number of insertions and deletions) between two sequences.
    /// This is defined as len(s1) + len(s2) - 2 * LCS(s1, s2).
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold. If the distance exceeds this value, returns scoreCutoff + 1.</param>
    /// <returns>The Indel distance between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Distance<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        SequenceUtils.TrimCommonAffixAndSwapIfNeeded(ref s1, ref s2);

        return DistanceImpl(s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DistanceImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        using var patternMatchVector = PatternMatchVector.Create(s1);
        return DistanceImpl(patternMatchVector, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int DistanceImpl<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var maximum = s1Vector.Length + s2.Length;
        var lcsSim = LongestCommonSubsequence.SimilarityImpl(s1Vector, s2);
        var dist = maximum - 2 * lcsSim;
        var result = scoreCutoff == null || dist <= scoreCutoff.Value
            ? dist
            : scoreCutoff.Value + 1;

        return result;
    }

    /// <summary>
    /// Computes the normalized Indel distance between two sequences, in the range [0, 1].
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold. If the distance exceeds this value, returns scoreCutoff + 1.</param>
    /// <returns>The normalized Indel distance between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NormalizedDistance<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        SequenceUtils.TrimCommonAffixAndSwapIfNeeded(ref s1, ref s2);

        return NormalizedDistanceImpl(s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizedDistanceImpl<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var dist = DistanceImpl(s1Vector, s2, scoreCutoff);
        return NormalizedDistanceImpl(s1Vector.Length, s2.Length, dist, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizedDistanceImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var dist = Distance(s1, s2);
        return NormalizedDistanceImpl(s1.Length, s2.Length, dist, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double NormalizedDistanceImpl(int s1Length, int s2Length, int distance,
        int? scoreCutoff = null)
    {
        var maximum = s1Length + s2Length;
        var normDist = maximum == 0 ? 0 : distance / (double)maximum;
        var result = scoreCutoff == null || normDist <= scoreCutoff.Value
            ? normDist
            : scoreCutoff.Value + 1;
        return result;
    }
    /// <summary>
    /// Computes the normalized Indel similarity between two sequences, in the range [0, 1].
    /// This is defined as 1 - NormalizedDistance(s1, s2).
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold. If the similarity is below this value, returns 0.</param>
    /// <returns>The normalized Indel similarity between the two sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NormalizedSimilarity<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        return NormalizedSimilarityImpl(s1, s2, scoreCutoff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double NormalizedSimilarityImpl<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var normDist = NormalizedDistanceImpl(s1Vector, s2, scoreCutoff);
        var normSim = 1 - normDist;
        var result = scoreCutoff == null || normSim >= scoreCutoff.Value
            ? normSim
            : 0;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizedSimilarityImpl<T>(ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        var normDist = NormalizedDistanceImpl(s1, s2);
        var normSim = 1 - normDist;
        var result = scoreCutoff == null || normSim >= scoreCutoff.Value
            ? normSim
            : 0;
        return result;
    }
}
