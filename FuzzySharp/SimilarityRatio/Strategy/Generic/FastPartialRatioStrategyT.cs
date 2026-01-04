using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

internal static class FastPartialRatioStrategyT<T> where T : IEquatable<T>
{
    public static int Calculate(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2)
    {
        var shorter = input1;
        var longer = input2;

        SequenceUtils.SwapIfSourceIsLonger(ref shorter, ref longer);

        var matchingBlocks = Levenshtein.GetMatchingBlocks(shorter, longer);

        double maxScore = 0;

        foreach (var matchingBlock in matchingBlocks)
        {
            int dist = matchingBlock.DestPos - matchingBlock.SourcePos;

            int longStart = dist > 0 ? dist : 0;
            int longEnd   = longStart + shorter.Length;

            if (longEnd > longer.Length) longEnd = longer.Length;

            var longSubstr = longer[longStart..longEnd];

            double ratio = Indel.NormalizedSimilarity(shorter, longSubstr);

            if (ratio > .995)
            {
                return 100;
            }

            if (ratio > maxScore)
            {
                maxScore = ratio;
            }
        }

        return (int)Math.Round(100 * maxScore);
    }
}