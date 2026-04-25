namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedSimpleRatioScorerBase : ICachedRatioScorer
{
    protected abstract CachedScorer Scorer { get; }
    public abstract void Dispose();

    public int Score(string input2)
    {
        return Scorer(input2);
    }
}