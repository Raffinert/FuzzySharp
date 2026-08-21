using Xunit;
using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class RatioTests
{
    private const string S1 = "new york mets", 
        S1A = "new york mets", 
        S2 = "new YORK mets", 
        S3 = "the wonderful new york mets", 
        S4 = "new york mets vs atlanta braves", 
        S5 = "atlanta braves vs new york mets",
        S7 = "new york city mets - atlanta braves", 
        S8 = "{", 
        S8A = "{", 
        S9 = "{a", 
        S9A = "{a", 
        S10 = "a{", 
        S10A = "{b";

    // Edge cases

    [Fact]
    public void Test_Equal()
    {
        Assert.Equal(100, Fuzz.Ratio(S1, S1A));
        Assert.Equal(100, Fuzz.Ratio(S8, S8A));
        Assert.Equal(100, Fuzz.Ratio(S9, S9A));
    }

    [Fact]
    public void Test_Case_Insensitive()
    {
        Assert.NotEqual(100, Fuzz.Ratio(S1, S2));
        Assert.Equal(100, Fuzz.Ratio(S1, S2, StringPreprocessor.Full));
    }

    [Fact]
    public void Test_Partial()
    {
        Assert.Equal(100, Fuzz.PartialRatio(S1, S3));
    }

    [Fact]
    public void TestTokenSortRatio()
    {
        Assert.Equal(100, Fuzz.TokenSortRatio(S1, S1A));
    }

    [Fact]
    public void TestPartialTokenSortRatio()
    {
        Assert.Equal(100, Fuzz.PartialTokenSortRatio(S1, S1A, StringPreprocessor.Full));
        Assert.Equal(100, Fuzz.PartialTokenSortRatio(S4, S5, StringPreprocessor.Full));
        Assert.Equal(100, Fuzz.PartialTokenSortRatio(S8, S8A));
        Assert.Equal(100, Fuzz.PartialTokenSortRatio(S9, S9A, StringPreprocessor.Full));
        Assert.Equal(100, Fuzz.PartialTokenSortRatio(S9, S9A));

        //var al =  Fuzz1.PartialRatioAlignment("a certain string".AsSpan(), "cetain".AsSpan());
            
        Assert.Equal(66.66666666666667, Fuzz.PartialTokenSortRatio(S10, S10A), 10);
        Assert.Equal(0, Fuzz.PartialTokenSortRatio(S10, S10A, StringPreprocessor.Full));
    }

    [Fact]
    public void TestTokenSetRatio()
    {
        Assert.Equal(100, Fuzz.TokenSetRatio(S4, S5, StringPreprocessor.Full));
        Assert.Equal(100, Fuzz.TokenSetRatio(S8, S8A));
        Assert.Equal(100, Fuzz.TokenSetRatio(S9, S9A, StringPreprocessor.Full));
        Assert.Equal(100, Fuzz.TokenSetRatio(S9, S9A));
        Assert.Equal(50, Fuzz.TokenSetRatio(S10, S10A));
    }

    [Fact]
    public void TestTokenAbbreviationRatio()
    {
        Assert.Equal(40, Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full));
        Assert.Equal(66.66666666666667, Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full), 10);
    }

    [Fact]
    public void TestPartialTokenSetRatio()
    {
        Assert.Equal(100, Fuzz.PartialTokenSetRatio(S4, S7));
    }

    [Fact]
    public void TestWeightedRatioEqual()
    {
        Assert.Equal(100, Fuzz.WeightedRatio(S1, S1A));
    }

    [Fact]
    public void TestWeightedRatioCaseInsensitive()
    {
        Assert.Equal(100, Fuzz.WeightedRatio(S1, S2, StringPreprocessor.Full));
    }

    [Fact]
    public void TestWeightedRatioPartialMatch()
    {
        Assert.Equal(90, Fuzz.WeightedRatio(S1, S3));
    }

    [Fact]
    public void TestWeightedRatioMisorderedMatch()
    {
        Assert.Equal(95, Fuzz.WeightedRatio(S4, S5));
    }

    [Fact]
    public void TestEmptyStringsScore0()
    {
        Assert.Equal(0, Fuzz.Ratio("test_string", ""));
        Assert.Equal(0, Fuzz.PartialRatio("test_string", ""));
        Assert.Equal(0, Fuzz.Ratio("", ""));
        Assert.Equal(0, Fuzz.PartialRatio("", ""));
    }

    [Fact]
    public void TestIssueSeven()
    {
        const string s1 = "HSINCHUANG";
        const string s2 = "SINJHUAN";
        const string s3 = "LSINJHUANG DISTRIC";
        const string s4 = "SINJHUANG DISTRICT";

        Assert.True(Fuzz.PartialRatio(s1, s2) > 75);
        Assert.True(Fuzz.PartialRatio(s1, s3) > 75);
        Assert.True(Fuzz.PartialRatio(s1, s4) > 75);
    }

    [Fact]
    public void TestIssueEight()
    {
        // https://github.com/JakeBayer/FuzzySharp/issues/8
        Assert.Equal(100, Fuzz.PartialRatio("Partnernummer", "Partne\nrnum\nmerASDFPartnernummerASDF")); // was 85 
        Assert.Equal(100, Fuzz.PartialRatio("Partnernummer", "PartnerrrrnummerASDFPartnernummerASDF"));  // was 77

        // https://github.com/xdrop/fuzzywuzzy/issues/39
        Assert.Equal(100, Fuzz.PartialRatio("kaution", "kdeffxxxiban:de1110010060046666666datum:16.11.17zeit:01:12uft0000899999tan076601testd.-20-maisonette-z4-jobas-hagkautionauszug")); // was 57

        // https://github.com/seatgeek/fuzzywuzzy/issues/79
        Assert.Equal(100, Fuzz.PartialRatio("this is a test", "is this is a not really thing this is a test!")); // was 92 (actually 93)

        // https://github.com/Raffinert/FuzzySharp/issues/2
        Assert.Equal(100, Fuzz.PartialRatio("sh", "Growing eshops without a popular platform", StringPreprocessor.Full));
        Assert.Equal(100, Fuzz.PartialRatio("shop", "Growing eshops without a popular platform", StringPreprocessor.Full));
    }

    [Fact]
    public void MorePartialRatio()
    {
        Assert.Equal(100, Fuzz.PartialRatio("geeks for geeks", "geeks for geeks!"));
        Assert.Equal(70.58823529411765, Fuzz.PartialRatio("geeks for geeks", "geeks geeks"), 10);
        Assert.Equal(100, Fuzz.TokenSortRatio("geeks for geeks", "for geeks geeks"));
    }

    [Fact]
    public void TestPartialRatioUnicodeString()
    {
        const string s1 = "\u00C1";
        const string s2 = "ABCD";
        var score = Fuzz.PartialRatio(s1, s2);
        Assert.Equal(0, score);
    }

    [Fact]
    public void TestZeroRatio()
    {
        const string s1 = "abc";
        const string s2 = "def";
        var ratio = Fuzz.PartialTokenSortRatio(s1, s2);

        Assert.Equal(0, ratio);
    }

    [Fact]
    public void Test03()
    {
        const string s1 = "new york mets";
        const string s2 = "atlanta braves vs new york mets";
        var ratio = Fuzz.PartialTokenSortRatio(s1, s2);

        Assert.Equal(76.92307692307692, ratio, 10);
    }

    [Fact]
    public void TestRatioUnicodeString()
    {
        const string s1 = "\u00C1";
        const string s2 = "ABCD";
        var score = Fuzz.WeightedRatio(s1, s2);
        Assert.Equal(0, score);

        // Cyrillic.
        const string s3 = "\u043f\u0441\u0438\u0445\u043e\u043b\u043e\u0433";
        const string s4 = "\u043f\u0441\u0438\u0445\u043e\u0442\u0435\u0440\u0430\u043f\u0435\u0432\u0442";
        score = Fuzz.WeightedRatio(s3, s4);
        Assert.NotEqual(0, score);

        // Chinese.
        const string s5 = "\u6211\u4e86\u89e3\u6570\u5b66";
        const string s6 = "\u6211\u5b66\u6570\u5b66";
        score = Fuzz.WeightedRatio(s5, s6);
        Assert.NotEqual(0, score);
    }
}

