using System;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class PartialRatioStrategy
{
    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns the partial fuzz.ratio for that alignment, as a value in [0…100].
    /// </summary>
    public static int Calculate(string input1, string input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var score = PartialRatioStrategy<char>.Calculate(input1.AsSpan(), input2.AsSpan());

        return score;
    }
}