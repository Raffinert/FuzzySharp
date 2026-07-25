using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class ApproximateSubstringRatioScorer : SimpleRatioScorerBase
{
    protected override FuzzySharp.Scorer Scorer =>
        ApproximateSubstringRatioStrategy.Calculate;
}
