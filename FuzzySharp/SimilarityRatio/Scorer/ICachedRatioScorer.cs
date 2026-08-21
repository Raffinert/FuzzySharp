using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer;

public interface ICachedRatioScorer: IDisposable
{
    double Score(string input2);
}
