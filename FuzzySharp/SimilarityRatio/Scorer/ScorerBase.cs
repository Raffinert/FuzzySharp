using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public abstract class ScorerBase : IRatioScorer
{
    public abstract int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2);

    public int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2, Processor<char> preprocessor)
    {
        preprocessor(ref input1);
        preprocessor(ref input2);
        return Score(input1, input2);
    }
}