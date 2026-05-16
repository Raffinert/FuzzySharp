using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.Utils;
using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class TokenAbbreviationScorerBase : StrategySensitiveScorerBase
{
    public override int Score(ReadOnlySpan<char> shorter, ReadOnlySpan<char> longer)
    {
        var s = shorter;
        var l = longer;
        SequenceUtils.SwapIfSourceIsLonger(ref s, ref l);

        double lenRatio = (double)l.Length / s.Length;

        // if longer isn't at least 1.5 times longer than the other, then it's probably not an abbreviation
        if (lenRatio < 1.5) return 0;

        // numbers can't be abbreviations for other numbers, though that would be hilarious. "Yes, 4 - as in 4,238"
        var tokensLonger = l.ExtractTokens();
        var tokensShorter = s.ExtractTokens();

        SequenceUtils.SwapIfSourceIsLonger(ref tokensShorter, ref tokensLonger);

        // more than 4 tokens and it's probably not an abbreviation (and could get costly)
        if (tokensShorter.Count > 4)
        {
            return 0;
        }

        var allPermutations = tokensLonger.PermutationsOfSize(tokensShorter.Count);

        int maxScore = 0;

        foreach (var permutation in allPermutations)
        {
            double sum = 0;
            for (int i = 0; i < tokensShorter.Count; i++)
            {
                var i1 = permutation[i].AsSpan();
                var i2 = tokensShorter[i].AsSpan();
                if (StringContainsInOrder(i1, i2)) // must be at least twice as long
                {
                    var score = Scorer(i1, i2);
                    sum += score;
                }
            }
            var avgScore = (int)(sum / tokensShorter.Count);
            if (avgScore > maxScore)
            {
                maxScore = avgScore;
            }
        }

        return maxScore;
    }

    /// <summary>
    /// Does s2 have all its characters appear in order in s1? (Basically, is s2 a potential abbreviation of s1?)
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    private static bool StringContainsInOrder(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        if (s1.Length < s2.Length) return false;
        var s2Idx = 0;
        for (var i = 0; i < s1.Length; i++)
        {
            if (s2[s2Idx] == s1[i])
                s2Idx++;
            if (s2Idx == s2.Length)
                return true;
            if (i + s2.Length - s2Idx == s1.Length)
                return false;
        }
        return false;
    }
}