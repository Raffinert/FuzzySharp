using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenSetScorer(string input1) : CachedTokenSetScorerBase(input1)
{
    protected override FuzzySharp.Scorer Scorer => DefaultRatioStrategy.Calculate;
    public override void Dispose() { }
}