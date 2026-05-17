using System;
using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal class CachedDefaultRatioStrategy : ICachedStrategy
{
    private readonly Indel _indel;
    private readonly Func<string, string> _preprocessor;

    public CachedDefaultRatioStrategy(string input1, Func<string, string> preprocessor = null)
    {
        preprocessor ??= StringPreprocessor.None;
        _preprocessor = preprocessor;
        _indel = new Indel(_preprocessor(input1));
    }

    public int Calculate(string input2)
    {
        var  processedInput2 = _preprocessor(input2);
        if (processedInput2.Length == 0)
        {
            return 0;
        }

        return (int)Math.Round(100 * _indel.NormalizedSimilarityWith(processedInput2));
    }

    public void Dispose()
    {
        _indel.Dispose();
    }
}