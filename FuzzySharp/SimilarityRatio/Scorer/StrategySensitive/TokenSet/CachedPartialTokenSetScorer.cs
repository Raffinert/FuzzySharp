using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedPartialTokenSetScorer : CachedTokenSetScorerBase
{
    public CachedPartialTokenSetScorer(string input1) : base(input1)
    {
    }

    public CachedPartialTokenSetScorer(CachedTokenSetScorerBase other)
    {
        Tokens1 = other.Tokens1;
    }

    protected override FuzzySharp.Scorer Scorer => PartialRatioStrategy.Calculate;
    public override void Dispose()
    {
    }
}