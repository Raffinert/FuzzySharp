using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class DefaultRatioStrategy
{
    public static int Calculate(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        return (int)Math.Round(100 * Indel.NormalizedSimilarity(input1, input2));
    }
}