using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface IRatioScorer
{
    double Score(string input1, string input2);
    double Score(string input1, string input2, Func<string, string> preprocessor);
}
