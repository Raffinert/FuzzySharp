using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.NewAbstraction;

public interface IFuzzyScorer<T> : IDisposable where T : IEquatable<T>
{
    static abstract IFuzzyScorer<T> Create(ReadOnlyMemory<T> input1);

    int ScoreFrom(ReadOnlySpan<T> input2);

    static abstract int Score(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2);
    static abstract int Score(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2, Processor<T> processor);
}