using System;

namespace Raffinert.FuzzySharp.Utils;

public sealed class CharMask
{
    public static CharMaskBuffer<T> Create<T>(ReadOnlySpan<T> source) where T : notnull, IEquatable<T>
    {
        var blocks = (source.Length + 63) >> 6;

        var charMask = new CharMaskBuffer<T>(64, blocks);
        int i = 0;

        foreach (var item in source)
        {
            charMask.AddBit(item, i++);
        }

        return charMask;
    }
}