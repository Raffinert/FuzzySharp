using System;
using System.Linq;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;
using Raffinert.FuzzySharp.Utils;
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
    [InlineData(1023)]
    [InlineData(1024)]
    [InlineData(1025)]
    [InlineData(2049)]
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
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(16)]
    [InlineData(43)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(129)]
    [InlineData(257)]
    [InlineData(1023)]
    [InlineData(1024)]
    [InlineData(1025)]
    [InlineData(2049)]
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
    [InlineData("abcdefgh", "zzcdefgh")]
    [InlineData("abcdefghi", "zzcdefghi")]
    [InlineData("abcdefghi", "xxxxxxxxa")]
    [InlineData("abcdefghi", "zzzzzzzzzzzz")]
    [InlineData("абвгґдеєж", "жєедґгвба")]
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
    [InlineData("banana apple", "banana banana apple orange")]
    [InlineData("яблуко груша", "слива груша яблуко")]
    public void TokenSetPrefixFormulaMatchesUnoptimizedScorer(string a, string b)
    {
        double expected = new ReferenceTokenSetScorer().Score(a, b);
        using var cached = new CachedTokenSetScorer(a);
        Assert.Equal(expected, Fuzz.TokenSetRatio(a, b));
        Assert.Equal(expected, cached.Score(b));
        Assert.Equal(expected, Fuzz.TokenSetRatio(b, a));
    }

    // The public extension point still evaluates all three comparisons.
    [Theory]
    [InlineData("apple banana", "apple orange")]
    [InlineData("apple", "apple orange")]
    [InlineData("apple orange", "apple")]
    [InlineData("apple", "apple")]
    public void CustomTokenSetScorerStillInvokesItsOwnStrategy(string a, string b)
    {
        var scorer = new CountingTokenSetScorer();
        Assert.Equal(42, scorer.Score(a, b));
        Assert.Equal(3, scorer.Calls);
    }

    [Theory]
    [InlineData("apple banana", "apple orange")]
    [InlineData("apple", "apple orange")]
    [InlineData("apple orange", "apple")]
    [InlineData("apple", "apple")]
    public void CustomCachedTokenSetScorerStillInvokesItsOwnStrategy(string a, string b)
    {
        using var scorer = new CountingCachedTokenSetScorer(a);
        Assert.Equal(142, scorer.Score(b));
        Assert.Equal(3, scorer.Calls);
        Assert.Equal(142, scorer.Score(b));
        Assert.Equal(6, scorer.Calls);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(1023)]
    [InlineData(1024)]
    [InlineData(1025)]
    [InlineData(2049)]
    public void ReversedPatternVectorsMatchExplicitReversal(int length)
    {
        const string alphabet = "abяєґ";
        var chars = Enumerable.Range(0, length).Select(i => alphabet[i % alphabet.Length]).ToArray();
        var reversed = chars.Reverse().ToArray();
        using var actual = PatternMatchVector.CreateReversed<char>(chars);
        using var expected = PatternMatchVector.Create<char>(reversed);
        var ints = Array.ConvertAll(chars, c => (int)c);
        using var generic = PatternMatchVector.CreateReversed<int>(ints);
        if (length > 0 && length <= 64)
        {
            using var inPlace = PatternMatchVector.Create<char>(chars);
            using var genericInPlace = PatternMatchVector.Create<int>(ints);
            PatternMatchVector.ReverseSingleWord(inPlace);
            PatternMatchVector.ReverseSingleWord(genericInPlace);
            foreach (var symbol in alphabet + "z")
            {
                Assert.Equal(expected.GetOrZero(symbol).ToArray(), inPlace.GetOrZero(symbol).ToArray());
                Assert.Equal(expected.GetOrZero(symbol).ToArray(), genericInPlace.GetOrZero(symbol)[..1].ToArray());
            }
        }
        foreach (var symbol in alphabet + "z")
        {
            Assert.Equal(expected.ContainsKey(symbol), actual.ContainsKey(symbol));
            Assert.Equal(expected.ContainsKey(symbol), generic.ContainsKey(symbol));
            Assert.Equal(expected.GetOrZero(symbol).ToArray(), actual.GetOrZero(symbol).ToArray());
            // Generic missing-key masks can expose the pool's rounded-up rental.
            Assert.Equal(expected.GetOrZero(symbol).ToArray(), generic.GetOrZero(symbol)[..generic.Blocks].ToArray());
        }
    }

    [Theory]
    [InlineData(63, 1)]
    [InlineData(64, 2)]
    [InlineData(65, 17)]
    [InlineData(65, 131)]
    [InlineData(127, 64)]
    [InlineData(128, 129)]
    [InlineData(129, 257)]
    [InlineData(257, 33)]
    [InlineData(1023, 1)]
    [InlineData(1024, 2)]
    [InlineData(1025, 17)]
    [InlineData(2049, 3)]
    public void PartialRatioLongWindowsMatchExhaustiveDynamicProgramming(int length, int extraLength)
    {
        var random = new Random(length * 7919 + extraLength);
        var pattern = RandomSequence(random, length);
        var text = RandomSequence(random, length + extraLength);
        double expected = ReferencePartialRatio(pattern, text);

        AssertWindowAlignment(pattern, text, expected);
    }

    [Theory]
    [InlineData(63, 0, 17)]
    [InlineData(64, 1, 17)]
    [InlineData(65, 17, 0)]
    [InlineData(65, 64, 65)]
    [InlineData(129, 65, 33)]
    [InlineData(1025, 17, 33)]
    public void PartialRatioLongWindowsFindExactInteriorAndLastWindows(int length, int before, int after)
    {
        var pattern = Enumerable.Range(0, length).ToArray();
        var text = Enumerable.Repeat(-1, before).Concat(pattern).Concat(Enumerable.Repeat(-1, after)).ToArray();
        var alignment = AssertWindowAlignment(pattern, text, 100);
        Assert.Equal(before, alignment.DestStart);
        Assert.Equal(before + length, alignment.DestEnd);
    }

    [Theory]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(129)]
    [InlineData(1025)]
    public void PartialRatioLongWindowsFindNearExactInteriorWithoutRoundingToPerfect(int length)
    {
        var pattern = Enumerable.Range(0, length).ToArray();
        var candidate = (int[])pattern.Clone();
        candidate[length / 2] = -2;
        var text = Enumerable.Repeat(-1, 33).Concat(candidate).Concat(Enumerable.Repeat(-1, 35)).ToArray();
        double expected = ReferenceRatio(pattern, candidate);

        var alignment = AssertWindowAlignment(pattern, text, expected);
        Assert.True(alignment.Score < 100);
        Assert.Equal(33, alignment.DestStart);
        Assert.Equal(33 + length, alignment.DestEnd);
    }

    [Theory]
    [InlineData(63, false)]
    [InlineData(65, false)]
    [InlineData(1025, false)]
    [InlineData(65, true)]
    [InlineData(2049, true)]
    public void PartialRatioLongEdgesCanBeatInteriorWindows(int length, bool suffix)
    {
        var pattern = Enumerable.Range(0, length).ToArray();
        var text = suffix
            ? Enumerable.Repeat(-1, 70).Concat(pattern.Skip(1)).ToArray()
            : pattern.Take(length - 1).Concat(Enumerable.Repeat(-1, 70)).ToArray();
        double expected = 100 * (1.0 - 1.0 / (2 * length - 1));
        var alignment = AssertWindowAlignment(pattern, text, expected);
        Assert.Equal(suffix ? 70 : 0, alignment.DestStart);
        Assert.Equal(suffix ? text.Length : length - 1, alignment.DestEnd);
    }

    [Theory]
    [InlineData(65)]
    [InlineData(1025)]
    public void PartialRatioLongWindowsKeepFirstExactMatchOnTies(int length)
    {
        var pattern = Enumerable.Range(0, length).ToArray();
        var text = pattern.Concat(Enumerable.Repeat(-1, 37)).Concat(pattern).ToArray();
        var alignment = AssertWindowAlignment(pattern, text, 100);
        Assert.Equal(0, alignment.DestStart);
        Assert.Equal(length, alignment.DestEnd);
    }

#if NET8_0_OR_GREATER
    [Theory]
    [InlineData(9, false)]
    [InlineData(63, false)]
    [InlineData(64, false)]
    [InlineData(9, true)]
    [InlineData(64, true)]
    public void PartialRatioSingleWordEdgesDoNotAllocateAnotherPatternVector(int length, bool unicode)
    {
        string alphabet = unicode ? "абвгґдеє" : "abcdefgh";
        string query = new string(Enumerable.Range(0, length).Select(i => alphabet[i % alphabet.Length]).ToArray());
        string candidate = query.Substring(0, length - 1) + "!";
        for (int i = 0; i < 256; i++)
            Fuzz.PartialRatio(query, candidate);

        const int repetitions = 128;
        long start = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < repetitions; i++)
        {
            using (PatternMatchVector.Create<char>(query)) { }
            using (PatternMatchVector.Create<char>(candidate)) { }
        }
        long queryVectors = GC.GetAllocatedBytesForCurrentThread() - start;
        start = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < repetitions; i++)
            Fuzz.PartialRatio(query, candidate);

        // Equal non-exact inputs require two directions, each with its own
        // query vector. Neither suffix scan should allocate a second vector.
        Assert.Equal(queryVectors, GC.GetAllocatedBytesForCurrentThread() - start);
    }
#endif

    private static PartialRatioStrategy<int>.ScoreAlignment AssertWindowAlignment(int[] pattern, int[] text, double expected)
    {
        var actual = PartialRatioStrategy<int>.PartialRatioAlignment(pattern, text);
        Assert.Equal(expected, actual.Score, 12);
        Assert.Equal(0, actual.SrcStart);
        Assert.Equal(pattern.Length, actual.SrcEnd);
        Assert.InRange(actual.DestStart, 0, text.Length - 1);
        Assert.InRange(actual.DestEnd, actual.DestStart + 1, text.Length);
        Assert.Equal(actual.Score, ReferenceRatio(pattern, text.AsSpan(actual.DestStart, actual.DestEnd - actual.DestStart)), 12);

        var swapped = PartialRatioStrategy<int>.PartialRatioAlignment(text, pattern);
        Assert.Equal(actual.Score, swapped.Score);
        Assert.Equal(actual.DestStart, swapped.SrcStart);
        Assert.Equal(actual.DestEnd, swapped.SrcEnd);
        Assert.Equal(actual.SrcStart, swapped.DestStart);
        Assert.Equal(actual.SrcEnd, swapped.DestEnd);

        var first = new string(Array.ConvertAll(pattern, value => (char)(0x1000 + value)));
        var second = new string(Array.ConvertAll(text, value => (char)(0x1000 + value)));
        var characters = PartialRatioStrategy<char>.PartialRatioAlignment(first.AsSpan(), second.AsSpan());
        Assert.Equal(actual.Score, characters.Score, 12);
        Assert.Equal(actual.DestStart, characters.DestStart);
        Assert.Equal(actual.DestEnd, characters.DestEnd);
        Assert.Equal(expected, Fuzz.PartialRatio(first, second), 12);
        Assert.Equal(expected, Fuzz.PartialRatio(second, first), 12);
        return actual;
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

    private sealed class CountingCachedTokenSetScorer(string query) : CachedTokenSetScorerBase(query)
    {
        public int Calls { get; private set; }
        protected override global::Raffinert.FuzzySharp.Scorer Scorer => (_, _) =>
        {
            Calls++;
            return 142;
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
        var prefixes = ReferencePrefixRatios(a, b[..(a.Length - 1)], reverse: false);
        var suffixes = ReferencePrefixRatios(a, b[(b.Length - a.Length)..], reverse: true);
        for (int i = 1; i < a.Length; i++)
        {
            double score = prefixes[i - 1];
            if (score > best)
                (best, start, end) = (score, 0, i);
        }
        for (int i = b.Length - a.Length; i < b.Length; i++)
        {
            double score = suffixes[b.Length - i - 1];
            if (score > best)
                (best, start, end) = (score, i, b.Length);
        }
        return (best, start, end);
    }

    // Independent quadratic DP reference, keeping scores for every prefix.
    // This also makes the 1024/1025 pooled-row boundary practical to test.
    private static double[] ReferencePrefixRatios(ReadOnlySpan<int> pattern, ReadOnlySpan<int> text, bool reverse)
    {
        var row = new int[pattern.Length + 1];
        var scores = new double[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            int symbol = text[reverse ? text.Length - i - 1 : i];
            int diagonal = 0;
            for (int j = 0; j < pattern.Length; j++)
            {
                int above = row[j + 1];
                int item = pattern[reverse ? pattern.Length - j - 1 : j];
                row[j + 1] = item == symbol ? diagonal + 1 : Math.Max(above, row[j]);
                diagonal = above;
            }
            int total = pattern.Length + i + 1;
            scores[i] = 100 * (1.0 - (total - 2 * row[pattern.Length]) / (double)total);
        }
        return scores;
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
