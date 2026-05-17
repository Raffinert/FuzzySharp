using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface IRatioScorer
{
    int Score(string input1, string input2);
    int Score(string input1, string input2, Func<string, string> preprocessor);
}