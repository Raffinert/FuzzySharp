namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public abstract class CachedScorerBase : ICachedRatioScorer
{
    public abstract int Score(string input2);
    public abstract void Dispose();
}