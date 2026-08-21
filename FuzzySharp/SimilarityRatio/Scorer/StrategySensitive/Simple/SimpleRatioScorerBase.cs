namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class SimpleRatioScorerBase : StrategySensitiveScorerBase
{
    public override double Score(string input1, string input2)
    {
        return Scorer(input1, input2);
    }
}
