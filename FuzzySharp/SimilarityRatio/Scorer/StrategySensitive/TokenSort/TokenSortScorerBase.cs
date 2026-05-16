using Raffinert.FuzzySharp.Extensions;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

using System;

public abstract class TokenSortScorerBase : StrategySensitiveScorerBase
{
    public override int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        var sorted1 = input1.NormalizeSpacesAndSort();
        var sorted2 = input2.NormalizeSpacesAndSort();

        return Scorer(sorted1.Span, sorted2.Span);
    }
}