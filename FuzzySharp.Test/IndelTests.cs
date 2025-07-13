using NUnit.Framework;
using Raffinert.FuzzySharp.SimilarityRatio.NewAbstraction;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class IndelTests
{
    [Test]
    [TestCase(
    "I had two heart attacks, an abortion, did crack... while I was pregnant. Other than that, I'm fine.",
    "You couldn't even be a vegetable - even artichokes have a heart.",
    103)]
    [TestCase("", "", 0)]
    [TestCase("a", "", 1)]
    [TestCase("", "a", 1)]
    [TestCase("kitten", "kitten", 0)]
    [TestCase("kitten", "sitting", 5)]     // replace + replace + insert
    [TestCase("flaw", "lawn", 2)]          // delete + insert
    [TestCase("gumbo", "gambol", 3)]       // insert + replace
    [TestCase("book", "back", 4)]          // two substitutions
    [TestCase("Sunday", "Saturday", 4)]    // insertion + substitution + insertion
    // Test a few boundary scenarios (longer strings, only one‐character difference)
    [TestCase("a", "b", 2)]
    [TestCase("ab", "ba", 2)]
    [TestCase("abcdef", "azced", 5)]
    [TestCase("distance", "difference", 8)]
    public void TestIndelDistance(string s1, string s2, int expectedDistance)
    {
        int distance = IndelStringFuzzyScorer.Score(s1, s2);
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void TestIndelDistance()
    {
        var wordA = new string('A', 4112);
        var wordB = new string('B', 4112);
        int maxDistance = IndelStringFuzzyScorer.Score(wordA, wordB);
        int zeroDistance = IndelStringFuzzyScorer.Score(wordA, wordA);
        Assert.AreEqual(8224, maxDistance);
        Assert.AreEqual(0, zeroDistance);
    }
}
