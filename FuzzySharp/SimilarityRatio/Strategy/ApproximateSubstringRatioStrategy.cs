using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class ApproximateSubstringRatioStrategy
{
    public static int Calculate(string input1, string input2)
    {
        ReadOnlySpan<char> first = input1.AsSpan();
        ReadOnlySpan<char> second = input2.AsSpan();

        if (!first.IsEmpty && !second.IsEmpty)
        {
            ReadOnlySpan<char> pattern = first.Length <= second.Length
                ? first
                : second;
            ReadOnlySpan<char> text = first.Length <= second.Length
                ? second
                : first;

            if (text.IndexOf(pattern) >= 0)
            {
                return 100;
            }
        }

        return Generic.ApproximateSubstringRatioStrategy<char>.Calculate(
            first,
            second);
    }
}
