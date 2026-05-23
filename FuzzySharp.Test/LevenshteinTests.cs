using NUnit.Framework;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class LevenshteinTests
{
    [Test]
    [TestCase(
    "I had two heart attacks, an abortion, did crack... while I was pregnant. Other than that, I'm fine.",
    "You couldn't even be a vegetable - even artichokes have a heart.",
    76)]
    [TestCase("", "", 0)]
    [TestCase("a", "", 1)]
    [TestCase("", "a", 1)]
    [TestCase("kitten", "kitten", 0)]
    [TestCase("kitten", "sitting", 3)]     // substitution + insertion + insertion
    [TestCase("flaw", "lawn", 2)]          // substitution + insertion
    [TestCase("gumbo", "gambol", 2)]       // insertion + substitution
    [TestCase("book", "back", 2)]          // two substitutions
    [TestCase("Sunday", "Saturday", 3)]    // insertion + substitution + insertion
    // Test a few boundary scenarios (longer strings, only one‐character difference)
    [TestCase("a", "b", 1)]
    [TestCase("ab", "ba", 2)]
    [TestCase("abcdef", "azced", 3)]
    [TestCase("distance", "difference", 5)]
    public void TestLevenshteinDistance(string s1, string s2, int expectedDistance)
    {
        int distance = Levenshtein.Distance(s1, s2);
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void TestLevenshteinDistance()
    {
        var wordA = new string('A', 4112);
        var wordB = new string('B', 4112);
        int maxDistance = Levenshtein.Distance(wordA, wordB);
        int zeroDistance = Levenshtein.Distance(wordA, wordA);
        Assert.AreEqual(4112, maxDistance);
        Assert.AreEqual(0, zeroDistance);
    }

    [Test, TestCaseSource(typeof(RandomWordPairs), nameof(RandomWordPairs.GetWordPairs))]
    public void Levenshtein_ShouldBeEqual(string s1, string s2)
    {
        var fd = Levenshtein.Distance(s1, s2);
        var eo = Levenshtein.GetEditOps(s1, s2);
        //var mb1 = eo.AsMatchingBlocks(s1.Length, s2.Length);
        //var mb2 = global::FuzzySharp.Levenshtein.GetMatchingBlocks(s1, s2);
        var qd = Quickenshtein.Levenshtein.GetDistance(s1, s2);

        Assert.That(fd, Is.EqualTo(qd));
        Assert.That(eo.Length, Is.EqualTo(qd));
        //Assert.That(mb, Is.EquivalentTo(mb2));
    }

    [Test]
    [TestCase("abc", "adc", 1, 1, 1, 1)]
    [TestCase("abc", "adc", 1, 1, 2, 2)]
    [TestCase("abc", "adc", 2, 1, 1, 1)]
    [TestCase("abc", "adc", 1, 2, 1, 1)]
    [TestCase("abc", "adc", 2, 2, 3, 3)]
    [TestCase("abc", "", 2, 1, 1, 3)]
    [TestCase("", "abc", 2, 1, 1, 6)]
    public void TestLevenshteinDistance_Weighted(string s1, string s2, int insertCost, int deleteCost, int replaceCost, int expected)
    {
        var distance = Levenshtein.Distance(s1, s2, insertCost, deleteCost, replaceCost);
        Assert.That(distance, Is.EqualTo(expected));
    }

    [Test]
    public void TestLevenshteinDistance_WeightedScoreCutoff()
    {
        var distance = Levenshtein.Distance("abc", "", insertCost: 2, deleteCost: 1, replaceCost: 3, scoreCutoff: 2);
        Assert.That(distance, Is.EqualTo(3));
    }

    [Test]
    public void TestLevenshteinSimilarity_UsesSimilarityCutoff()
    {
        var similarity = Levenshtein.Similarity("kitten", "sitting", scoreCutoff: 5);
        Assert.That(similarity, Is.EqualTo(0));
    }

    [Test]
    public void TestLevenshteinMaximum_MatchesExpectedFormula()
    {
        var maximum = Levenshtein.LevenshteinMaximum(5, 3, insertCost: 2, deleteCost: 3, replaceCost: 4);
        Assert.That(maximum, Is.EqualTo(18));
    }

    [Test]
    public void TestLevenshteinDistance_CutoffDoesNotExitEarly_ForRecoverablePrefix_SingleUlong()
    {
        var distance = Levenshtein.Distance("abc", "xabc", scoreCutoff: 1);
        Assert.That(distance, Is.EqualTo(1));
    }

    [Test]
    public void TestLevenshteinDistance_CutoffDoesNotExitEarly_ForRecoverablePrefix_MultiUlong()
    {
        var source = new string('a', 70);
        var target = "x" + source;

        var distance = Levenshtein.Distance(source, target, scoreCutoff: 1);
        Assert.That(distance, Is.EqualTo(1));
    }
}
