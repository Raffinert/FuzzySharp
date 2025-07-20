using NUnit.Framework;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;
using System;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

// Original code https://github.com/rapidfuzz/RapidFuzz/blob/main/tests/test_fuzz.py
public class RatioIssuesTests
{
    [Test]
    public void Issue76()
    {
        Assert.That(Fuzz.PartialRatio("physics 2 vid", "study physics physics 2"), Is.EqualTo(82));
        Assert.That(Fuzz.PartialRatio("physics 2 vid", "study physics physics 2 video"), Is.EqualTo(100));
    }

    [Test]
    public void Issue90()
    {
        Assert.That(Fuzz.PartialRatio("ax b", "a b a c b"), Is.EqualTo(86));
    }

    [Test]
    public void Issue138()
    {
        var str1 = new string('a', 65);
        var str2 = "a" + (char)256 + new string('a', 63);
        Assert.That(Fuzz.PartialRatio(str1, str2), Is.EqualTo(98));
    }

    [Test]
    public void PartialRatioAlignment()
    {
        var a = "a certain string".AsSpan();
        var s = "certain".AsSpan();

        var align1 = PartialRatioStrategy<char>.PartialRatioAlignment(s, a);

        Assert.That(align1.Score, Is.EqualTo(100));
        Assert.That(align1.SrcStart, Is.EqualTo(0));
        Assert.That(align1.SrcEnd, Is.EqualTo(s.Length));
        Assert.That(align1.DestStart, Is.EqualTo(2));
        Assert.That(align1.DestEnd, Is.EqualTo(2 + s.Length));

        var align2 = PartialRatioStrategy<char>.PartialRatioAlignment(a, s);
        Assert.That(align2.Score, Is.EqualTo(100));
        Assert.That(align2.SrcStart, Is.EqualTo(2));
        Assert.That(align2.SrcEnd, Is.EqualTo(2 + s.Length));
        Assert.That(align2.DestStart, Is.EqualTo(0));
        Assert.That(align2.DestEnd, Is.EqualTo(s.Length));

        Assert.That(PartialRatioStrategy<char>.PartialRatioAlignment(null, "test".AsSpan()).Score, Is.EqualTo(0));
        Assert.That(PartialRatioStrategy<char>.PartialRatioAlignment("test".AsSpan(), null).Score, Is.EqualTo(0));
        Assert.That(PartialRatioStrategy<char>.PartialRatioAlignment("test".AsSpan(), "tesx".AsSpan(), scoreCutoff: 90).Score, Is.EqualTo(0));
    }

    [Test]
    public void Issue196()
    {
        Assert.That(Fuzz.WeightedRatio("South Korea", "North Korea"), Is.EqualTo(82));
    }

    [Test]
    public void Issue231()
    {
        var str1 = "er merkantilismus förderte handle und verkehr mit teils marktkonformen, teils dirigistischen maßnahmen.";
        var str2 = "ils marktkonformen, teils dirigistischen maßnahmen. an der schwelle zum 19. jahrhundert entstand ein neu";

        var alignment = PartialRatioStrategy<char>.PartialRatioAlignment(str1.AsSpan(), str2.AsSpan());

        Assert.That(alignment, Is.Not.Null);
        Assert.That(alignment.SrcStart, Is.EqualTo(0));
        Assert.That(alignment.SrcEnd, Is.EqualTo(103));
        Assert.That(alignment.DestStart, Is.EqualTo(0));
        Assert.That(alignment.DestEnd, Is.EqualTo(51));
    }
}
