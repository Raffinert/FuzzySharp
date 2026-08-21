using Xunit;

namespace Raffinert.FuzzySharp.Test;

public class LevenshteinTests
{
    [Theory]
    [InlineData(
    "I had two heart attacks, an abortion, did crack... while I was pregnant. Other than that, I'm fine.",
    "You couldn't even be a vegetable - even artichokes have a heart.",
    76)]
    [InlineData("", "", 0)]
    [InlineData("a", "", 1)]
    [InlineData("", "a", 1)]
    [InlineData("kitten", "kitten", 0)]
    [InlineData("kitten", "sitting", 3)]     // substitution + insertion + insertion
    [InlineData("flaw", "lawn", 2)]          // substitution + insertion
    [InlineData("gumbo", "gambol", 2)]       // insertion + substitution
    [InlineData("book", "back", 2)]          // two substitutions
    [InlineData("Sunday", "Saturday", 3)]    // substitution + insertion + insertion
    // Test a few boundary scenarios (longer strings, only one‐character difference)
    [InlineData("a", "b", 1)]
    [InlineData("ab", "ba", 2)]
    [InlineData("abcdef", "azced", 3)]
    [InlineData("distance", "difference", 5)]
    public void TestLevenshteinDistance(string s1, string s2, int expectedDistance)
    {
        int distance = Levenshtein.Distance(s1, s2);
        Assert.Equal(expectedDistance, distance);
    }

    [Fact]
    public void TestLevenshteinDistance_LongInputs()
    {
        var wordA = new string('A', 4112);
        var wordB = new string('B', 4112);
        int maxDistance = Levenshtein.Distance(wordA, wordB);
        int zeroDistance = Levenshtein.Distance(wordA, wordA);
        Assert.Equal(4112, maxDistance);
        Assert.Equal(0, zeroDistance);
    }

    [Theory]
    [MemberData(nameof(RandomWordPairs.GetWordPairs), MemberType = typeof(RandomWordPairs))]
    public void Levenshtein_ShouldBeEqual(string s1, string s2)
    {
        var fd = Levenshtein.Distance(s1, s2);
        var eo = Levenshtein.GetEditOps(s1, s2);
        //var mb1 = eo.AsMatchingBlocks(s1.Length, s2.Length);
        //var mb2 = global::FuzzySharp.Levenshtein.GetMatchingBlocks(s1, s2);
        var qd = Quickenshtein.Levenshtein.GetDistance(s1, s2);

        Assert.Equal(qd, fd);
        Assert.Equal(qd, eo.Length);
        //Assert.That(mb, Is.EquivalentTo(mb2));
    }

    [Theory]
    [InlineData("abc", "adc", 1, 1, 1, 1)]
    [InlineData("abc", "adc", 1, 1, 2, 2)]
    [InlineData("abc", "adc", 2, 1, 1, 1)]
    [InlineData("abc", "adc", 1, 2, 1, 1)]
    [InlineData("abc", "adc", 2, 2, 3, 3)]
    [InlineData("abc", "", 2, 1, 1, 3)]
    [InlineData("", "abc", 2, 1, 1, 6)]
    public void TestLevenshteinDistance_Weighted(string s1, string s2, int insertCost, int deleteCost, int replaceCost, int expected)
    {
        var distance = Levenshtein.Distance(s1, s2, insertCost, deleteCost, replaceCost);
        Assert.Equal(expected, distance);
    }

    [Fact]
    public void TestLevenshteinMaximum_MatchesExpectedFormula()
    {
        var maximum = Levenshtein.LevenshteinMaximum(5, 3, insertCost: 2, deleteCost: 3, replaceCost: 4);
        Assert.Equal(18, maximum);
    }

}

