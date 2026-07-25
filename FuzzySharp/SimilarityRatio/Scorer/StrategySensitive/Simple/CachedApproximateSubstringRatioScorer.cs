using System;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedApproximateSubstringRatioScorer : CachedSimpleRatioScorerBase
{
    private readonly ICachedStrategy _strategy;
    private readonly bool _isStrategyOwner;
    private bool _disposed;

    public CachedApproximateSubstringRatioScorer(
        string input1,
        Func<string, string> preprocessor = null)
    {
        _strategy = new CachedApproximateSubstringRatioStrategy(input1, preprocessor);
        _isStrategyOwner = true;
    }

    public CachedApproximateSubstringRatioScorer(
        ICachedStrategy strategy,
        bool isStrategyOwner = false)
    {
        _strategy = strategy;
        _isStrategyOwner = isStrategyOwner;
    }

    protected override CachedScorer Scorer => input2 => _strategy.Calculate(input2);

    public override void Dispose()
    {
        if (_isStrategyOwner && !_disposed)
        {
            _strategy.Dispose();
            _disposed = true;
        }
    }
}
