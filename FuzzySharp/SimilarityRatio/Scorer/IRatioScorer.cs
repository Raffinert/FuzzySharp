using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface IRatioScorer
{
    public int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2);
    public int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2, Processor<char> preprocessor);
}