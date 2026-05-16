using System;
using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenDifferenceScorer : ICachedRatioScorer
{
    private readonly Processor<char> _preprocessor;
    private readonly CachedDefaultRatioStrategy<string> _scorer;

    public CachedTokenDifferenceScorer(string input1, Processor<char> preprocessor = null)
    {
        _preprocessor = preprocessor ?? StringPreprocessors.None;
        var span = input1.AsSpan();
        _preprocessor(ref span);
        var tokens1 = span.GetSortedWords();
        _scorer = new CachedDefaultRatioStrategy<string>(tokens1);
    }

    public int Score(ReadOnlySpan<char> input2)
    {
        _preprocessor(ref input2);
        var tokens2 = input2.GetSortedWords();
        return _scorer.Calculate(tokens2);
    }

    public void Dispose()
    {
        _scorer.Dispose();
    }
}