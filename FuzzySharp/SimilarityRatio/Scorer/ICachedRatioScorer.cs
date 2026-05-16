using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface ICachedRatioScorer: IDisposable
{
    int Score(ReadOnlySpan<char> input2);
}