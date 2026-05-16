using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

public interface IRatioScorer<T> where T : IEquatable<T>
{
    int Score(T[] input1, T[] input2);
}

public interface ICachedRatioScorer<T> where T : IEquatable<T>
{
    int Score(T[] input2);
}