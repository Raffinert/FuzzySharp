using System;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

/// <summary>
/// Caches a query for repeated approximate-substring scoring. Concurrent calls
/// to <see cref="CachedSimpleRatioScorerBase.Score"/> are supported before
/// disposal; concurrent scoring and disposal are not supported.
/// </summary>
public sealed class CachedApproximateSubstringRatioScorer : CachedSimpleRatioScorerBase
{
    private readonly ICachedStrategy _strategy;
    private readonly bool _isStrategyOwner;
    private bool _disposed;

    /// <summary>
    /// Initializes an owned cached strategy for <paramref name="input1"/>.
    /// Dispose this scorer when it is no longer needed; disposal is idempotent.
    /// </summary>
    public CachedApproximateSubstringRatioScorer(
        string input1,
        Func<string, string> preprocessor = null)
    {
        _strategy = new CachedApproximateSubstringRatioStrategy(input1, preprocessor);
        _isStrategyOwner = true;
    }

    internal CachedApproximateSubstringRatioScorer(
        ICachedStrategy strategy,
        bool isStrategyOwner = false)
    {
        _strategy = strategy;
        _isStrategyOwner = isStrategyOwner;
    }

    protected override CachedScorer Scorer => Calculate;

    public override void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_isStrategyOwner)
        {
            _strategy.Dispose();
        }

        _disposed = true;
    }

    private int Calculate(string input2)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(CachedApproximateSubstringRatioScorer));
        }

        return _strategy.Calculate(input2);
    }
}
