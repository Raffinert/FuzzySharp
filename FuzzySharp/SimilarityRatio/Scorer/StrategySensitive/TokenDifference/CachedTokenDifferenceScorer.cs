using System;
using Raffinert.FuzzySharp.Extensions;
using System.Linq;
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
        _scorer = new CachedDefaultRatioStrategy<string>(tokens1.Select(m => m.ToString()).ToArray());
    }

    public int Score(ReadOnlySpan<char> input2)
    {
        var span = input2;
        _preprocessor(ref span);
        var tokens2 = span.GetSortedWords();
        return _scorer.Calculate(tokens2.Select(m => m.ToString()).ToArray());
    }

    public void Dispose()
    {
        _scorer.Dispose();
    }
}