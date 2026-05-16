using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class Levenshtein(ReadOnlySpan<char> source) : IDisposable
{
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source);

    public int DistanceFrom(ReadOnlySpan<char> value)
    {
        return Distance(_patternMatchVector, value);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}