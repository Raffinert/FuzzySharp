using System;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public sealed class PartialTokenDifferenceScorer : TokenDifferenceScorerBase
{
    protected override Func<string[], string[], double> Scorer => static (strings1, strings2) => PartialRatioStrategy<string>.Calculate(strings1.AsSpan(), strings2.AsSpan());
}
