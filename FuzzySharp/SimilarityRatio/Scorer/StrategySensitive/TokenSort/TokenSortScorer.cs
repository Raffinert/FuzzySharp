using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class TokenSortScorer : TokenSortScorerBase
{
    protected override FuzzySharp.Scorer Scorer => DefaultRatioStrategy.Calculate;
}