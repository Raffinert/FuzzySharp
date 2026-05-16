using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal class CachedDefaultRatioStrategy<T>(ReadOnlySpan<T> input1) : IDisposable
    where T : IEquatable<T>
{
    private readonly IndelT<T> _indel = new(input1);
    private readonly int _inputLength = input1.Length;

    public int Calculate(ReadOnlySpan<T> input2)
    {
        if (_inputLength == 0 || input2.Length == 0)
        {
            return 0;
        }

        var result = (int)Math.Round(100 * _indel.NormalizedSimilarityWith(input2));

        return result;
    }

    public void Dispose()
    {
        _indel.Dispose();
    }
}