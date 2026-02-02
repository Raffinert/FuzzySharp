using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class PartialTokenSetScorer : TokenSetScorerBase
{
    protected override FuzzySharp.Scorer Scorer => PartialRatioStrategy.Calculate;
}