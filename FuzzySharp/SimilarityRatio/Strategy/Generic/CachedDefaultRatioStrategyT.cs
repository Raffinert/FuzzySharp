using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal class CachedDefaultRatioStrategy<T>(T[] input1) : IDisposable
    where T : IEquatable<T>
{
    private readonly IndelT<T> _indel = new(input1);

    public double Calculate(T[] input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var result = 100 * _indel.NormalizedSimilarityWith(input2);

        return result;
    }

    public void Dispose()
    {
        _indel.Dispose();
    }
}
