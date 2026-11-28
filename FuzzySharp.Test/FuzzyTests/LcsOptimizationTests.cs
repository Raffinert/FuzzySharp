using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;
using Xunit;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class LcsOptimizationTests
{
    [Theory]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData(257)]
    public void LcsAndMatrixMatchDynamicProgramming(int length)
    {
        var random = new Random(length);
        for (int sample = 0; sample < 8; sample++)
        {
            var a = RandomSequence(random, length);
            var b = RandomSequence(random, length + 17);
            int expected = ReferenceLcs(a, b);

            Assert.Equal(expected, LongestCommonSubsequence.Similarity<int>(a, b));
            Assert.Equal(expected, LongestCommonSubsequence.Matrix<int>(a, b).Sim);
            Assert.Equal(a.Length + b.Length - 2 * expected, Indel.Distance<int>(a, b));
        }

        // Exercise carries across a full machine word with dense masks.
        var repeated = new int[length];
        var shifted = new int[length + 1];
        shifted[0] = 1;
        Assert.Equal(length, LongestCommonSubsequence.Similarity<int>(repeated, shifted));
        Assert.Equal(length, LongestCommonSubsequence.Matrix<int>(repeated, shifted).Sim);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(43)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(129)]
    [InlineData(257)]
    public void PartialRatioEdgesMatchReferenceAndPreserveAlignment(int length)
    {
        var random = new Random(7919 + length);
        for (int sample = 0; sample < 5; sample++)
        {
            int[] a = RandomSequence(random, length);
            int[] b = RandomSequence(random, length);
            var forward = ReferenceEdgeAlignment(a, b);
            var backward = ReferenceEdgeAlignment(b, a);
            var actual = PartialRatioStrategy<int>.PartialRatioAlignment(a, b);

            if (backward.Score > forward.Score)
            {
                Assert.Equal(backward.Score, actual.Score, 12);
                Assert.Equal(backward.Start, actual.SrcStart);
                Assert.Equal(backward.End, actual.SrcEnd);
                Assert.Equal(0, actual.DestStart);
                Assert.Equal(length, actual.DestEnd);
            }
            else
            {
                Assert.Equal(forward.Score, actual.Score, 12);
                Assert.Equal(0, actual.SrcStart);
                Assert.Equal(length, actual.SrcEnd);
                Assert.Equal(forward.Start, actual.DestStart);
                Assert.Equal(forward.End, actual.DestEnd);
            }
        }
    }

    [Theory]
    [InlineData("abc", "bcd")]
    [InlineData("abc", "xab")]
    [InlineData("abc", "xyz")]
    [InlineData("apple", "pineapple")]
    [InlineData("абвг", "гдеабв")]
    [InlineData("the quick brown fox jumps over the lazy dog", "the lazy dog jumps over the quick brown fox")]
    public void PartialRatioMatchesExhaustiveWindowSearch(string a, string b)
    {
        var first = Array.ConvertAll(a.ToCharArray(), c => (int)c);
        var second = Array.ConvertAll(b.ToCharArray(), c => (int)c);
        double expected = ReferencePartialRatio(first, second);
        Assert.Equal(expected, Fuzz.PartialRatio(a, b), 12);
        Assert.Equal(expected, PartialRatioStrategy<int>.Calculate(first, second), 12);
        Assert.Equal(expected, Fuzz.PartialRatio(b, a), 12);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "a")]
    [InlineData("apple", "banana")]
    [InlineData("apple banana", "apple banana orange")]
    [InlineData("apple apple banana", "banana apple")]
    [InlineData("apple banana orange", "apple banana lemon")]
    [InlineData("яблуко груша", "яблуко слива")]
    [InlineData("a b c", "d e f")]
    public void TokenSetPrefixFormulaMatchesUnoptimizedScorer(string a, string b)
    {
        double expected = new ReferenceTokenSetScorer().Score(a, b);
        using var cached = new CachedTokenSetScorer(a);
        Assert.Equal(expected, Fuzz.TokenSetRatio(a, b));
        Assert.Equal(expected, cached.Score(b));
        Assert.Equal(expected, Fuzz.TokenSetRatio(b, a));
    }

    // The public extension point still evaluates all three comparisons.
    [Fact]
    public void CustomTokenSetScorerStillInvokesItsOwnStrategy()
    {
        var scorer = new CountingTokenSetScorer();
        Assert.Equal(42, scorer.Score("apple banana", "apple orange"));
        Assert.Equal(3, scorer.Calls);
    }

    private sealed class ReferenceTokenSetScorer : TokenSetScorerBase
    {
        protected override global::Raffinert.FuzzySharp.Scorer Scorer => Fuzz.Ratio;
    }

    private sealed class CountingTokenSetScorer : TokenSetScorerBase
    {
        public int Calls { get; private set; }
        protected override global::Raffinert.FuzzySharp.Scorer Scorer => (_, _) =>
        {
            Calls++;
            return 42;
        };
    }

    private static int[] RandomSequence(Random random, int length)
    {
        var result = new int[length];
        for (int i = 0; i < result.Length; i++)
            result[i] = random.Next(8);
        return result;
    }

    private static int ReferenceLcs(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
    {
        var row = new int[b.Length + 1];
        foreach (int x in a)
        {
            int diagonal = 0;
            for (int j = 0; j < b.Length; j++)
            {
                int above = row[j + 1];
                row[j + 1] = x == b[j]
                    ? diagonal + 1
                    : Math.Max(above, row[j]);
                diagonal = above;
            }
        }
        return row[b.Length];
    }

    private static double ReferenceRatio(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
    {
        int total = a.Length + b.Length;
        return 100 * (1.0 - (total - 2 * ReferenceLcs(a, b)) / (double)total);
    }

    private static (double Score, int Start, int End) ReferenceEdgeAlignment(
        ReadOnlySpan<int> a, ReadOnlySpan<int> b)
    {
        double best = 0;
        int start = 0, end = a.Length;
        for (int i = 1; i < a.Length; i++)
        {
            double score = ReferenceRatio(a, b[..i]);
            if (score > best)
                (best, start, end) = (score, 0, i);
        }
        for (int i = b.Length - a.Length; i < b.Length; i++)
        {
            double score = ReferenceRatio(a, b[i..]);
            if (score > best)
                (best, start, end) = (score, i, b.Length);
        }
        return (best, start, end);
    }

    private static double ReferencePartialRatio(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
    {
        if (a.Length > b.Length)
            return ReferencePartialRatio(b, a);

        double best = ReferenceEdgeAlignment(a, b).Score;
        for (int i = 0; i <= b.Length - a.Length; i++)
            best = Math.Max(best, ReferenceRatio(a, b.Slice(i, a.Length)));

        if (a.Length == b.Length)
            best = Math.Max(best, ReferenceEdgeAlignment(b, a).Score);
        return best;
    }
}
