using Raffinert.FuzzySharp.PreProcess;
using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public abstract class ScorerBase : IRatioScorer
{
    public abstract int Score(string input1, string input2);

    public int Score(string input1, string input2, Func<string, string> preprocessor)
    {
        preprocessor ??= StringPreprocessor.Full;
        input1 = preprocessor(input1);
        input2 = preprocessor(input2);
        return Score(input1, input2);
    }
}