using System;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

internal static class Polyfill
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong ReverseBits(ulong value)
    {
        value = ((value >> 1) & 0x5555555555555555UL) | ((value & 0x5555555555555555UL) << 1);
        value = ((value >> 2) & 0x3333333333333333UL) | ((value & 0x3333333333333333UL) << 2);
        value = ((value >> 4) & 0x0F0F0F0F0F0F0F0FUL) | ((value & 0x0F0F0F0F0F0F0F0FUL) << 4);
        return System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ulong value)
    {

#if NET6_0_OR_GREATER
        return System.Numerics.BitOperations.PopCount(value);
#else
        value -= value >> 1 & 6148914691236517205UL /*0x5555555555555555*/;
        value = (ulong)(((long)value & 3689348814741910323L /*0x3333333333333333*/) + ((long)(value >> 2) & 3689348814741910323L /*0x3333333333333333*/));
        value = (ulong)(((long)value + (long)(value >> 4) & 1085102592571150095L) * 72340172838076673L >>> 56);
        return (int)value;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ArrayFill<T>(T[] array, T value, int startIndex, int count)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        Array.Fill(array, value, startIndex, count);
#else
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if ((uint)startIndex > (uint)array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if ((uint)count > (uint)(array.Length - startIndex))
            throw new ArgumentOutOfRangeException(nameof(count));

        for (var i = startIndex; i < startIndex + count; i++)
        {
            array[i] = value;
        }
#endif
    }
}
