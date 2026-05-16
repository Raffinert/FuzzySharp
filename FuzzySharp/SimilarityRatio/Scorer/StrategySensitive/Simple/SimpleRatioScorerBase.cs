namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

using System;

public abstract class SimpleRatioScorerBase : StrategySensitiveScorerBase
{
    public override int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        return Scorer(input1, input2);
    }
}