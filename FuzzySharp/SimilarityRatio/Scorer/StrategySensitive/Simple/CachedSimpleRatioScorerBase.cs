namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedSimpleRatioScorerBase : CachedStrategySensitiveScorerBase
{
    public override int Score(string input2)
    {
        return Scorer(input2);
    }
}