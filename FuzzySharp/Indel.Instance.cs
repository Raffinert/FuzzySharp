using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class Indel(ReadOnlySpan<char> source) : IDisposable
{
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source);
    public int DistanceFrom(ReadOnlySpan<char> value)
    {
        return DistanceImpl(_patternMatchVector, value);
    }

    public double NormalizedSimilarityWith(ReadOnlySpan<char> value)
    {
        return NormalizedSimilarityImpl(_patternMatchVector, value);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}

public sealed class IndelT<T>(ReadOnlySpan<T> source) : IDisposable where T : IEquatable<T>
{
    private readonly IPatternMatchVector<T> _patternMatchVector = PatternMatchVector.Create(source);

    public int DistanceFrom(ReadOnlySpan<T> value)
    {
        return Indel.DistanceImpl<T>(_patternMatchVector, value);
    }

    public double NormalizedSimilarityWith(ReadOnlySpan<T> value)
    {
        return Indel.NormalizedSimilarityImpl(_patternMatchVector, value);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}
