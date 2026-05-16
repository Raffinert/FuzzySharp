namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

using System;

public abstract class CachedSimpleRatioScorerBase : ICachedRatioScorer
{
    protected abstract CachedScorer Scorer { get; }
    public abstract void Dispose();

    public int Score(ReadOnlySpan<char> input2)
    {
        return Scorer(input2);
    }
}