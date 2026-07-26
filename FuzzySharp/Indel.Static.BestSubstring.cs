using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

/// <summary>
/// The result of a directional approximate-substring search.
/// </summary>
public readonly struct IndelSubstringMatch(int distance, int firstBestEndIndex) :
    IEquatable<IndelSubstringMatch>
{
    /// <summary>
    /// Gets the minimum insertion-deletion distance between the complete pattern
    /// and a substring candidate.
    /// </summary>
    public int Distance { get; } = distance;

    /// <summary>
    /// Gets the zero-based endpoint of the first text position whose distance
    /// strictly improved the running best distance. Equal-distance later
    /// endpoints do not replace it. Returns -1 when no text endpoint improves
    /// on deleting the complete pattern.
    /// </summary>
    public int FirstBestEndIndex { get; } = firstBestEndIndex;

    /// <summary>
    /// Gets whether a text endpoint improved on the deletion baseline, whose
    /// distance equals the complete pattern length.
    /// </summary>
    public bool ImprovedOverEmptyMatch => FirstBestEndIndex >= 0;

    public bool Equals(IndelSubstringMatch other)
    {
        return Distance == other.Distance
               && FirstBestEndIndex == other.FirstBestEndIndex;
    }

    public override bool Equals(object obj)
    {
        return obj is IndelSubstringMatch match
               && Equals(match);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Distance * 397) ^ FirstBestEndIndex;
        }
    }

    public static bool operator ==(
        IndelSubstringMatch left,
        IndelSubstringMatch right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        IndelSubstringMatch left,
        IndelSubstringMatch right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Distance = {Distance}, FirstBestEndIndex = {FirstBestEndIndex}";
    }
}

public sealed partial class Indel
{
    /// <summary>
    /// Finds the minimum insertion-deletion distance between the complete
    /// character pattern and any substring of text.
    ///
    /// This operation is directional: <paramref name="pattern"/> is matched in
    /// full against substrings of <paramref name="text"/>. Insertions and
    /// deletions cost one; a substitution costs two. The result's endpoint is
    /// the first strict improvement over the deletion baseline and does not
    /// identify a unique matched substring or start index.
    /// </summary>
    public static IndelSubstringMatch BestSubstringMatch(
        ReadOnlySpan<char> pattern,
        ReadOnlySpan<char> text)
    {
        if (pattern.IsEmpty)
        {
            return new IndelSubstringMatch(
                distance: 0,
                firstBestEndIndex: -1);
        }

        if (text.IsEmpty)
        {
            return new IndelSubstringMatch(
                distance: pattern.Length,
                firstBestEndIndex: -1);
        }

        // Exact ordinal span search is heavily optimized by the runtime and
        // avoids running the more expensive recurrence up to a distant exact
        // occurrence. IndexOf also preserves the earliest-zero tie behavior.
        int exactStart = text.IndexOf(pattern);
        if (exactStart >= 0)
        {
            return new IndelSubstringMatch(
                distance: 0,
                firstBestEndIndex: exactStart + pattern.Length - 1);
        }

        using var patternMatchVector = PatternMatchVector.Create(pattern);
        return BestSubstringMatchImpl(
            patternMatchVector,
            text);
    }

    /// <summary>
    /// Finds the minimum insertion-deletion distance between the complete
    /// pattern and any substring of text.
    ///
    /// This operation is directional: <paramref name="pattern"/> is matched in
    /// full against substrings of <paramref name="text"/>. Insertions and
    /// deletions cost one; a substitution costs two. The result's endpoint is
    /// the first strict improvement over the deletion baseline and does not
    /// identify a unique matched substring or start index.
    /// </summary>
    public static IndelSubstringMatch BestSubstringMatch<T>(
        ReadOnlySpan<T> pattern,
        ReadOnlySpan<T> text)
        where T : notnull, IEquatable<T>
    {
        if (pattern.IsEmpty)
        {
            return new IndelSubstringMatch(
                distance: 0,
                firstBestEndIndex: -1);
        }

        if (text.IsEmpty)
        {
            return new IndelSubstringMatch(
                distance: pattern.Length,
                firstBestEndIndex: -1);
        }

        using var patternMatchVector = PatternMatchVector.Create(pattern);
        return BestSubstringMatchImpl(
            patternMatchVector,
            text);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IndelSubstringMatch BestSubstringMatchImpl<T>(
        IPatternMatchVector<T> patternVector,
        ReadOnlySpan<T> text)
        where T : notnull, IEquatable<T>
    {
        if (patternVector.Blocks == 1)
        {
            return BestSubstringMatchSingleBlock(
                patternVector,
                text);
        }

        return BestSubstringMatchMultipleBlocks(
            patternVector,
            text);
    }

    private static IndelSubstringMatch
        BestSubstringMatchSingleBlock<T>(
            IPatternMatchVector<T> patternVector,
            ReadOnlySpan<T> text)
        where T : notnull, IEquatable<T>
    {
        int patternLength = patternVector.Length;

        ulong vectorMask = patternLength == 64
            ? ulong.MaxValue
            : (1UL << patternLength) - 1UL;

        ulong highestBit =
            1UL << (patternLength - 1);

        ulong positiveVertical = vectorMask;
        ulong negativeVertical = 0;

        int currentDistance = patternLength;
        int bestDistance = patternLength;
        int bestEndIndex = -1;

        unchecked
        {
            for (int textIndex = 0;
                 textIndex < text.Length;
                 textIndex++)
            {
                ulong matchMask =
                    patternVector.GetOrZero(
                        text[textIndex])[0];

                ulong zeroDiagonal =
                    ((((matchMask & positiveVertical)
                       + positiveVertical)
                      ^ positiveVertical)
                     | matchMask
                     | negativeVertical)
                    & vectorMask;

                ulong negativeHorizontal =
                    positiveVertical & zeroDiagonal;

                ulong positiveVerticalMinusNegativeHorizontal =
                    positiveVertical - negativeHorizontal;

                ulong horizontalStarts =
                    (negativeVertical
                     | ~(positiveVertical | zeroDiagonal))
                    & vectorMask;

                ulong horizontalContinuation =
                    positiveVerticalMinusNegativeHorizontal
                    >> 1;

                ulong positiveHorizontal =
                    ((horizontalStarts
                      + horizontalContinuation)
                     ^ horizontalContinuation)
                    & vectorMask;

                // Original approximate-substring boundary:
                // D[0, j] = 0.
                ulong shiftedPositiveHorizontal =
                    (positiveHorizontal << 1)
                    & vectorMask;

                ulong shiftedNegativeHorizontal =
                    (negativeHorizontal << 1)
                    & vectorMask;

                ulong nextNegativeVertical =
                    shiftedPositiveHorizontal
                    & zeroDiagonal;

                ulong nextPositiveVertical =
                    (shiftedNegativeHorizontal
                     | ~(shiftedPositiveHorizontal
                         | zeroDiagonal)
                     | (shiftedPositiveHorizontal
                        & positiveVerticalMinusNegativeHorizontal))
                    & vectorMask;

                if ((positiveHorizontal & highestBit) != 0)
                {
                    currentDistance++;
                }

                if ((negativeHorizontal & highestBit) != 0)
                {
                    currentDistance--;
                }

                positiveVertical =
                    nextPositiveVertical;

                negativeVertical =
                    nextNegativeVertical;

                if (currentDistance < bestDistance)
                {
                    bestDistance = currentDistance;
                    bestEndIndex = textIndex;

                    if (bestDistance == 0)
                    {
                        break;
                    }
                }
            }
        }

        return new IndelSubstringMatch(
            distance: bestDistance,
            firstBestEndIndex: bestEndIndex);
    }


    /// <summary>
    /// Patterns up to this many blocks (2048 symbols) keep their state on the stack.
    /// Two vectors of <see cref="StackBlockLimit"/> ulongs = 512 bytes.
    /// </summary>
    private const int StackBlockLimit = 32;

    private static IndelSubstringMatch BestSubstringMatchMultipleBlocks<T>(
        IPatternMatchVector<T> patternVector,
        ReadOnlySpan<T> text)
        where T : notnull, IEquatable<T>
    {
        int patternLength = patternVector.Length;
        int blockCount = patternVector.Blocks;

        int lastBlockBits = patternLength & 63;
        ulong lastBlockMask = lastBlockBits == 0 ? ulong.MaxValue : (1UL << lastBlockBits) - 1UL;

        // Bit position of the pattern's final character inside the last block.
        int lastRowShift = (patternLength - 1) & 63;

        // Only the two persistent state vectors need storage now: every other
        // quantity lives in registers for exactly as long as it is needed.
        ulong[]? rentedBuffer = null;
        Span<ulong> buffer = blockCount <= StackBlockLimit
                ? stackalloc ulong[StackBlockLimit * 2]
                : rentedBuffer = ArrayPool<ulong>.Shared.Rent(blockCount * 2);

        try
        {
            // Sliced to exactly blockCount so the loop below can be bounded by
            // Length, which lets the JIT drop the bounds checks on these two.
            Span<ulong> positiveVertical = buffer.Slice(0, blockCount);
            Span<ulong> negativeVertical = buffer.Slice(blockCount, blockCount);

            // Vertical deltas in the indel metric are always +/-1, never 0, so
            // Pv | Mv covers the whole pattern and Pv stays masked to it.
            positiveVertical.Fill(ulong.MaxValue);
            positiveVertical[blockCount - 1] = lastBlockMask;
            negativeVertical.Clear();

            int currentDistance = patternLength;
            int bestDistance = patternLength;
            int bestEndIndex = -1;
            int lastBlock = blockCount - 1;

            unchecked
            {
                for (int textIndex = 0; textIndex < text.Length; textIndex++)
                {
                    ReadOnlySpan<ulong> matchMasks = patternVector.GetOrZero(text[textIndex]);
                    Debug.Assert(matchMasks.Length >= blockCount);
                    matchMasks = matchMasks.Slice(0, blockCount);

                    // Carries for the two additions, and the bits shifted out of
                    // the previous block by the two one-bit left shifts.
                    ulong diagonalCarry = 0;
                    ulong horizontalCarry = 0;
                    ulong positiveHorizontalShiftIn = 0;
                    ulong negativeHorizontalShiftIn = 0;

                    // The right shift of (Pv - Mh) needs bit 0 of the *next* block,
                    // so each block is finished one iteration late: these hold the
                    // pending block's inputs.
                    ulong pendingPositiveVertical = positiveVertical[0];
                    ulong pendingNegativeVertical = negativeVertical[0];
                    ulong pendingMatch = matchMasks[0];

                    ulong pendingZeroDiagonal = ZeroDiagonal(
                        pendingMatch,
                        pendingPositiveVertical,
                        pendingNegativeVertical,
                        ref diagonalCarry);

                    // Mh is a bitwise subset of Pv, so Pv - Mh never borrows:
                    // it is exactly Pv & ~D0. The original borrow-propagation
                    // loop was a no-op serial dependency.
                    ulong pendingPositiveVerticalMinusNegativeHorizontal =
                        pendingPositiveVertical & ~pendingZeroDiagonal;

                    for (int block = 1; block < positiveVertical.Length; block++)
                    {
                        ulong positive = positiveVertical[block];
                        ulong negative = negativeVertical[block];
                        ulong match = matchMasks[block];

                        ulong zeroDiagonal = ZeroDiagonal(match, positive, negative, ref diagonalCarry);
                        ulong positiveMinusNegativeHorizontal = positive & ~zeroDiagonal;

                        // Finish block-1 now that its right-shift input is known.
                        // (x & 1) << 63 == x << 63.
                        ulong horizontalContinuation =
                            (pendingPositiveVerticalMinusNegativeHorizontal >> 1) |
                            (positiveMinusNegativeHorizontal << 63);

                        ulong horizontalStarts = pendingNegativeVertical |
                            ~(pendingPositiveVertical | pendingZeroDiagonal);

                        ulong positiveHorizontal = AddWithCarry(
                            horizontalStarts,
                            horizontalContinuation,
                            ref horizontalCarry) ^ horizontalContinuation;

                        ulong negativeHorizontal = pendingPositiveVertical & pendingZeroDiagonal;

                        ulong shiftedPositiveHorizontal =
                            (positiveHorizontal << 1) | positiveHorizontalShiftIn;
                        positiveHorizontalShiftIn = positiveHorizontal >> 63;

                        ulong shiftedNegativeHorizontal =
                            (negativeHorizontal << 1) | negativeHorizontalShiftIn;
                        negativeHorizontalShiftIn = negativeHorizontal >> 63;

                        negativeVertical[block - 1] =
                            shiftedPositiveHorizontal & pendingZeroDiagonal;
                        positiveVertical[block - 1] =
                            shiftedNegativeHorizontal |
                            ~(shiftedPositiveHorizontal | pendingZeroDiagonal) |
                            (shiftedPositiveHorizontal & pendingPositiveVerticalMinusNegativeHorizontal);

                        pendingPositiveVertical = positive;
                        pendingNegativeVertical = negative;
                        pendingZeroDiagonal = zeroDiagonal;
                        pendingPositiveVerticalMinusNegativeHorizontal = positiveMinusNegativeHorizontal;
                    }

                    // Last block: nothing shifts in from above.
                    pendingZeroDiagonal &= lastBlockMask;

                    {
                        // Masking D0 cannot change Pv & ~D0 here, because Pv is
                        // already confined to lastBlockMask.
                        ulong horizontalContinuation =
                            pendingPositiveVerticalMinusNegativeHorizontal >> 1;

                        ulong horizontalStarts = pendingNegativeVertical |
                            ~(pendingPositiveVertical | pendingZeroDiagonal);

                        ulong positiveHorizontal = (AddWithCarry(
                            horizontalStarts,
                            horizontalContinuation,
                            ref horizontalCarry) ^ horizontalContinuation) & lastBlockMask;

                        ulong negativeHorizontal = pendingPositiveVertical & pendingZeroDiagonal;

                        // Branchless: these two bits are near-random, so branching
                        // on them mispredicts on roughly half the text positions.
                        currentDistance += (int)((positiveHorizontal >> lastRowShift) & 1UL)
                                         - (int)((negativeHorizontal >> lastRowShift) & 1UL);

                        ulong shiftedPositiveHorizontal =
                            (positiveHorizontal << 1) | positiveHorizontalShiftIn;
                        ulong shiftedNegativeHorizontal =
                            (negativeHorizontal << 1) | negativeHorizontalShiftIn;

                        negativeVertical[lastBlock] =
                            shiftedPositiveHorizontal & pendingZeroDiagonal & lastBlockMask;
                        positiveVertical[lastBlock] =
                            (shiftedNegativeHorizontal |
                             ~(shiftedPositiveHorizontal | pendingZeroDiagonal) |
                             (shiftedPositiveHorizontal & pendingPositiveVerticalMinusNegativeHorizontal))
                            & lastBlockMask;
                    }

                    if (currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        bestEndIndex = textIndex;

                        if (bestDistance == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return new IndelSubstringMatch(
                distance: bestDistance,
                firstBestEndIndex: bestEndIndex);
        }
        finally
        {
            if (rentedBuffer != null)
            {
                ArrayPool<ulong>.Shared.Return(rentedBuffer);
            }
        }
    }

    /// <summary>D0 = (((match &amp; Pv) + Pv) ^ Pv) | match | Mv, with a cross-block carry.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ZeroDiagonal(ulong match, ulong positive, ulong negative, ref ulong carry)
    {
        ulong sum = AddWithCarry(match & positive, positive, ref carry);
        return (sum ^ positive) | match | negative;
    }

    /// <summary>
    /// Full adder over one block. The carry-out is the majority bit
    /// ((a &amp; b) | ((a | b) &amp; ~sum)) &gt;&gt; 63, which avoids the two dependent
    /// comparisons of a split two-step addition.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong AddWithCarry(ulong left, ulong right, ref ulong carry)
    {
        ulong sum = left + right + carry;
        carry = ((left & right) | ((left | right) & ~sum)) >> 63;
        return sum;
    }

}
