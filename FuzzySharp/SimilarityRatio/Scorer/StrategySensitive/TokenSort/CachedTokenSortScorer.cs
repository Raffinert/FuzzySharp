using Raffinert.FuzzySharp.Extensions;
using System;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenSortScorer : ICachedRatioScorer
{
    private readonly ICachedStrategy _strategy;
    private readonly bool _isStrategyOwner;

    public CachedTokenSortScorer(ReadOnlySpan<char> input1)
    {
        var sorted1 = input1.NormalizeSpacesAndSort();
        _strategy = new CachedDefaultRatioStrategy(sorted1.Span);
        _isStrategyOwner = true;
    }

    public CachedTokenSortScorer(ICachedStrategy strategy, bool isStrategyOwner = false)
    {
        _strategy = strategy;
        _isStrategyOwner = isStrategyOwner;
    }

    public int Score(ReadOnlySpan<char> input2)
    {
        var sorted2 = input2.NormalizeSpacesAndSort();
        return _strategy.Calculate(sorted2.Span);
    }

    public void Dispose()
    {
        if(_isStrategyOwner)
        {
            _strategy.Dispose();
        }
    }
}