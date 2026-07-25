using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp;

public readonly struct IndelSubstringMatch(int distance, int endIndex) :
    IEquatable<IndelSubstringMatch>
{
    public int Distance { get; } = distance;

    public int EndIndex { get; } = endIndex;

    public bool Found => EndIndex >= 0;

    public bool Equals(IndelSubstringMatch other)
    {
        return Distance == other.Distance
               && EndIndex == other.EndIndex;
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
            return (Distance * 397) ^ EndIndex;
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
        return $"Distance = {Distance}, EndIndex = {EndIndex}";
    }
}

public sealed partial class Indel
{
    /// <summary>
    /// Finds the minimum insertion-deletion distance between the complete
    /// pattern and any substring of text.
    ///
    /// The returned EndIndex identifies the text position at which the best
    /// approximate substring match ends.
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
                endIndex: -1);
        }

        if (text.IsEmpty)
        {
            return new IndelSubstringMatch(
                distance: pattern.Length,
                endIndex: -1);
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
            endIndex: bestEndIndex);
    }

    //private static IndelSubstringMatch
    //    BestSubstringMatchMultipleBlocks<T>(
    //        IPatternMatchVector<T> patternVector,
    //        ReadOnlySpan<T> text)
    //    where T : notnull, IEquatable<T>
    //{
    //    int patternLength = patternVector.Length;
    //    int blockCount = patternVector.Blocks;
    //    int lastBlockBits = patternLength & 63;
    //    ulong lastBlockMask = lastBlockBits == 0
    //        ? ulong.MaxValue
    //        : (1UL << lastBlockBits) - 1UL;
    //    ulong highestBit = 1UL << ((patternLength - 1) & 63);

    //    // Rent a single contiguous buffer for all 6 vectors instead of 6 separate
    //    // pool allocations. Each vector occupies `blockCount` ulongs.
    //    int totalSize = blockCount * 6;
    //    ulong[] buffer = ArrayPool<ulong>.Shared.Rent(totalSize);

    //    try
    //    {
    //        Span<ulong> positiveVertical = buffer.AsSpan(0, blockCount);
    //        Span<ulong> negativeVertical = buffer.AsSpan(blockCount, blockCount);
    //        Span<ulong> zeroDiagonal = buffer.AsSpan(blockCount * 2, blockCount);
    //        Span<ulong> negativeHorizontal = buffer.AsSpan(blockCount * 3, blockCount);
    //        Span<ulong> positiveVerticalMinusNegativeHorizontal =
    //            buffer.AsSpan(blockCount * 4, blockCount);
    //        Span<ulong> positiveHorizontal = buffer.AsSpan(blockCount * 5, blockCount);

    //        positiveVertical.Fill(ulong.MaxValue);
    //        positiveVertical[blockCount - 1] = lastBlockMask;
    //        negativeVertical.Clear();

    //        int currentDistance = patternLength;
    //        int bestDistance = patternLength;
    //        int bestEndIndex = -1;

    //        unchecked
    //        {
    //            for (int textIndex = 0; textIndex < text.Length; textIndex++)
    //            {
    //                ReadOnlySpan<ulong> matchMasks =
    //                    patternVector.GetOrZero(text[textIndex]);

    //                // zeroDiagonal = (((match & Pv) + Pv) ^ Pv) | match | Mv
    //                ulong carry = 0;
    //                for (int block = 0; block < blockCount; block++)
    //                {
    //                    ulong positive = positiveVertical[block];
    //                    ulong addend = matchMasks[block] & positive;
    //                    ulong sum = addend + positive;
    //                    ulong carryFromAddend = sum < addend ? 1UL : 0UL;
    //                    ulong sumWithCarry = sum + carry;
    //                    carry = carryFromAddend | (sumWithCarry < sum ? 1UL : 0UL);

    //                    zeroDiagonal[block] =
    //                        (sumWithCarry ^ positive)
    //                        | matchMasks[block]
    //                        | negativeVertical[block];
    //                }

    //                zeroDiagonal[blockCount - 1] &= lastBlockMask;

    //                // negativeHorizontal = Pv & zeroDiagonal and
    //                // positiveVerticalMinusNegativeHorizontal = Pv - negativeHorizontal
    //                ulong borrow = 0;
    //                for (int block = 0; block < blockCount; block++)
    //                {
    //                    ulong positive = positiveVertical[block];
    //                    ulong negative = positive & zeroDiagonal[block];
    //                    negativeHorizontal[block] = negative;

    //                    ulong difference = positive - negative;
    //                    ulong borrowFromNegative = positive < negative ? 1UL : 0UL;
    //                    ulong differenceWithBorrow = difference - borrow;
    //                    borrow = borrowFromNegative
    //                             | (difference < borrow ? 1UL : 0UL);
    //                    positiveVerticalMinusNegativeHorizontal[block] =
    //                        differenceWithBorrow;
    //                }

    //                // The right shift crosses block boundaries, so process it
    //                // from the most significant block down.
    //                carry = 0;
    //                for (int block = blockCount - 1; block >= 0; block--)
    //                {
    //                    ulong value = positiveVerticalMinusNegativeHorizontal[block];
    //                    positiveHorizontal[block] =
    //                        (value >> 1) | (carry << 63);
    //                    carry = value & 1UL;
    //                }

    //                carry = 0;
    //                for (int block = 0; block < blockCount; block++)
    //                {
    //                    ulong horizontalStarts =
    //                        negativeVertical[block]
    //                        | ~(positiveVertical[block] | zeroDiagonal[block]);
    //                    ulong continuation = positiveHorizontal[block];
    //                    ulong sum = horizontalStarts + continuation;
    //                    ulong carryFromStarts = sum < horizontalStarts ? 1UL : 0UL;
    //                    ulong sumWithCarry = sum + carry;
    //                    carry = carryFromStarts | (sumWithCarry < sum ? 1UL : 0UL);
    //                    positiveHorizontal[block] = sumWithCarry ^ continuation;
    //                }

    //                positiveHorizontal[blockCount - 1] &= lastBlockMask;

    //                if ((positiveHorizontal[blockCount - 1] & highestBit) != 0)
    //                {
    //                    currentDistance++;
    //                }

    //                if ((negativeHorizontal[blockCount - 1] & highestBit) != 0)
    //                {
    //                    currentDistance--;
    //                }

    //                // Shift horizontal vectors one bit to the left while
    //                // carrying their most significant bits into the next block.
    //                ulong positiveHorizontalCarry = 0;
    //                ulong negativeHorizontalCarry = 0;
    //                for (int block = 0; block < blockCount; block++)
    //                {
    //                    ulong shiftedPositiveHorizontal =
    //                        (positiveHorizontal[block] << 1)
    //                        | positiveHorizontalCarry;
    //                    positiveHorizontalCarry = positiveHorizontal[block] >> 63;

    //                    ulong shiftedNegativeHorizontal =
    //                        (negativeHorizontal[block] << 1)
    //                        | negativeHorizontalCarry;
    //                    negativeHorizontalCarry = negativeHorizontal[block] >> 63;

    //                    ulong positiveVerticalMinusNegative =
    //                        positiveVerticalMinusNegativeHorizontal[block];

    //                    negativeVertical[block] =
    //                        shiftedPositiveHorizontal & zeroDiagonal[block];
    //                    positiveVertical[block] =
    //                        shiftedNegativeHorizontal
    //                        | ~(shiftedPositiveHorizontal | zeroDiagonal[block])
    //                        | (shiftedPositiveHorizontal
    //                           & positiveVerticalMinusNegative);
    //                }

    //                positiveVertical[blockCount - 1] &= lastBlockMask;
    //                negativeVertical[blockCount - 1] &= lastBlockMask;

    //                if (currentDistance < bestDistance)
    //                {
    //                    bestDistance = currentDistance;
    //                    bestEndIndex = textIndex;

    //                    if (bestDistance == 0)
    //                    {
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        return new IndelSubstringMatch(
    //            distance: bestDistance,
    //            endIndex: bestEndIndex);
    //    }
    //    finally
    //    {
    //        ArrayPool<ulong>.Shared.Return(buffer);
    //    }
    //}

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
                endIndex: bestEndIndex);
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

    //private static IndelSubstringMatch BestSubstringMatchMultipleBlocks<T>(
    //    IPatternMatchVector<T> patternVector,
    //    ReadOnlySpan<T> text)
    //    where T : notnull, IEquatable<T>
    //{
    //    int patternLength = patternVector.Length;
    //    int blockCount = patternVector.Blocks;

    //    int lastBlockBits = patternLength & 63;
    //    ulong lastBlockMask = lastBlockBits == 0 ? ulong.MaxValue : (1UL << lastBlockBits) - 1UL;

    //    // Bit position of the pattern's final character inside the last block.
    //    int lastRowShift = (patternLength - 1) & 63;

    //    // Only the two persistent state vectors need storage now: every other
    //    // quantity lives in registers for exactly as long as it is needed.
    //    ulong[]? rentedBuffer = null;
    //    Span<ulong> buffer = blockCount <= StackBlockLimit
    //        ? stackalloc ulong[StackBlockLimit * 2]
    //        : (rentedBuffer = ArrayPool<ulong>.Shared.Rent(blockCount * 2));

    //    try
    //    {
    //        Span<ulong> positiveVertical = buffer.Slice(0, blockCount);
    //        Span<ulong> negativeVertical = buffer.Slice(blockCount, blockCount);

    //        positiveVertical.Fill(ulong.MaxValue);
    //        positiveVertical[blockCount - 1] = lastBlockMask;
    //        negativeVertical.Clear();

    //        // Vertical deltas in the indel metric are always +/-1, never 0, so
    //        // Pv | Mv covers the whole pattern and Pv stays masked to it.
    //        ref ulong positiveVerticalRef = ref MemoryMarshal.GetReference(positiveVertical);
    //        ref ulong negativeVerticalRef = ref MemoryMarshal.GetReference(negativeVertical);

    //        int currentDistance = patternLength;
    //        int bestDistance = patternLength;
    //        int bestEndIndex = -1;
    //        int lastBlock = blockCount - 1;

    //        unchecked
    //        {
    //            for (int textIndex = 0; textIndex < text.Length; textIndex++)
    //            {
    //                ReadOnlySpan<ulong> matchMasks = patternVector.GetOrZero(text[textIndex]);
    //                Debug.Assert(matchMasks.Length >= blockCount);
    //                ref ulong matchRef = ref MemoryMarshal.GetReference(matchMasks);

    //                // Carries for the two additions, and the bits shifted out of
    //                // the previous block by the two one-bit left shifts.
    //                ulong diagonalCarry = 0;
    //                ulong horizontalCarry = 0;
    //                ulong positiveHorizontalShiftIn = 0;
    //                ulong negativeHorizontalShiftIn = 0;

    //                // The right shift of (Pv - Mh) needs bit 0 of the *next* block,
    //                // so each block is finished one iteration late: these hold the
    //                // pending block's inputs.
    //                ulong pendingPositiveVertical = positiveVerticalRef;
    //                ulong pendingNegativeVertical = negativeVerticalRef;
    //                ulong pendingMatch = matchRef;

    //                ulong pendingZeroDiagonal = ZeroDiagonal(
    //                    pendingMatch,
    //                    pendingPositiveVertical,
    //                    pendingNegativeVertical,
    //                    ref diagonalCarry);

    //                // Mh is a bitwise subset of Pv, so Pv - Mh never borrows:
    //                // it is exactly Pv & ~D0. The original borrow-propagation
    //                // loop was a no-op serial dependency.
    //                ulong pendingPositiveVerticalMinusNegativeHorizontal =
    //                    pendingPositiveVertical & ~pendingZeroDiagonal;

    //                for (int block = 1; block < blockCount; block++)
    //                {
    //                    ulong positive = Unsafe.Add(ref positiveVerticalRef, block);
    //                    ulong negative = Unsafe.Add(ref negativeVerticalRef, block);
    //                    ulong match = Unsafe.Add(ref matchRef, block);

    //                    ulong zeroDiagonal = ZeroDiagonal(match, positive, negative, ref diagonalCarry);
    //                    ulong positiveMinusNegativeHorizontal = positive & ~zeroDiagonal;

    //                    // Finish block-1 now that its right-shift input is known.
    //                    // (x & 1) << 63 == x << 63.
    //                    ulong horizontalContinuation =
    //                        (pendingPositiveVerticalMinusNegativeHorizontal >> 1) |
    //                        (positiveMinusNegativeHorizontal << 63);

    //                    ulong horizontalStarts = pendingNegativeVertical |
    //                        ~(pendingPositiveVertical | pendingZeroDiagonal);

    //                    ulong positiveHorizontal = AddWithCarry(
    //                        horizontalStarts,
    //                        horizontalContinuation,
    //                        ref horizontalCarry) ^ horizontalContinuation;

    //                    ulong negativeHorizontal = pendingPositiveVertical & pendingZeroDiagonal;

    //                    ulong shiftedPositiveHorizontal =
    //                        (positiveHorizontal << 1) | positiveHorizontalShiftIn;
    //                    positiveHorizontalShiftIn = positiveHorizontal >> 63;

    //                    ulong shiftedNegativeHorizontal =
    //                        (negativeHorizontal << 1) | negativeHorizontalShiftIn;
    //                    negativeHorizontalShiftIn = negativeHorizontal >> 63;

    //                    Unsafe.Add(ref negativeVerticalRef, block - 1) =
    //                        shiftedPositiveHorizontal & pendingZeroDiagonal;
    //                    Unsafe.Add(ref positiveVerticalRef, block - 1) =
    //                        shiftedNegativeHorizontal |
    //                        ~(shiftedPositiveHorizontal | pendingZeroDiagonal) |
    //                        (shiftedPositiveHorizontal & pendingPositiveVerticalMinusNegativeHorizontal);

    //                    pendingPositiveVertical = positive;
    //                    pendingNegativeVertical = negative;
    //                    pendingZeroDiagonal = zeroDiagonal;
    //                    pendingPositiveVerticalMinusNegativeHorizontal = positiveMinusNegativeHorizontal;
    //                }

    //                // Last block: nothing shifts in from above.
    //                pendingZeroDiagonal &= lastBlockMask;

    //                {
    //                    // Masking D0 cannot change Pv & ~D0 here, because Pv is
    //                    // already confined to lastBlockMask.
    //                    ulong horizontalContinuation =
    //                        pendingPositiveVerticalMinusNegativeHorizontal >> 1;

    //                    ulong horizontalStarts = pendingNegativeVertical |
    //                        ~(pendingPositiveVertical | pendingZeroDiagonal);

    //                    ulong positiveHorizontal = (AddWithCarry(
    //                        horizontalStarts,
    //                        horizontalContinuation,
    //                        ref horizontalCarry) ^ horizontalContinuation) & lastBlockMask;

    //                    ulong negativeHorizontal = pendingPositiveVertical & pendingZeroDiagonal;

    //                    // Branchless: these two bits are near-random, so branching
    //                    // on them mispredicts on roughly half the text positions.
    //                    currentDistance += (int)((positiveHorizontal >> lastRowShift) & 1UL)
    //                                     - (int)((negativeHorizontal >> lastRowShift) & 1UL);

    //                    ulong shiftedPositiveHorizontal =
    //                        (positiveHorizontal << 1) | positiveHorizontalShiftIn;
    //                    ulong shiftedNegativeHorizontal =
    //                        (negativeHorizontal << 1) | negativeHorizontalShiftIn;

    //                    Unsafe.Add(ref negativeVerticalRef, lastBlock) =
    //                        shiftedPositiveHorizontal & pendingZeroDiagonal & lastBlockMask;
    //                    Unsafe.Add(ref positiveVerticalRef, lastBlock) =
    //                        (shiftedNegativeHorizontal |
    //                         ~(shiftedPositiveHorizontal | pendingZeroDiagonal) |
    //                         (shiftedPositiveHorizontal & pendingPositiveVerticalMinusNegativeHorizontal))
    //                        & lastBlockMask;
    //                }

    //                if (currentDistance < bestDistance)
    //                {
    //                    bestDistance = currentDistance;
    //                    bestEndIndex = textIndex;

    //                    if (bestDistance == 0)
    //                    {
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        return new IndelSubstringMatch(
    //            distance: bestDistance,
    //            endIndex: bestEndIndex);
    //    }
    //    finally
    //    {
    //        if (rentedBuffer is not null)
    //        {
    //            ArrayPool<ulong>.Shared.Return(rentedBuffer);
    //        }
    //    }
    //}

    ///// <summary>D0 = (((match &amp; Pv) + Pv) ^ Pv) | match | Mv, with a cross-block carry.</summary>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static ulong ZeroDiagonal(ulong match, ulong positive, ulong negative, ref ulong carry)
    //{
    //    ulong sum = AddWithCarry(match & positive, positive, ref carry);
    //    return (sum ^ positive) | match | negative;
    //}

    ///// <summary>
    ///// Full adder over one block. The carry-out is the majority bit
    ///// ((a &amp; b) | ((a | b) &amp; ~sum)) &gt;&gt; 63, which avoids the two dependent
    ///// comparisons of a split two-step addition.
    ///// </summary>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static ulong AddWithCarry(ulong left, ulong right, ref ulong carry)
    //{
    //    ulong sum = left + right + carry;
    //    carry = ((left & right) | ((left | right) & ~sum)) >> 63;
    //    return sum;
    //}
}
