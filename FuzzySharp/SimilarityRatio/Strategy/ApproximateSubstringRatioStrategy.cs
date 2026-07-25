using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class ApproximateSubstringRatioStrategy
{
    public static int Calculate(string input1, string input2)
    {
        return Generic.ApproximateSubstringRatioStrategy<char>.Calculate(
            input1.AsSpan(),
            input2.AsSpan());
    }
}
