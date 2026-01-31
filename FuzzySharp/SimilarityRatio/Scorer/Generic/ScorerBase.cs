using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

public abstract class ScorerBase<T> : IRatioScorer<T> where T : IEquatable<T>
{
    public abstract int Score(T[] input1, T[] input2);
}

public abstract class CachedScorerBase<T> : ICachedRatioScorer<T> where T : IEquatable<T>
{
    public abstract int Score(T[] input2);
}