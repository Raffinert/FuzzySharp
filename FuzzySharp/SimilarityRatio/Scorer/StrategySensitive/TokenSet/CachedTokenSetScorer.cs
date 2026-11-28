using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenSetScorer(string input1) : CachedTokenSetScorerBase(input1)
{
    protected override FuzzySharp.Scorer Scorer => DefaultRatioStrategy.Calculate;

    internal override bool CanReturnPerfectScore => true;

    internal override double ScorePrefix(string prefix, string combined) =>
        TokenSetScorerHelpers.PrefixRatio(prefix.Length, combined.Length);
}
