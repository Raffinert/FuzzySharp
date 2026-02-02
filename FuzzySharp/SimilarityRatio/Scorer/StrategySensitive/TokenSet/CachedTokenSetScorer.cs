using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class CachedTokenSetScorer : CachedTokenSetScorerBase
{
    public CachedTokenSetScorer(string input1) : base(input1)
    {
    }

    public CachedTokenSetScorer(CachedTokenSetScorerBase other)
    {
        Tokens1 = other.Tokens1;
    }

    protected override FuzzySharp.Scorer Scorer => DefaultRatioStrategy.Calculate;
    public override void Dispose()
    {   
    }
}