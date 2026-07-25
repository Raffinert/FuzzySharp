using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class ApproximateSubstringRatioStrategy<T>
    where T : notnull, IEquatable<T>
{
    public static int Calculate(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2)
    {
        if (input1.IsEmpty || input2.IsEmpty)
        {
            return input1.IsEmpty && input2.IsEmpty ? 100 : 0;
        }

        if (input1.Length < input2.Length)
        {
            return Calculate(input1, input2, Indel.BestSubstringMatch(input1, input2));
        }

        if (input2.Length < input1.Length)
        {
            return Calculate(input2, input1, Indel.BestSubstringMatch(input2, input1));
        }

        int forward = Calculate(input1, input2, Indel.BestSubstringMatch(input1, input2));

        if(forward == 100)
        {
            return 100;
        }

        int reverse = Calculate(input2, input1, Indel.BestSubstringMatch(input2, input1));
        return Math.Max(forward, reverse);
    }

    internal static int Calculate(
        ReadOnlySpan<T> pattern,
        ReadOnlySpan<T> text,
        IndelSubstringMatch match)
    {
        if (pattern.IsEmpty || text.IsEmpty)
        {
            return pattern.IsEmpty && text.IsEmpty ? 100 : 0;
        }

        double similarity = 1.0 - match.Distance / (double)pattern.Length;
        int score = (int)Math.Round(100.0 * similarity);
        return Math.Max(0, Math.Min(100, score));
    }
}
