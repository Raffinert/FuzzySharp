using System;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenDifferenceScorer : ICachedRatioScorer
{
    private readonly Func<string, string> _preprocessFactory;
    private readonly CachedDefaultRatioStrategy<string> _scorer;

    public CachedTokenDifferenceScorer(string input1, PreprocessMode preprocess = PreprocessMode.None)
    {
        _preprocessFactory = StringPreprocessorFactory.GetPreprocessor(preprocess);
        var preprocessedInput1 = _preprocessFactory(input1);
        var tokens1 = preprocessedInput1.GetSortedWords();
        _scorer = new CachedDefaultRatioStrategy<string>(tokens1);
    }

    public int Score(string input2)
    {
        input2 = _preprocessFactory(input2);
        var tokens2 = input2.GetSortedWords();
        return _scorer.Calculate(tokens2);
    }

    public void Dispose()
    {
        _scorer.Dispose();
    }
}