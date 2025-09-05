using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class Indel(string source) : IDisposable
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly CharMaskBuffer<char> _charMask = CharMask.Create(source.AsSpan());

    public int DistanceFrom(string value)
    {
        return DistanceImpl(_source.AsSpan(), value.AsSpan(), _charMask);
    }

    public void Dispose()
    {
        _charMask.Dispose();
    }
}
