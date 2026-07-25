using System;
using Xunit;

namespace Raffinert.FuzzySharp.Test;

public class IndelBestSubstringTests
{
    private const int RandomSeed = 834729;

    [Fact]
    public void BestSubstringMatch_EmptyPattern_ReturnsEmptyMatch()
    {
        Assert.Equal(new IndelSubstringMatch(0, -1),
            Indel.BestSubstringMatch(ReadOnlySpan<char>.Empty, "text".AsSpan()));
    }

    [Fact]
    public void BestSubstringMatch_EmptyText_ReturnsPatternDeletionDistance()
    {
        Assert.Equal(new IndelSubstringMatch(3, -1),
            Indel.BestSubstringMatch("abc".AsSpan(), ReadOnlySpan<char>.Empty));
    }

    [Fact]
    public void BestSubstringMatch_BothEmpty_ReturnsEmptyMatch()
    {
        Assert.Equal(new IndelSubstringMatch(0, -1),
            Indel.BestSubstringMatch(ReadOnlySpan<char>.Empty, ReadOnlySpan<char>.Empty));
    }

    [Theory]
    [InlineData("abc", "abcxxxx", 2)]
    [InlineData("abc", "xxabcxx", 4)]
    [InlineData("abc", "abxabc", 5)]
    [InlineData("abc", "abcxxxabc", 2)]
    public void BestSubstringMatch_ExactMatch_ReturnsEarliestExactEndpoint(
        string pattern,
        string text,
        int expectedEndIndex)
    {
        IndelSubstringMatch result = Indel.BestSubstringMatch(
            pattern.AsSpan(),
            text.AsSpan());

        Assert.Equal(0, result.Distance);
        Assert.Equal(expectedEndIndex, result.EndIndex);
        Assert.True(result.Found);
    }

    [Fact]
    public void BestSubstringMatch_SubstitutionCostsTwoEdits()
    {
        IndelSubstringMatch result = Indel.BestSubstringMatch(
            "abc".AsSpan(),
            "axc".AsSpan());

        Assert.Equal(new IndelSubstringMatch(2, 0), result);
    }

    [Fact]
    public void BestSubstringMatch_CanDeletePatternElementsForShorterSubstring()
    {
        Assert.Equal(new IndelSubstringMatch(1, 1),
            Indel.BestSubstringMatch("abc".AsSpan(), "ab".AsSpan()));
    }

    [Fact]
    public void BestSubstringMatch_NoCommonElement_PreservesNoMatchTieBehavior()
    {
        IndelSubstringMatch result = Indel.BestSubstringMatch(
            "abc".AsSpan(),
            "xxx".AsSpan());

        Assert.Equal(3, result.Distance);
        Assert.False(result.Found);
    }

    [Fact]
    public void BestSubstringMatch_SupportsGenericSequences()
    {
        int[] pattern = { 10, 20, 30, 40 };
        int[] text = { 0, 10, 20, 30, 40, 99 };

        Assert.Equal(new IndelSubstringMatch(0, 4),
            Indel.BestSubstringMatch<int>(pattern.AsSpan(), text.AsSpan()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    public void BestSubstringMatch_HandlesWordBoundariesAndLastBlockMasking(
        int patternLength)
    {
        string pattern = CreateSequence(patternLength);
        string text = "prefix-" + pattern + "-suffix";

        Assert.Equal(new IndelSubstringMatch(0, "prefix-".Length + patternLength - 1),
            Indel.BestSubstringMatch(pattern.AsSpan(), text.AsSpan()));
    }

    [Fact]
    public void BestSubstringMatch_HandlesCarriesBorrowsAndCrossBlockShifts()
    {
        string pattern = new string('a', 64) + new string('b', 65);
        string text = new string('a', 63) + "x" + new string('b', 65);

        AssertMatchesOracle(pattern, text);
    }

    [Fact]
    public void BestSubstringMatch_TextElementsAbsentFromPattern_AreHandled()
    {
        string pattern = CreateSequence(129);
        string text = new string('#', 80) + pattern.Substring(0, 128) + "!";

        AssertMatchesOracle(pattern, text);
    }

    [Fact]
    public void BestSubstringMatch_PaperStyleExample_MatchesScalarOracle()
    {
        AssertMatchesOracle("ACGC", "GAAGCGACTGCAAACTCA");
    }

    [Fact]
    public void BestSubstringMatch_ExhaustiveBinaryInputs_MatchScalarOracle()
    {
        for (int patternLength = 0; patternLength <= 7; patternLength++)
        {
            for (int textLength = 0; textLength <= 8; textLength++)
            {
                foreach (string pattern in BinaryStrings(patternLength))
                {
                    foreach (string text in BinaryStrings(textLength))
                    {
                        AssertMatchesOracle(pattern, text);
                    }
                }
            }
        }
    }

    [Fact]
    public void BestSubstringMatch_DeterministicRandomInputs_MatchScalarOracle()
    {
        var random = new Random(RandomSeed);
        string[] alphabets = { "ab", "abcd", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" };

        for (int sample = 0; sample < 4_000; sample++)
        {
            string alphabet = alphabets[sample % alphabets.Length];
            int patternLength = random.Next(1, 141);
            int textLength = random.Next(0, 201);
            string pattern = CreateRandomString(random, patternLength, alphabet);
            string text = CreateRandomString(random, textLength, alphabet);

            AssertMatchesOracle(pattern, text);
        }
    }

    private static void AssertMatchesOracle(string pattern, string text)
    {
        IndelSubstringMatch expected = ReferenceBestSubstringMatch(
            pattern.AsSpan(), text.AsSpan());
        IndelSubstringMatch actual = Indel.BestSubstringMatch(
            pattern.AsSpan(), text.AsSpan());

        Assert.Equal(expected, actual);
    }

    private static IndelSubstringMatch ReferenceBestSubstringMatch<T>(
        ReadOnlySpan<T> pattern,
        ReadOnlySpan<T> text)
        where T : IEquatable<T>
    {
        if (pattern.IsEmpty)
        {
            return new IndelSubstringMatch(0, -1);
        }

        if (text.IsEmpty)
        {
            return new IndelSubstringMatch(pattern.Length, -1);
        }

        int[] previous = new int[pattern.Length + 1];
        int[] current = new int[pattern.Length + 1];

        for (int patternIndex = 0; patternIndex <= pattern.Length; patternIndex++)
        {
            previous[patternIndex] = patternIndex;
        }

        int bestDistance = pattern.Length;
        int bestEndIndex = -1;

        for (int textIndex = 0; textIndex < text.Length; textIndex++)
        {
            current[0] = 0;

            for (int patternIndex = 1; patternIndex <= pattern.Length; patternIndex++)
            {
                int substitutionCost = pattern[patternIndex - 1].Equals(text[textIndex])
                    ? 0
                    : 2;
                current[patternIndex] = Math.Min(
                    Math.Min(previous[patternIndex] + 1, current[patternIndex - 1] + 1),
                    previous[patternIndex - 1] + substitutionCost);
            }

            if (current[pattern.Length] < bestDistance)
            {
                bestDistance = current[pattern.Length];
                bestEndIndex = textIndex;
            }

            (previous, current) = (current, previous);
        }

        return new IndelSubstringMatch(bestDistance, bestEndIndex);
    }

    private static string[] BinaryStrings(int length)
    {
        int count = 1 << length;
        var values = new string[count];

        for (int value = 0; value < count; value++)
        {
            var characters = new char[length];
            for (int index = 0; index < length; index++)
            {
                characters[index] = (value & (1 << index)) == 0 ? 'a' : 'b';
            }

            values[value] = new string(characters);
        }

        return values;
    }

    private static string CreateSequence(int length)
    {
        var characters = new char[length];
        for (int index = 0; index < characters.Length; index++)
        {
            characters[index] = (char)('a' + index % 26);
        }

        return new string(characters);
    }

    private static string CreateRandomString(Random random, int length, string alphabet)
    {
        var characters = new char[length];
        for (int index = 0; index < characters.Length; index++)
        {
            characters[index] = alphabet[random.Next(alphabet.Length)];
        }

        return new string(characters);
    }
}
