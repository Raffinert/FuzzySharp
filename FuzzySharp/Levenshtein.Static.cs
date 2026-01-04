using Raffinert.FuzzySharp.Edits;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Provides static methods for computing the Levenshtein distance and similarity between sequences.
/// Implements bit-parallel and dynamic programming algorithms inspired by RapidFuzz's Levenshtein implementation.
/// </summary>
public sealed partial class Levenshtein
{
    /// <summary>
    /// Computes the Levenshtein distance between two strings with custom operation costs and optional cutoff.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The Levenshtein distance.</returns>
    public static int Distance(
        string source, string target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null)
        => Distance(source.AsSpan(), target.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the Levenshtein distance between two sequences with custom operation costs and optional cutoff.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="source">Source sequence.</param>
    /// <param name="target">Target sequence.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The Levenshtein distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Distance<T>(
        ReadOnlySpan<T> source, ReadOnlySpan<T> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null) where T : IEquatable<T>
    {
        SequenceUtils.TrimCommonAffixAndSwapIfNeeded(ref source, ref target);

        if (insertCost != 1 && deleteCost != 1 && replaceCost is not (1 or 2))
        {
            GenericDistance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff);
        }

        using var charMask = CharMask.Create(source);

        if (replaceCost == 1)
        {
            return scoreCutoff.HasValue
                ? Distance(source, target, scoreCutoff.Value, charMask)
                : Distance(source, target, charMask);
        }

        return Indel.DistanceImpl(source, target, charMask, scoreCutoff);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Distance<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, int scoreCutoff, CharMaskBuffer<T> charMask) where T : IEquatable<T>
    {
        if (source.Length <= 64)
        {
            return DistanceSingleULong(source, target, scoreCutoff, charMask);
        }

        return DistanceMultipleULongs(source, target, scoreCutoff, charMask);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Distance<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, CharMaskBuffer<T> charMask) where T : IEquatable<T>
    {
        if (source.Length <= 64)
        {
            return DistanceSingleULong(source, target, charMask);
        }

        return DistanceMultipleULongs(source, target, null, charMask);
    }

    /// <summary>
    /// Returns a list of matching blocks (contiguous matching subsequences) between s1 and s2 (special case for char spans).
    /// </summary>
    /// <param name="s1">First string.</param>
    /// <param name="s2">Second string.</param>
    /// <returns>List of matching blocks.</returns>
    public static List<MatchingBlock> GetMatchingBlocks(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        var editOps = GetEditOps(s1, s2);
        var matchingBlocks = editOps.AsMatchingBlocks(s1.Length, s2.Length);
        return matchingBlocks;
    }

    /// <summary>
    /// Returns a list of matching blocks (contiguous matching subsequences) between s1 and s2.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">First sequence.</param>
    /// <param name="s2">Second sequence.</param>
    /// <returns>List of matching blocks.</returns>
    public static List<MatchingBlock> GetMatchingBlocks<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        var editOps = GetEditOps(s1, s2);
        var matchingBlocks = editOps.AsMatchingBlocks(s1.Length, s2.Length);
        return matchingBlocks;
    }

    /// <summary>
    /// Computes the sequence of edit operations (insert, delete, replace) to transform s1 into s2 using Levenshtein distance.
    /// </summary>
    /// <param name="s1">Source string.</param>
    /// <param name="s2">Target string.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreHint">Optional score hint (ignored).</param>
    /// <returns>Array of edit operations (EditOp).</returns>
    public static EditOp[] GetEditOps(
        string s1,
        string s2,
        Func<string, string> processor = null,
        int? scoreHint = null
    )
    {
        if (processor != null)
        {
            s1 = processor(s1);
            s2 = processor(s2);
        }

        return GetEditOps(s1.AsSpan(), s2.AsSpan(), scoreHint: scoreHint);
    }

    /// <summary>
    /// Computes the sequence of edit operations (insert, delete, replace) to transform s1 into s2 using Levenshtein distance.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">Source sequence.</param>
    /// <param name="s2">Target sequence.</param>
    /// <param name="processor">Optional preprocessor for normalization.</param>
    /// <param name="scoreHint">Optional score hint (ignored).</param>
    /// <returns>Array of edit operations (EditOp).</returns>
    public static EditOp[] GetEditOps<T>(
        ReadOnlySpan<T> s1,
        ReadOnlySpan<T> s2,
        Processor<T> processor = null,
        int? scoreHint = null
    ) where T : IEquatable<T>
    {
        // 1) Optional preprocessing
        if (processor != null)
        {
            processor(ref s1);
            processor(ref s2);
        }

        // 2) For strings, conv_sequences is identity
        //    (for more general sequences you'd map items to ints)
        // 3) Strip off common prefix+suffix
        var (prefixLen, suffixLen) = SequenceUtils.TrimCommonAffix(ref s1, ref s2);

        // 4) Run the bit-parallel matrix
        var (dist, VPblocks, VNblocks) = Matrix(s1, s2);

        // 5) Initialize backtracking
        var originalDist = dist;
        var opsArray = new EditOp[originalDist];
        var col = s1.Length;
        var row = s2.Length;
        var nextIndex = originalDist; // we’ll decrement before placing

        // 6) If no edits, we’re done
        if (originalDist == 0)
            return [];

        // 7) Backtrack
        while (row > 0 && col > 0)
        {
            // determine which block & bit offset holds the (col-1) bit
            var bitPos = col - 1;
            var blk = bitPos / 64;
            var off = bitPos % 64;
            var bit = 1UL << off;

            // deletion?
            if ((VPblocks[row - 1][blk] & bit) != 0)
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
                row--;
                // insertion?
                if (row > 0 && (VNblocks[row - 1][blk] & bit) != 0)
                {
                    nextIndex--;
                    opsArray[nextIndex] = new EditOp
                    {
                        EditType = EditType.INSERT,
                        SourcePos = col + prefixLen,
                        DestPos = row + prefixLen
                    };
                }
                else
                {
                    // move diagonally
                    col--;
                    // replace?
                    if (!EqualityComparer<T>.Default.Equals(s1[col], s2[row]))
                    {
                        nextIndex--;
                        opsArray[nextIndex] = new EditOp
                        {
                            EditType = EditType.REPLACE,
                            SourcePos = col + prefixLen,
                            DestPos = row + prefixLen
                        };
                    }
                }
            }
        }

        // any remaining deletes
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
        // any remaining inserts
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
    /// Computes the maximum possible Levenshtein distance between two sequences given the operation costs.
    /// </summary>
    /// <param name="len1">Length of the first sequence.</param>
    /// <param name="len2">Length of the second sequence.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <returns>The maximum possible Levenshtein distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LevenshteinMaximum(int len1, int len2, int insertCost, int deleteCost, int replaceCost)
    {
        // Cost of deleting all of s1 then inserting all of s2
        var totalDelIns = len1 * deleteCost + len2 * insertCost;

        // Cost of replacing common prefix and handling extra characters
        var common = len1 < len2 ? len1 : len2;
        var extraCost = len1 >= len2
            ? len1 - len2 * deleteCost
            : len2 - len1 * insertCost;

        var replaceAndDiff = common * replaceCost + extraCost;

        // Return the smaller of the two worst-case scenarios
        return totalDelIns < replaceAndDiff ? totalDelIns : replaceAndDiff;
    }

    /// <summary>
    /// Computes the Myers bit-parallel VP/VN matrices and final edit distance for patterns of arbitrary length.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="pattern">Pattern sequence (any length).</param>
    /// <param name="text">Text sequence.</param>
    /// <returns>Tuple of (distance, VP matrix, VN matrix).</returns>
    public static (int Distance, List<ulong[]> VP, List<ulong[]> VN) MatrixMultipleULongs<T>(ReadOnlySpan<T> pattern, ReadOnlySpan<T> text) where T : IEquatable<T>
    {
        var m = pattern.Length;
        if (m == 0)
            return (text.Length, new List<ulong[]>(), new List<ulong[]>());

        // Number of 64‐bit blocks needed to cover the pattern
        var blocks = (m + 63) / 64;

        // Initial VP = all 1s in those m bits, VN = 0
        var VP = new ulong[blocks];
        var VN = new ulong[blocks];
        for (var i = 0; i < blocks; i++)
        {
            // For all but the last block, fill with 0xFFFFFFFFFFFFFFFF
            // For the last block, only the low (m % 64) bits are 1
            if (i < blocks - 1 || m % 64 == 0)
                VP[i] = ulong.MaxValue;
            else
                VP[i] = (1UL << (m % 64)) - 1;
            VN[i] = 0UL;
        }

        // Mask to extract the highest‐order bit of the full m‐bit vector
        var lastBlk = blocks - 1;
        var topBitPos = (m - 1) % 64;
        var topBitMask = 1UL << topBitPos;

        // Build the “block” table: for each character, which bit(s) in each block it sets
        using var blockTable = CharMask.Create(pattern);

        var currDist = m;
        var matrixVP = new List<ulong[]>();
        var matrixVN = new List<ulong[]>();

        // Temporary arrays for per‐character computation
        var D0 = new ulong[blocks];
        var HP = new ulong[blocks];
        var HN = new ulong[blocks];
        var sum = new ulong[blocks];
        var HPs = new ulong[blocks];
        var HNs = new ulong[blocks];

        // Process each character of the text
        foreach (var c in text)
        {
            // 1) Load the pattern‐mask for c, or zeros if not present
            var X = blockTable.GetOrZero(c);

            // 2) Compute D0 = (((X & VP) + VP) ^ VP) | X | VN
            //    -> Must do a big‐integer add and carry across blocks
            ulong carry = 0;
            for (var b = 0; b < blocks; b++)
            {
                var Pv = VP[b];
                var XandVP = X[b] & Pv;
                // big‐integer add: XandVP + Pv + carry
                var t = unchecked(XandVP + Pv);
                var c1 = t < XandVP ? 1UL : 0UL;          // carry from first add
                var t2 = unchecked(t + carry);
                var c2 = t2 < carry ? 1UL : 0UL;           // carry from second add
                carry = c1 | c2;

                sum[b] = t2;
                D0[b] = (sum[b] ^ Pv) | X[b] | VN[b];
            }

            // 3) HP = VN | ~(D0 | VP),   HN = D0 & VP
            for (var b = 0; b < blocks; b++)
            {
                HP[b] = VN[b] | ~(D0[b] | VP[b]);
                HN[b] = D0[b] & VP[b];
            }

            // 4) Update distance by inspecting the highest bit
            if ((HP[lastBlk] & topBitMask) != 0) currDist++;
            if ((HN[lastBlk] & topBitMask) != 0) currDist--;

            // 5) Shift HP and HN left by one over the entire multi‐block vector
            //    and set the low bit of HP[0] to 1
            ulong carryHP = 1, carryHN = 0;
            for (var b = 0; b < blocks; b++)
            {
                ulong hpb = HP[b], hnb = HN[b];
                var newCarryHP = hpb >> 63;
                var newCarryHN = hnb >> 63;
                HPs[b] = (hpb << 1) | carryHP;
                HNs[b] = (hnb << 1) | carryHN;
                carryHP = newCarryHP;
                carryHN = newCarryHN;
            }

            // 6) Recompute VP, VN
            for (var b = 0; b < blocks; b++)
            {
                VP[b] = HNs[b] | ~(D0[b] | HPs[b]);
                VN[b] = HPs[b] & D0[b];
            }

            // 7) Keep a snapshot of VP/VN for this character
            matrixVP.Add((ulong[])VP.Clone());
            matrixVN.Add((ulong[])VN.Clone());
        }

        return (currDist, matrixVP, matrixVN);
    }

    /// <summary>
    /// Computes the Myers bit-parallel VP/VN matrices and final edit distance for patterns up to 64 elements.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">Pattern sequence (≤ 64 elements).</param>
    /// <param name="s2">Text sequence.</param>
    /// <returns>Tuple of (distance, VP matrix, VN matrix).</returns>
    public static (int Distance, List<ulong[]> VP, List<ulong[]> VN) MatrixSingleULong<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        if (s1.IsEmpty)
            return (s2.Length, [], []);

        if (s1.Length > 64)
            throw new ArgumentException("Pattern too long for 64-bit bit-parallel algorithm.", nameof(s1));

        // Initial bitmasks
        var VP = (1UL << s1.Length) - 1;
        ulong VN = 0;
        var currDist = s1.Length;
        var mask = 1UL << (s1.Length - 1);

        // Build the “block” table: for each character in s1, which bit(s) it sets
        using var blockTable = CharMask.Create(s1);

        var matrixVP = new List<ulong[]>();
        var matrixVN = new List<ulong[]>();

        foreach (var c in s2)
        {
            var PMj = blockTable.GetOrZero(c)[0];

            // Step 1: D0 = (((PMj & VP) + VP) ^ VP) | PMj | VN
            // Use unchecked so addition wraps modulo 2^64
            var X = PMj;
            var D0 = unchecked(((X & VP) + VP) ^ VP) | X | VN;

            // Step 2: HP = VN | ~(D0 | VP);  HN = D0 & VP
            var HP = VN | ~(D0 | VP);
            var HN = D0 & VP;

            // Step 3: adjust distance by looking at the high bit
            if ((HP & mask) != 0) currDist++;
            if ((HN & mask) != 0) currDist--;

            // Step 4: shift and recompute VP, VN
            HP = (HP << 1) | 1UL;
            HN <<= 1;
            VP = HN | ~(D0 | HP);
            VN = HP & D0;

            matrixVP.Add([VP]);
            matrixVN.Add([VN]);
        }

        return (currDist, matrixVP, matrixVN);
    }

    /// <summary>
    /// Computes the normalized Levenshtein distance in [0, 1].
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold.</param>
    /// <returns>Normalized distance (0 = identical, 1 = completely different).</returns>
    public static double NormalizedDistance(
        ReadOnlySpan<char> source, ReadOnlySpan<char> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
    {
        int len1 = source.Length, len2 = target.Length;
        if (len1 == 0 && len2 == 0) return 0.0;
        var maximum = LevenshteinMaximum(len1, len2, insertCost, deleteCost, replaceCost);
        if (maximum == 0) return 0.0;

        var dist = Distance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff.HasValue ? (int?)Math.Floor(scoreCutoff.Value * maximum) : null);
        var nd = dist / (double)maximum;
        return nd > scoreCutoff ? 1.0 : nd;
    }

    /// <summary>
    /// Computes the normalized Levenshtein distance for two strings.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum normalized distance threshold.</param>
    /// <returns>Normalized distance (0 = identical, 1 = completely different).</returns>
    public static double NormalizedDistance(
        string source, string target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
        => NormalizedDistance(source.AsSpan(), target.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the normalized Levenshtein similarity in [0, 1] (1 - normalized distance).
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum normalized similarity threshold.</param>
    /// <returns>Normalized similarity (1 = identical, 0 = completely different).</returns>
    public static double NormalizedSimilarity(
        ReadOnlySpan<char> source, ReadOnlySpan<char> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
    {
        var nd = NormalizedDistance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff);
        var ns = 1.0 - nd;

        return ns < scoreCutoff ? 0.0 : ns;
    }

    /// <summary>
    /// Computes the normalized Levenshtein similarity for two strings.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum normalized similarity threshold.</param>
    /// <returns>Normalized similarity (1 = identical, 0 = completely different).</returns>
    public static double NormalizedSimilarity(
        string source, string target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        double? scoreCutoff = null)
        => NormalizedSimilarity(source.AsSpan(), target.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the Levenshtein similarity (maximum possible distance minus actual distance).
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="target">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold.</param>
    /// <returns>The Levenshtein similarity score.</returns>
    public static int Similarity(
        ReadOnlySpan<char> source, ReadOnlySpan<char> target,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null)
    {
        int len1 = source.Length, len2 = target.Length;
        var maximum = LevenshteinMaximum(len1, len2, insertCost, deleteCost, replaceCost);
        var dist = Distance(source, target, insertCost, deleteCost, replaceCost, scoreCutoff);
        var sim = maximum - dist;
        return sim < scoreCutoff ? 0 : sim;
    }

    /// <summary>
    /// Computes the Levenshtein similarity for two strings.
    /// </summary>
    /// <param name="source">Source string.</param>
    /// <param name="s2">Target string.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional minimum similarity threshold.</param>
    /// <returns>The Levenshtein similarity score.</returns>
    public static int Similarity(
        string source, string s2,
        int insertCost = 1, int deleteCost = 1, int replaceCost = 1,
        int? scoreCutoff = null)
        => Similarity(source.AsSpan(), s2.AsSpan(), insertCost, deleteCost, replaceCost, scoreCutoff);

    /// <summary>
    /// Computes the Levenshtein distance between two sequences with custom operation costs using a dynamic programming approach.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="source">Source sequence.</param>
    /// <param name="target">Target sequence.</param>
    /// <param name="insertCost">Cost of an insertion.</param>
    /// <param name="deleteCost">Cost of a deletion.</param>
    /// <param name="replaceCost">Cost of a replacement.</param>
    /// <param name="scoreCutoff">Optional maximum distance threshold.</param>
    /// <returns>The Levenshtein distance.</returns>
    private static int GenericDistance<T>(
        ReadOnlySpan<T> source, ReadOnlySpan<T> target,
        int insertCost, int deleteCost, int replaceCost,
        int? scoreCutoff) where T : IEquatable<T>
    {
        var len1 = source.Length;
        // allocate a single row of len1+1
        Span<int> row = new int[len1 + 1];
        // initial: cost of deleting all of s1's prefix
        for (var i = 0; i <= len1; i++)
            row[i] = i * deleteCost;

        foreach (var c2 in target)
        {
            var prev = row[0];
            row[0] += insertCost;
            for (var i = 0; i < len1; i++)
            {
                var curr = row[i + 1];
                var cost = prev;
                if (!EqualityComparer<T>.Default.Equals(source[i], c2))
                {
                    var del = row[i] + deleteCost;
                    var ins = row[i + 1] + insertCost;
                    var rep = prev + replaceCost;
                    cost = del < ins
                        ? del < rep ? del : rep
                        : ins < rep ? ins : rep;
                }
                prev = curr;
                row[i + 1] = cost;
            }
            if (scoreCutoff.HasValue && row[len1] > scoreCutoff.Value)
                return scoreCutoff.Value + 1;
        }
        return row[len1];
    }

    /// <summary>
    /// Computes the Myers bit-parallel VP/VN matrices and final edit distance, dispatching to the appropriate implementation.
    /// </summary>
    /// <typeparam name="T">Element type, must implement IEquatable&lt;T&gt;.</typeparam>
    /// <param name="s1">Pattern sequence.</param>
    /// <param name="s2">Text sequence.</param>
    /// <returns>Tuple of (distance, VP matrix, VN matrix).</returns>
    private static (int Distance, List<ulong[]> VP, List<ulong[]> VN) Matrix<T>(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2) where T : IEquatable<T>
    {
        return s1.Length <= 64
            ? MatrixSingleULong(s1, s2)
            : MatrixMultipleULongs(s1, s2);
    }

    /// <summary>
    /// Computes the Levenshtein distance (Myers’s bit‐parallel over >64 bits), with an optional cutoff.
    /// Uses a dictionary to store per‐character bitmasks rented from ArrayPool, and uses stackalloc if
    /// 6*blocks ≤ STACKALLOC_THRESHOLD_ULONGS; otherwise allocates a new ulong[] on the heap for the six lanes.
    /// </summary>
    private static int DistanceMultipleULongs<T>(
        ReadOnlySpan<T> source,
        ReadOnlySpan<T> target,
        int? scoreCutoff,
        CharMaskBuffer<T> charMask
    ) where T : IEquatable<T>
    {
        var m = source.Length;
        if (m == 0)
        {
            var d = target.Length;
            return d > scoreCutoff
                ? scoreCutoff.Value + 1
                : d;
        }

        // Number of 64‐bit blocks needed to cover pattern length m
        var blocks = (m + 63) >> 6;
        var totalScratch = 6 * blocks;
        var scratchArray = ArrayPool<ulong>.Shared.Rent(totalScratch);
        try
        {
            var result = DistanceMultipleULongsImpl(target, scoreCutoff, m, blocks, charMask, scratchArray);
            return result;
        }
        finally
        {
            ArrayPool<ulong>.Shared.Return(scratchArray);
        }
    }


    // ─────────────────────────────────────────────────────────────────────────────
    // Shared implementation that uses the precomputed dictionary of masks.
    // 'scratch' must have Length == 6 * blocks. It is partitioned into six lanes:
    //   scratch[0..blocks)       → VP
    //   scratch[blocks..2*blocks)→ VN
    //   scratch[2*blocks..3*blocks)→ X
    //   scratch[3*blocks..4*blocks)→ D0
    //   scratch[4*blocks..5*blocks)→ HP
    //   scratch[5*blocks..6*blocks)→ HN
    // ─────────────────────────────────────────────────────────────────────────────
    private static int DistanceMultipleULongsImpl<T>(ReadOnlySpan<T> target,
        int? scoreCutoff,
        int m,
        int blocks,
        CharMaskBuffer<T> charMask,
        Span<ulong> scratch
    ) where T : IEquatable<T>
    {
        // Partition scratch into six spans of length = blocks
        var VP = scratch.Slice(0 * blocks, blocks);
        var VN = scratch.Slice(1 * blocks, blocks);
        var X = scratch.Slice(2 * blocks, blocks);
        var D0 = scratch.Slice(3 * blocks, blocks);
        var HP = scratch.Slice(4 * blocks, blocks);
        var HN = scratch.Slice(5 * blocks, blocks);

        // Initialize VP (low m bits = 1) and VN = 0
        for (var b = 0; b < blocks; b++)
        {
            if (b < blocks - 1)
            {
                VP[b] = ulong.MaxValue;
            }
            else
            {
                var rem = m - ((blocks - 1) << 6);
                VP[b] = (rem == 64) ? ulong.MaxValue : ((1UL << rem) - 1);
            }
            VN[b] = 0UL;
        }

        var last = blocks - 1;
        var highestBitMask = 1UL << ((m - 1) & 63);
        var dist = m;

        foreach (var c2 in target)
        {
            // Look up the precomputed bitmask array, or use zeroMask if not found
            var PMitem = charMask.GetOrZero(c2);

            // “D0‐loop” with carry across blocks
            var carry = 0UL;
            for (var b = 0; b < blocks; b++)
            {
                var pm = PMitem[b];
                var vp = VP[b];
                var vn = VN[b];
                var x = pm | vn;
                X[b] = x;
                var tmp = x & vp;

                // tmp + vp
                var sum1 = tmp + vp;
                var c1 = (sum1 < tmp) ? 1UL : 0UL;
                // sum1 + carry
                var sum = sum1 + carry;
                var c2o = (sum < sum1) ? 1UL : 0UL;
                carry = c1 | c2o;

                // D0 = (sum ^ vp) | x
                var d0 = (sum ^ vp) | x;
                D0[b] = d0;

                // HP = vn | ~(d0 | vp)
                // HN = d0 & vp
                HP[b] = vn | ~(d0 | vp);
                HN[b] = d0 & vp;
            }

            // Update distance by checking top bit of last block
            if ((HP[last] & highestBitMask) != 0UL) dist++;
            if ((HN[last] & highestBitMask) != 0UL) dist--;
            if (scoreCutoff.HasValue && dist > scoreCutoff.Value)
            {
                return scoreCutoff.Value + 1;
            }

            // Shift HP/HN left by 1 (with cross‐block carry), then compute new VP/VN
            var carryHP = 1UL;
            var carryHN = 0UL;
            for (var b = 0; b < blocks; b++)
            {
                var hp = HP[b];
                var hn = HN[b];

                var hpHigh = hp >> 63;
                var hnHigh = hn >> 63;

                hp = (hp << 1) | carryHP;
                hn = (hn << 1) | carryHN;

                var d0 = D0[b];
                VP[b] = hn | ~(d0 | hp);
                VN[b] = hp & d0;

                carryHP = hpHigh;
                carryHN = hnHigh;
            }
        }

        return dist;
    }

    private static int DistanceSingleULong<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, int scoreCutoff, CharMaskBuffer<T> charMask) where T : IEquatable<T>
    {
        var m = source.Length;
        if (m == 0) return target.Length;

        // initial bitmask: lower m bits set
        var VP = m < 64 ? (1UL << m) - 1 : ulong.MaxValue;
        ulong VN = 0;
        var highestBit = 1UL << (m - 1);
        var dist = m;

        foreach (var c2 in target)
        {
            var PM = charMask.GetOrZero(c2)[0];

            // Myers bit-parallel update
            var X = PM | VN;
            var D0 = (((X & VP) + VP) ^ VP) | X;
            D0 |= VN;
            var HP = VN | ~(D0 | VP);
            var HN = D0 & VP;

            if ((HP & highestBit) != 0) dist++;
            if ((HN & highestBit) != 0) dist--;

            if (dist > scoreCutoff)
                return scoreCutoff + 1;

            // shift in
            HP = (HP << 1) | 1;
            HN <<= 1;
            VP = HN | ~(D0 | HP);
            VN = HP & D0;
        }

        return dist;
    }

    private static int DistanceSingleULong<T>(ReadOnlySpan<T> source, ReadOnlySpan<T> target, CharMaskBuffer<T> charMask) where T : IEquatable<T>
    {
        var m = source.Length;
        if (m == 0) return target.Length;

        // initial bitmask: lower m bits set
        var VP = m < 64 ? (1UL << m) - 1 : ulong.MaxValue;
        ulong VN = 0;
        var highestBit = 1UL << (m - 1);
        var dist = m;

        foreach (var c2 in target)
        {
            var PM = charMask.GetOrZero(c2)[0];

            // Myers bit-parallel update
            var X = PM | VN;
            var D0 = (((X & VP) + VP) ^ VP) | X;
            D0 |= VN;
            var HP = VN | ~(D0 | VP);
            var HN = D0 & VP;

            if ((HP & highestBit) != 0) dist++;
            if ((HN & highestBit) != 0) dist--;

            // shift in
            HP = (HP << 1) | 1;
            HN <<= 1;
            VP = HN | ~(D0 | HP);
            VN = HP & D0;
        }

        return dist;
    }
}