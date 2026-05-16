using System;
using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal class CachedDefaultRatioStrategy : ICachedStrategy
{
    private readonly Indel _indel;
    private readonly Processor<char> _preprocessor;

    public CachedDefaultRatioStrategy(ReadOnlySpan<char> input1, Processor<char> preprocessor = null)
    {
        _preprocessor = preprocessor ?? StringPreprocessors.None;
        _preprocessor(ref input1);
        _indel = new Indel(input1);
    }

    public int Calculate(ReadOnlySpan<char> input2)
    {
        _preprocessor(ref input2);
        if (input2.Length == 0)
        {
            return 0;
        }

        return (int)Math.Round(100 * _indel.NormalizedSimilarityWith(input2));
    }

    public void Dispose()
    {
        _indel.Dispose();
    }
}