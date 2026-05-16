using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive.Generic;
using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class TokenDifferenceScorerBase : StrategySensitiveScorerBase<string>, IRatioScorer
{
    public override int Score(string[] input1, string[] input2)
    {
        return Scorer(input1, input2);
    }

    public int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2, Processor<char> preprocessor)
    {
        preprocessor(ref input1);
        preprocessor(ref input2);
        return Score(input1, input2);
    }

    public int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        var tokens1 = input1.GetSortedWords();
        var tokens2 = input2.GetSortedWords();

        return Score(tokens1, tokens2);
    }
}