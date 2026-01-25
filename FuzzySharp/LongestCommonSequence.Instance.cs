using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp;

public sealed partial class LongestCommonSequence(string source) : IDisposable
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly IPatternMatchVector<char> _patternMatchVector = PatternMatchVector.Create(source.AsSpan());

    public int DistanceFrom(string value)
    {
        return DistanceImpl(_source.AsSpan(), value.AsSpan(), _patternMatchVector);
    }

    public void Dispose()
    {
        _patternMatchVector.Dispose();
    }
}