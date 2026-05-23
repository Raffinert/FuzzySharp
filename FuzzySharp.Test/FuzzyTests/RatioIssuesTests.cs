using Xunit;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;
using System;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

// Original code https://github.com/rapidfuzz/RapidFuzz/blob/main/tests/test_fuzz.py
public class RatioIssuesTests
{
    [Fact]
    public void Issue76()
    {
        Assert.Equal(82, Fuzz.PartialRatio("physics 2 vid", "study physics physics 2"));
        Assert.Equal(100, Fuzz.PartialRatio("physics 2 vid", "study physics physics 2 video"));
    }

    [Fact]
    public void Issue90()
    {
        Assert.Equal(86, Fuzz.PartialRatio("ax b", "a b a c b"));
    }

    [Fact]
    public void Issue138()
    {
        var str1 = new string('a', 65);
        var str2 = "a" + (char)256 + new string('a', 63);
        Assert.Equal(99, Fuzz.PartialRatio(str1, str2));
    }

    [Fact]
    public void PartialRatioAlignment()
    {
        var a = "a certain string".AsSpan();
        var s = "certain".AsSpan();

        var align1 = PartialRatioStrategy<char>.PartialRatioAlignment(s, a);

        Assert.Equal(100, align1.Score);
        Assert.Equal(0, align1.SrcStart);
        Assert.Equal(s.Length, align1.SrcEnd);
        Assert.Equal(2, align1.DestStart);
        Assert.Equal(2 + s.Length, align1.DestEnd);

        var align2 = PartialRatioStrategy<char>.PartialRatioAlignment(a, s);
        Assert.Equal(100, align2.Score);
        Assert.Equal(2, align2.SrcStart);
        Assert.Equal(2 + s.Length, align2.SrcEnd);
        Assert.Equal(0, align2.DestStart);
        Assert.Equal(s.Length, align2.DestEnd);

        Assert.Equal(0, PartialRatioStrategy<char>.PartialRatioAlignment(null, "test".AsSpan()).Score);
        Assert.Equal(0, PartialRatioStrategy<char>.PartialRatioAlignment("test".AsSpan(), null).Score);
        Assert.Equal(0, PartialRatioStrategy<char>.PartialRatioAlignment("test".AsSpan(), "tesx".AsSpan(), scoreCutoff: 90).Score);
    }

    [Fact]
    public void Issue196()
    {
        Assert.Equal(82, Fuzz.WeightedRatio("South Korea", "North Korea"));
    }

    [Fact]
    public void Issue231()
    {
        var str1 = "er merkantilismus förderte handle und verkehr mit teils marktkonformen, teils dirigistischen maßnahmen.";
        var str2 = "ils marktkonformen, teils dirigistischen maßnahmen. an der schwelle zum 19. jahrhundert entstand ein neu";

        var alignment = PartialRatioStrategy<char>.PartialRatioAlignment(str1.AsSpan(), str2.AsSpan());

        Assert.NotNull(alignment);
        Assert.Equal(0, alignment.SrcStart);
        Assert.Equal(103, alignment.SrcEnd);
        Assert.Equal(0, alignment.DestStart);
        Assert.Equal(51, alignment.DestEnd);
    }
}

