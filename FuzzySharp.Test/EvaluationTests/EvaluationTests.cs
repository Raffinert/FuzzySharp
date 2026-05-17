using NUnit.Framework;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Test.EvaluationTests;

[TestFixture]
public class EvaluationTests
{
    [Test]
    public void Evaluate()
    {
        var a1 = Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
        var a2 = Fuzz.Ratio("mysmilarstring", "mysimilarstring");

        var b1 = Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");

        var c1 = Fuzz.TokenSortRatio("order words out of", "  words out of order");
        var c2 = Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");

        var d1 = Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
        var d2 = Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");

        var e1 = Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");

        var f1 = Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
        var f2 = Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");

        var f3 = Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
        var f4 = Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");

        var g1 = Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
        var g2 = Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);

        var cached = Process.Configure().Cached().Build();

        var h1 = Process.ExtractOne("cowboys", ["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"]);
        var h11 = cached.ExtractOne("cowboys", ["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"]);
        var h2 = string.Join(", ", Process.ExtractTop("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], limit: 3));
        var h21 = string.Join(", ", cached.ExtractTop("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], limit: 3));
        var h3 = string.Join(", ", Process.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));
        var h31 = string.Join(", ", cached.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));
        var h4 = string.Join(", ", Process.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], cutoff: 40));
        var h41 = string.Join(", ", cached.ExtractAll("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"], cutoff: 40));
        var h5 = string.Join(", ", Process.ExtractSorted("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));
        var h51 = string.Join(", ", cached.ExtractSorted("goolge", ["google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl"]));

        var i1 = Process.ExtractOne("cowboys", ["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"], StringPreprocessor.None, ScorerCache.Get<DefaultRatioScorer>());
        using var cachedRatioScorer = new CachedDefaultRatioScorer("cowboys");
        var i11 = Process.Configure().Cached(cachedRatioScorer).Build()
            .ExtractOne(["Atlanta Falcons", "New York Jets", "New York Giants", "Dallas Cowboys"], processor: StringPreprocessor.None);

        var i12 = Process.Configure().Cached(cachedRatioScorer).Build()
            .ExtractTop([new { Name = "Atlanta Falcons" }, new { Name = "New York Jets" }, new { Name = "New York Giants" }, new { Name = "Dallas Cowboys" }], s => s.Name);

        string[][] events =
        [
            ["chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm"],
            ["new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm"],
            ["atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm"]
        ];
        var query = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };

        var best = Process.ExtractOne(query, events, strings => strings[0]);
        var best1 = cached.ExtractOne(query, events, strings => strings[0]);

        var ratio = ScorerCache.Get<DefaultRatioScorer>();
        var partial = ScorerCache.Get<PartialRatioScorer>();
        var tokenSet = ScorerCache.Get<TokenSetScorer>();
        var partialTokenSet = ScorerCache.Get<PartialTokenSetScorer>();
        var tokenSort = ScorerCache.Get<TokenSortScorer>();
        var partialTokenSort = ScorerCache.Get<PartialTokenSortScorer>();
        var tokenAbbreviation = ScorerCache.Get<TokenAbbreviationScorer>();
        var partialTokenAbbreviation = ScorerCache.Get<PartialTokenAbbreviationScorer>();
        var weighted = ScorerCache.Get<WeightedRatioScorer>();
    }

    [Test]
    public void TokenInitialismScorer_WhenGivenStringWithTrailingSpaces_DoesNotBreak()
    {
        // arrange
        var longer = "lusiki plaza share block ";
        var shorter = "jmft";

        // act
        var ratio = Fuzz.TokenInitialismRatio(shorter, longer);

        // assert
        Assert.IsTrue(ratio >= 0);
    }

    [Theory]
    [TestCase("+30.0% Damage to Close Enemies [30.01%", 1)]
    [TestCase("+14.3% Damage to Crowd Controlled Enemies [7.5 - 18.0]%", 2)]
    public void FullPreprocessor(string input, int caseNumber)
    {
        // Arrange
        List<string> choices =
        [
            "+#% Damage",
            "+#% Damage to Crowd Controlled Enemies",
            "+#% Damage to Close Enemies",
            "+#% Damage to Chilled Enemies",
            "+#% Damage to Poisoned Enemies",
            "#% Block Chance#% Blocked Damage Reduction",
            "#% Damage Reduction from Bleeding Enemies",
            "#% Damage Reduction",
            "+#% Cold Damage"
        ];

        // Act
        var cachedResults = Process.Configure().Cached().Build().ExtractTop(input, choices, limit: 2).ToArray();
        var cachedParallelResults = Process.Configure().Cached().Parallel().Build().ExtractTop(input, choices, limit: 2).ToArray();
        var regularResults = Process.ExtractTop(input, choices, limit: 2).ToArray();

        // Assert
        Assert.IsNotEmpty(cachedResults);
        Assert.IsNotEmpty(cachedParallelResults);
        Assert.IsNotEmpty(regularResults);
        Assert.AreEqual(regularResults[0].Value, cachedResults[0].Value, $"Case {caseNumber}: top match differs");
        Assert.AreEqual(regularResults[0].Score, cachedResults[0].Score, $"Case {caseNumber}: top score differs");
        Assert.AreEqual(cachedParallelResults[0].Value, cachedResults[0].Value, $"Case {caseNumber}: top match differs");
        Assert.AreEqual(cachedParallelResults[0].Score, cachedResults[0].Score, $"Case {caseNumber}: top score differs");
    }

    [Theory]
    [TestCase("+30.0% Damage to Close Enemies [30.01%", 1)]
    [TestCase("+14.3% Damage to Crowd Controlled Enemies [7.5 - 18.0]%", 2)]
    public void NonePreprocessor(string input, int caseNumber)
    {
        // Arrange
        List<string> choices =
        [
            "+#% Damage",
            "+#% Damage to Crowd Controlled Enemies",
            "+#% Damage to Close Enemies",
            "+#% Damage to Chilled Enemies",
            "+#% Damage to Poisoned Enemies",
            "#% Block Chance#% Blocked Damage Reduction",
            "#% Damage Reduction from Bleeding Enemies",
            "#% Damage Reduction",
            "+#% Cold Damage"
        ];

        // Act
        var regularResults = Process.ExtractTop(input, choices, StringPreprocessor.None, limit: 2).ToArray();
        var cachedParallelResults = Process.Configure().Cached().Parallel().Build().ExtractTop(input, choices, StringPreprocessor.None, limit: 2).ToArray();
        var cachedResults = Process.Configure().Cached().Build().ExtractTop(input, choices, processor: StringPreprocessor.None, limit: 2).ToArray();

        // Assert
        Assert.IsNotEmpty(cachedResults);
        Assert.IsNotEmpty(cachedParallelResults);
        Assert.IsNotEmpty(regularResults);
        Assert.AreEqual(regularResults[0].Value, cachedResults[0].Value, $"Case {caseNumber}: top match differs");
        Assert.AreEqual(regularResults[0].Score, cachedResults[0].Score, $"Case {caseNumber}: top score differs");
        Assert.AreEqual(cachedParallelResults[0].Value, cachedResults[0].Value, $"Case {caseNumber}: top match differs");
        Assert.AreEqual(cachedParallelResults[0].Score, cachedResults[0].Score, $"Case {caseNumber}: top score differs");
    }
}
