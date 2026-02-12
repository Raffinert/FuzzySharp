namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedStrategySensitiveScorerBase : CachedScorerBase
{
    protected abstract CachedScorer Scorer { get; }
}