using System.Linq;
using NUnit.Framework;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

/// <summary>
/// Tests for the overloads that accept a plain <c>string query</c> paired with
/// a generic <c>IEnumerable&lt;T&gt; choices</c> and a <c>Func&lt;T, string&gt; processor</c>.
/// Covers <see cref="Process.ExtractAllBy{T}(string, System.Collections.Generic.IEnumerable{T}, System.Func{T,string}, Raffinert.FuzzySharp.SimilarityRatio.Scorer.IRatioScorer, int)"/>,
/// <see cref="Process.ExtractTopBy{T}(string, System.Collections.Generic.IEnumerable{T}, System.Func{T,string}, Raffinert.FuzzySharp.SimilarityRatio.Scorer.IRatioScorer, int, int)"/>,
/// <see cref="Process.ExtractSortedBy{T}(string, System.Collections.Generic.IEnumerable{T}, System.Func{T,string}, Raffinert.FuzzySharp.SimilarityRatio.Scorer.IRatioScorer, int)"/>, and
/// <see cref="Process.ExtractOneBy{T}(string, System.Collections.Generic.IEnumerable{T}, System.Func{T,string}, Raffinert.FuzzySharp.SimilarityRatio.Scorer.IRatioScorer, int)"/>,
/// as well as the equivalent <see cref="ProcessPipeline"/> methods.
/// </summary>
[TestFixture]
public class ProcessStringQueryGenericChoicesTests
{
    private class Event
    {
        public string Name { get; }
        public string Venue { get; }
        public Event(string name, string venue) { Name = name; Venue = venue; }
        public override string ToString() => $"{Name} @ {Venue}";
    }

    private Event[] _events;
    private string _query;

    [SetUp]
    public void Setup()
    {
        _events = new[]
        {
            new Event("new york mets vs chicago cubs", "CitiField"),
            new Event("chicago cubs vs chicago white sox", "Wrigley Field"),
            new Event("philadelphia phillies vs atlanta braves", "Citizens Bank Park"),
            new Event("braves vs mets", "Turner Field"),
        };

        _query = "new york mets at chicago cubs";
    }

    // -------------------------------------------------------------------------
    // Process.ExtractAll<T>(string query, ...)
    // -------------------------------------------------------------------------

    [Test]
    public void ExtractAllBy_StringQuery_GenericChoices_ReturnsAllAboveCutoff()
    {
        var results = Process.ExtractAllBy(_query, _events, e => e.Name, cutoff: 50).ToList();

        Assert.IsNotEmpty(results);
        Assert.IsTrue(results.All(r => r.Score >= 50));
    }

    [Test]
    public void ExtractAllBy_StringQuery_GenericChoices_NoCutoff_ReturnsAllChoices()
    {
        var results = Process.ExtractAllBy(_query, _events, e => e.Name).ToList();

        Assert.AreEqual(_events.Length, results.Count);
    }

    [Test]
    public void ExtractAllBy_StringQuery_GenericChoices_BestMatchIsCorrect()
    {
        var results = Process.ExtractAllBy(_query, _events, e => e.Name).ToList();
        var best = results.OrderByDescending(r => r.Score).First();

        Assert.AreEqual(_events[0], best.Value);
    }

    // -------------------------------------------------------------------------
    // Process.ExtractTop<T>(string query, ...)
    // -------------------------------------------------------------------------

    [Test]
    public void ExtractTopBy_StringQuery_GenericChoices_RespectsLimit()
    {
        var results = Process.ExtractTopBy(_query, _events, e => e.Name, limit: 2).ToList();

        Assert.AreEqual(2, results.Count);
    }

    [Test]
    public void ExtractTopBy_StringQuery_GenericChoices_ResultsAreSortedDescending()
    {
        var results = Process.ExtractTopBy(_query, _events, e => e.Name, limit: 3).ToList();

        for (int i = 1; i < results.Count; i++)
        {
            Assert.GreaterOrEqual(results[i - 1].Score, results[i].Score);
        }
    }

    [Test]
    public void ExtractTopBy_StringQuery_GenericChoices_BestMatchIsCorrect()
    {
        var results = Process.ExtractTopBy(_query, _events, e => e.Name, limit: 1).ToList();

        Assert.AreEqual(_events[0], results[0].Value);
    }

    // -------------------------------------------------------------------------
    // Process.ExtractSorted<T>(string query, ...)
    // -------------------------------------------------------------------------

    [Test]
    public void ExtractSortedBy_StringQuery_GenericChoices_ResultsAreSortedDescending()
    {
        var results = Process.ExtractSortedBy(_query, _events, e => e.Name).ToList();

        for (int i = 1; i < results.Count; i++)
        {
            Assert.GreaterOrEqual(results[i - 1].Score, results[i].Score);
        }
    }

    [Test]
    public void ExtractSortedBy_StringQuery_GenericChoices_BestMatchIsFirst()
    {
        var results = Process.ExtractSortedBy(_query, _events, e => e.Name).ToList();

        Assert.AreEqual(_events[0], results[0].Value);
    }

    [Test]
    public void ExtractSortedBy_StringQuery_GenericChoices_CutoffFiltersResults()
    {
        var results = Process.ExtractSortedBy("zzzzzz completely unrelated", _events, e => e.Name, cutoff: 90).ToList();

        Assert.IsEmpty(results);
    }

    // -------------------------------------------------------------------------
    // Process.ExtractOne<T>(string query, ...)
    // -------------------------------------------------------------------------

    [Test]
    public void ExtractOneBy_StringQuery_GenericChoices_ReturnsBestMatch()
    {
        var result = Process.ExtractOneBy(_query, _events, e => e.Name);

        Assert.AreEqual(_events[0], result.Value);
    }

    [Test]
    public void ExtractOneBy_StringQuery_GenericChoices_IndexIsCorrect()
    {
        var result = Process.ExtractOneBy(_query, _events, e => e.Name);

        Assert.AreEqual(0, result.Index);
    }

    [Test]
    public void ExtractOneBy_StringQuery_GenericChoices_ScoreIsPositive()
    {
        var result = Process.ExtractOneBy(_query, _events, e => e.Name);

        Assert.Greater(result.Score, 0);
    }

    // -------------------------------------------------------------------------
    // ProcessPipeline equivalents (via Process.Configure())
    // -------------------------------------------------------------------------

    [Test]
    public void Pipeline_ExtractAllBy_StringQuery_GenericChoices_MatchesProcess()
    {
        var pipeline = Process.Configure().Build();

        var direct   = Process.ExtractAllBy(_query, _events, e => e.Name).OrderBy(r => r.Index).ToList();
        var piped    = pipeline.ExtractAllBy(_query, _events, e => e.Name).OrderBy(r => r.Index).ToList();

        Assert.AreEqual(direct.Count, piped.Count);
        for (int i = 0; i < direct.Count; i++)
        {
            Assert.AreEqual(direct[i].Value, piped[i].Value);
            Assert.AreEqual(direct[i].Score, piped[i].Score);
        }
    }

    [Test]
    public void Pipeline_ExtractTopBy_StringQuery_GenericChoices_MatchesProcess()
    {
        var pipeline = Process.Configure().Build();

        var direct = Process.ExtractTopBy(_query, _events, e => e.Name, limit: 2).ToList();
        var piped  = pipeline.ExtractTopBy(_query, _events, e => e.Name, limit: 2).ToList();

        Assert.AreEqual(direct.Count, piped.Count);
        for (int i = 0; i < direct.Count; i++)
        {
            Assert.AreEqual(direct[i].Value, piped[i].Value);
        }
    }

    [Test]
    public void Pipeline_ExtractOneBy_StringQuery_GenericChoices_MatchesProcess()
    {
        var pipeline = Process.Configure().Build();

        var direct = Process.ExtractOneBy(_query, _events, e => e.Name);
        var piped  = pipeline.ExtractOneBy(_query, _events, e => e.Name);

        Assert.AreEqual(direct.Value, piped.Value);
        Assert.AreEqual(direct.Score, piped.Score);
    }
}
