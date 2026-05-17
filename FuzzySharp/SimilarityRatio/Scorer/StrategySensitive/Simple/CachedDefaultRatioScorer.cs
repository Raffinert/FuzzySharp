using System;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedDefaultRatioScorer : CachedSimpleRatioScorerBase
{
    private readonly ICachedStrategy _strategy;
    private readonly bool _isStrategyOwner;

    public CachedDefaultRatioScorer(string input1, Func<string, string> preprocessor = null)
    {
        _strategy = new CachedDefaultRatioStrategy(input1, preprocessor);
        _isStrategyOwner = true;
    }

    public CachedDefaultRatioScorer(ICachedStrategy strategy, bool isStrategyOwner = false)
    {
        _strategy = strategy;
        _isStrategyOwner = isStrategyOwner;
    }

    protected override CachedScorer Scorer => input2 => _strategy.Calculate(input2);
    public override void Dispose()
    {
        if (_isStrategyOwner)
        {
            _strategy.Dispose();
        }
    }
}