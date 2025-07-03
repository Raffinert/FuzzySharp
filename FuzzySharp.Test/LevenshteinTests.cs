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
}
