using System;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenDifferenceScorer : ICachedRatioScorer
{
    private readonly Func<string, string> _preprocessor;
    private readonly CachedDefaultRatioStrategy<string> _scorer;

    public CachedTokenDifferenceScorer(string input1, Func<string, string> preprocessor = null)
    {
        _preprocessor = preprocessor ?? StringPreprocessor.None;
        var preprocessedInput1 = _preprocessor(input1);
        var tokens1 = preprocessedInput1.GetSortedWords();
        _scorer = new CachedDefaultRatioStrategy<string>(tokens1);
    }

    public double Score(string input2)
    {
        input2 = _preprocessor(input2);
        var tokens2 = input2.GetSortedWords();
        return _scorer.Calculate(tokens2);
    }

    public void Dispose()
    {
        _scorer.Dispose();
    }
}
