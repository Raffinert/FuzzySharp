using Raffinert.FuzzySharp.Edits;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Provides static methods for computing the Longest Common Subsequence (LCS) and related similarity metrics.
/// Implements a bit-parallel LCS algorithm inspired by RapidFuzz's LCSseq implementation.
/// </summary>
public sealed partial class LongestCommonSubsequence
{
    /// <summary>
    /// Computes the LCS-based distance between two sequences.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>The LCS distance (max(len1, len2) - LCS length).</returns>
    public static int Distance<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        using var patternMatchVector = PatternMatchVector.Create(s1);

        return DistanceImpl(patternMatchVector, s2);
    }

    private static int DistanceImpl<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        int maximum = Math.Max(s1Vector.Length, s2.Length);
        int sim = SimilarityImpl(s1Vector, s2);
        int dist = maximum - sim;

        return dist;
    }

    /// <summary>
    /// Computes the sequence of edit operations (insert, delete) to transform s1 into s2 using LCS.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>Array of edit operations (EditOp).</returns>
    public static EditOp[] GetEditOps<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        // strip any common prefix/suffix if you like
        var (prefixLen, _) = SequenceUtils.TrimCommonAffix(ref s1, ref s2);

        // now Matrix returns List<ulong[]> — one ulong[] per char of s2
        var (sim, matrix) = Matrix(s1, s2);

        int dist = s1.Length + s2.Length - 2 * sim;
        if (dist == 0)
            return [];

        var opsArray = new EditOp[dist];
        int nextIndex = dist;

        int row = s2.Length, col = s1.Length;

        while (row > 0 && col > 0)
        {
            // pick up the bit-mask vector for the previous row
            var bits = matrix[row - 1];

            // compute which block and which bit in that block is "col-1"
            int bitIndex = col - 1;
            int block = bitIndex / 64;
            int offset = bitIndex % 64;
            ulong mask = 1UL << offset;

            // if bit is set ⇒ this was a deletion in LCS fallback
            if ((bits[block] & mask) != 0)
            {
                nextIndex--;
                col--;
                opsArray[nextIndex] = new EditOp
                {
                    EditType = EditType.DELETE,
                    SourcePos = col + prefixLen,
                    DestPos = row + prefixLen
                };
            }
            else
            {
                // no deletion ⇒ move up a row
                row--;

                // but if still in-bounds and the bit is still zero ⇒ insertion
                if (row > 0)
                {
                    bits = matrix[row - 1];
                    if ((bits[block] & mask) == 0)
                    {
                        nextIndex--;
                        opsArray[nextIndex] = new EditOp
                        {
                            EditType = EditType.INSERT,
                            SourcePos = col + prefixLen,
                            DestPos = row + prefixLen
                        };
                        continue;
                    }
                }

                // otherwise it was a match/move-left in LCS
                col--;
            }
        }

        // any remaining deletes on the left edge
        while (col > 0)
        {
            nextIndex--;
            col--;
            opsArray[nextIndex] = new EditOp
            {
                EditType = EditType.DELETE,
                SourcePos = col + prefixLen,
                DestPos = row + prefixLen
            };
        }
        // any remaining inserts on the top edge
        while (row > 0)
        {
            nextIndex--;
            row--;
            opsArray[nextIndex] = new EditOp
            {
                EditType = EditType.INSERT,
                SourcePos = col + prefixLen,
                DestPos = row + prefixLen
            };
        }

        return opsArray;
    }

    /// <summary>
    /// Returns a list of matching blocks (contiguous matching subsequences) between s1 and s2.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>List of matching blocks.</returns>
    public static List<MatchingBlock> MatchingBlocks<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        var editOps = GetEditOps(s1, s2, processor);
        var matchingBlocks = editOps.AsMatchingBlocks(s1.Length, s2.Length);
        return matchingBlocks;
    }

    /// <summary>
    /// Computes the bit-parallel LCS matrix for the given sequences.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <returns>Tuple of (LCS length, matrix of bitmasks per row).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Sim, List<ulong[]> Matrix) Matrix<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        return s1.Length <= 64
            ? MatrixSingleULong(s1, s2)
            : MatrixMultipleULongs(s1, s2);
    }

    /// <summary>
    /// Computes the normalized LCS-based distance in [0, 1].
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>Normalized distance (0 = identical, 1 = completely different).</returns>
    public static double NormalizedDistance<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        if ((s1.IsEmpty && !s2.IsEmpty) || (!s1.IsEmpty && s2.IsEmpty))
        {
            return 1;
        }

        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        if (s1.IsEmpty || s2.IsEmpty)
            return 0.0;

        int maximum = Math.Max(s1.Length, s2.Length);
        double normDist = Distance(s1, s2) / (double)maximum;

        return normDist;
    }

    /// <summary>
    /// Computes the normalized LCS-based similarity in [0, 1].
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>Normalized similarity (1 = identical, 0 = completely different).</returns>
    public static double NormalizedSimilarity<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {

        if (s1.IsEmpty || s2.IsEmpty)
        {
            return 0;
        }

        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        double normSim = 1.0 - NormalizedDistance(s1, s2);

        return normSim;
    }

    /// <summary>
    /// Returns a list of opcodes describing how to turn s1 into s2 using LCS.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>List of opcodes (OpCode).</returns>
    public static List<OpCode> Opcodes<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        var editOps = GetEditOps(s1, s2, processor);
        var opCodes = editOps.AsOpCodes(s1.Length, s2.Length);
        return opCodes;
    }

    /// <summary>
    /// Computes the length of the longest common subsequence (LCS) between two sequences.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <returns>The length of the LCS.</returns>
    public static int Similarity<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null) where T : IEquatable<T>
    {
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        using var patternMatchVector = PatternMatchVector.Create(s1);

        return SimilarityImpl(patternMatchVector, s2);
    }

    internal static int SimilarityImpl<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        var sim = s1Vector.Length > 64
            ? BlockSimilarityMultipleULongs(s1Vector, s2)
            : BlockSimilaritySingleULong(s1Vector, s2);

        return sim;
    }

    /// <summary>
    /// Computes the LCS similarity using a bit-parallel algorithm for sequences that can be longer than 64 elements.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1Vector">Precomputed per-symbol bitmasks for s1.</param>
    /// <param name="s2">Second sequence (text).</param>
    /// <returns>The length of the longest common subsequence.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BlockSimilarity<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        return s1Vector.Length <= 64
            ? BlockSimilaritySingleULong(s1Vector, s2)
            : BlockSimilarityMultipleULongs(s1Vector, s2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BlockSimilaritySingleULong<T>(
        IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2
    ) where T : IEquatable<T>
    {
        if (s1Vector.Length == 0)
            return 0;

        int len1 = s1Vector.Length;
        ulong mask = len1 == 64 ? ulong.MaxValue : (1UL << len1) - 1UL;

        ulong S = mask;
        
        for (int i = 0; i < s2.Length; i++)
        {
            ulong M = s1Vector.GetOrZero(s2[i])[0];
            ulong u = S & M;
            unchecked
            {
                S = (S + u) | (S - u);
            }
        }

        return CountZeroBits(S, len1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BlockSimilarityMultipleULongs<T>(IPatternMatchVector<T> s1Vector,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        if (s1Vector.Length == 0)
            return 0;

        int len1 = s1Vector.Length;
        int blockCount = (len1 + 63) / 64;

        var scratch = ArrayPool<ulong>.Shared.Rent(blockCount);
        try
        {
            var S = scratch.AsSpan(0, blockCount);

            // --- 2) prepare the \"all-ones up to len1\" mask and state S ---
            S.Fill(ulong.MaxValue);
            int rem = len1 & 63;
            if (rem != 0)
                S[blockCount - 1] = (1UL << rem) - 1;

            // --- 3) main bit-parallel loop: S = (S + u) | (S - u)  ---
            for (int chIdx = 0; chIdx < s2.Length; chIdx++)
            {
                var U = s1Vector.GetOrZero(s2[chIdx]);

                // big-integer add/subtract using u = S & U
                ulong carry = 0;
                ulong borrow = 0;
                for (int b = 0; b < blockCount; b++)
                {
                    ulong s = S[b];
                    ulong u = s & U[b];

                    // Sum = S + u
                    ulong t = unchecked(s + u);
                    ulong c1 = t < s ? 1UL : 0UL;
                    ulong sum = unchecked(t + carry);
                    ulong c2 = sum < t ? 1UL : 0UL;
                    carry = c1 | c2;

                    // Diff = S - u
                    ulong t1 = unchecked(s - u);
                    ulong b1 = s < u ? 1UL : 0UL;
                    ulong diff = unchecked(t1 - borrow);
                    ulong b2 = t1 < borrow ? 1UL : 0UL;
                    borrow = b1 | b2;
                    // update S = Sum | Diff
                    S[b] = sum | diff;
                }
            }

            // --- 4) count zero bits in the lower len1 positions of S ---
            return CountZeroBits(S, len1);
        }
        finally
        {
            ArrayPool<ulong>.Shared.Return(scratch);
        }
    }

    private static int CountZeroBits(ulong x, int length)
    {
        // invert and mask
        ulong inv = ~x & (length == 64 ? ulong.MaxValue : (1UL << length) - 1UL);
        return Polyfill.PopCount(inv);
    }

    private static int CountZeroBits(ReadOnlySpan<ulong> S, int length)
    {
        int fullBlocks = length / 64;
        int remBits = length % 64;
        int zeros = 0;

        // all full blocks
        for (int i = 0; i < fullBlocks; i++)
            zeros += Polyfill.PopCount(~S[i]);

        // last partial block
        if (remBits > 0)
        {
            ulong mask = (1UL << remBits) - 1;
            zeros += Polyfill.PopCount(~S[fullBlocks] & mask);
        }

        return zeros;
    }

    private static (int Sim, List<ulong[]> Matrix) MatrixMultipleULongs<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        int m = s1.Length;
        if (m == 0)
            return (0, new List<ulong[]>(s2.Length));

        int blocks = (m + 64 - 1) / 64;
        // initialize S[] = all-1 in low m bits
        var S = new ulong[blocks];
        for (int i = 0; i < blocks; i++)
        {
            if (i < blocks - 1 || m % 64 == 0)
                S[i] = ulong.MaxValue;
            else
                S[i] = (1UL << (m % 64)) - 1;
        }

        // build blockTable: element → bit-mask array
        using var s1Vector = PatternMatchVector.Create(s1);

        var matrix = new List<ulong[]>(s2.Length);

        foreach (var y in s2)
        {
            // load mask for y
            var U = s1Vector.GetOrZero(y);

            // big-integer add/subtract using u = S & U
            ulong carry = 0;
            ulong borrow = 0;
            for (int b = 0; b < blocks; b++)
            {
                // u = S & U
                ulong s = S[b];
                ulong u = s & U[b];

                // Sum = S + u
                ulong t = unchecked(s + u);
                ulong c1 = t < s ? 1UL : 0UL;
                ulong sum = unchecked(t + carry);
                ulong c2 = sum < t ? 1UL : 0UL;
                carry = c1 | c2;

                // Diff = S - u
                ulong t1 = unchecked(s - u);
                ulong b1 = s < u ? 1UL : 0UL;
                ulong diff = unchecked(t1 - borrow);
                ulong b2 = t1 < borrow ? 1UL : 0UL;
                borrow = b1 | b2;
                // update S = Sum | Diff
                S[b] = sum | diff;
            }

            // snapshot row
            matrix.Add((ulong[])S.Clone());
        }

        int sim = CountZeroBits(S, m);
        return (sim, matrix);
    }

    private static (int Sim, List<ulong[]> Matrix) MatrixSingleULong<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        if (s1.IsEmpty)
            return (0, new List<ulong[]>(s2.Length));

        int m = s1.Length;
        ulong s = m == 64 ? ulong.MaxValue : (1UL << m) - 1UL;

        // build bit-mask
        using var block = PatternMatchVector.Create(s1);

        var matrix = new List<ulong[]>(s2.Length);
        foreach (var y in s2)
        {
            var M = block.GetOrZero(y)[0];

            ulong u = s & M;

            unchecked { s = (s + u) | (s - u); }
            matrix.Add([s]);
        }

        int sim = CountZeroBits(s, m);
        return (sim, matrix);
    }
}
