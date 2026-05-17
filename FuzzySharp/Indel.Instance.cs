using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class Indel(string source) : IDisposable
{
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(string value)
    {
        return DistanceImpl(_patternMatchVector, value.AsSpan());
    }

    public double NormalizedSimilarityWith(string value)
    {
        return NormalizedSimilarityImpl(_patternMatchVector, value.AsSpan());
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}

public sealed class IndelT<T>(T[] source) : IDisposable where T : IEquatable<T>
{
    private readonly IPatternMatchVector<T> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(T[] value)
    {
        return Indel.DistanceImpl(_patternMatchVector, value.AsSpan());
    }

    public double NormalizedSimilarityWith(T[] value)
    {
        return Indel.NormalizedSimilarityImpl(_patternMatchVector, value.AsSpan());
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}
