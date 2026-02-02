using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;
using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class TokenDifferenceScorer : TokenDifferenceScorerBase
{
    protected override Func<string[], string[], int> Scorer => DefaultRatioStrategy<string>.Calculate;
}