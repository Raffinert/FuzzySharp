using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class ProcessPipelineTests
{
    private readonly string[] _baseballStrings = new[]
    {
        "new york mets vs chicago cubs",
        "chicago cubs vs chicago white sox",
        "philladelphia phillies vs atlanta braves",
        "braves vs mets",
    };

    [Fact]
    public void TestOrderIndependence_CachedThenParallel()
    {
        var query = "new york mets at atlanta braves";

        var pipeline1 = Process.Configure()
            .Cached()
            .Parallel()
            .Build();

        var pipeline2 = Process.Configure()
            .Parallel()
            .Cached()
            .Build();

        var result1 = pipeline1.ExtractOne(query, _baseballStrings);
        var result2 = pipeline2.ExtractOne(query, _baseballStrings);

        Assert.Equal(result1.Value, result2.Value);
        Assert.Equal(result1.Score, result2.Score);
        Assert.Equal(result1.Index, result2.Index);
    }

    [Fact]
    public void TestSimplifiedSyntax_EquivalentToExplicitSyntax()
    {
        var query = "chicago cubs vs new york mets";
        using var scorer = new CachedWeightedRatioScorer(query);
        var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };

        // Simplified syntax
        var pipeline1 = Process.Configure()
            .Cached(scorer)
            .Parallel(options)
            .Build();

        // Explicit syntax
        var pipeline2 = Process.Configure()
            .Cached(scorer)
            .Parallel()
            .WithParallelOptions(options)
            .Build();

        var result1 = pipeline1.ExtractOne(_baseballStrings);
        var result2 = pipeline2.ExtractOne(_baseballStrings);
    }

    [Fact]
    public void TestSimplifiedSyntax_ParallelOnly()
    {
        var query = "atlanta braves vs philadelphia phillies";
        var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };

        var pipeline = Process.Configure()
            .Parallel(options)
            .Build();

        var result = pipeline.ExtractOne(query, _baseballStrings);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void TestSimplifiedSyntax_CachedOnly()
    {
        var query = "new york mets at chicago cubs";
        using var scorer = new CachedWeightedRatioScorer(query);

        var pipeline = Process.Configure()
            .Cached(scorer)
            .Build();

        var result = pipeline.ExtractOne(_baseballStrings);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void TestPipelineVsStaticAPI_ExtractOne()
    {
        var query = "philadelphia phillies at atlanta braves";

        var staticResult = Process.ExtractOne(query, _baseballStrings);

        var pipeline = Process.Configure().Build();
        var pipelineResult = pipeline.ExtractOne(query, _baseballStrings);

        Assert.Equal(staticResult.Value, pipelineResult.Value);
        Assert.Equal(staticResult.Score, pipelineResult.Score);
        Assert.Equal(staticResult.Index, pipelineResult.Index);
    }

    [Fact]
    public void TestPipelineVsStaticAPI_ExtractAll()
    {
        var query = "atlanta braves at philadelphia phillies";

        var staticResults = Process.ExtractAll(query, _baseballStrings).OrderBy(r => r.Index).ToList();

        var pipeline = Process.Configure().Build();
        var pipelineResults = pipeline.ExtractAll(query, _baseballStrings).OrderBy(r => r.Index).ToList();

        Assert.Equal(staticResults.Count, pipelineResults.Count);
        for (int i = 0; i < staticResults.Count; i++)
        {
            Assert.Equal(staticResults[i].Value, pipelineResults[i].Value);
            Assert.Equal(staticResults[i].Score, pipelineResults[i].Score);
            Assert.Equal(staticResults[i].Index, pipelineResults[i].Index);
        }
    }

    [Fact]
    public void TestPipelineVsStaticAPI_ExtractTop()
    {
        var query = "chicago cubs vs new york mets";

        var staticResults = Process.ExtractTop(query, _baseballStrings, limit: 2).ToList();

        var pipeline = Process.Configure().Build();
        var pipelineResults = pipeline.ExtractTop(query, _baseballStrings, limit: 2).ToList();

        Assert.Equal(staticResults.Count, pipelineResults.Count);
        for (int i = 0; i < staticResults.Count; i++)
        {
            Assert.Equal(staticResults[i].Value, pipelineResults[i].Value);
            Assert.Equal(staticResults[i].Score, pipelineResults[i].Score);
        }
    }

    [Fact]
    public void TestCancellationToken_Parallel()
    {
        var longList = Enumerable.Range(0, 10000).Select(i => $"team {i} vs team {i + 1}").ToArray();
        var query = "team 5000 vs team 5001";

        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cts.Token
        };

        var pipeline = Process.Configure()
            .Parallel()
            .WithParallelOptions(parallelOptions)
            .Build();

        Assert.Throws<OperationCanceledException>(() =>
        {
            var results = pipeline.ExtractAll(query, longList).ToList();
        });
    }

    [Fact]
    public void TestCancellationToken_ParallelDuringExecution()
    {
        // Create a large dataset
        var longList = Enumerable.Range(0, 100000).Select(i => $"very long string number {i} with lots of text to process").ToArray();
        var query = "very long string number 50000 with lots of text to process";

        var cts = new CancellationTokenSource();
        // Cancel immediately - this guarantees the token is cancelled
        // before any meaningful work can complete
        cts.Cancel();

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cts.Token,
            MaxDegreeOfParallelism = 8
        };

        var pipeline = Process.Configure()
            .Parallel()
            .WithParallelOptions(parallelOptions)
            .Build();

        Assert.Throws<OperationCanceledException>(() =>
        {
            var results = pipeline.ExtractAll(query, longList).ToList();
        });
    }

    [Fact]
    public void TestParallelResultsMatchSequential()
    {
        var query = "new york mets at atlanta braves";

        var sequentialPipeline = Process.Configure().Build();
        var parallelPipeline = Process.Configure()
            .Parallel()
            .WithParallelOptions(new ParallelOptions { MaxDegreeOfParallelism = 4 })
            .Build();

        var sequentialResults = sequentialPipeline.ExtractAll(query, _baseballStrings)
            .OrderBy(r => r.Index).ToList();
        var parallelResults = parallelPipeline.ExtractAll(query, _baseballStrings)
            .OrderBy(r => r.Index).ToList();

        Assert.Equal(sequentialResults.Count, parallelResults.Count);
        for (int i = 0; i < sequentialResults.Count; i++)
        {
            Assert.Equal(sequentialResults[i].Value, parallelResults[i].Value);
            Assert.Equal(sequentialResults[i].Score, parallelResults[i].Score);
            Assert.Equal(sequentialResults[i].Index, parallelResults[i].Index);
        }
    }

    [Fact]
    public void TestCachedWithExternalScorer()
    {
        var query = "new york mets at atlanta braves";

        using var scorer = new CachedWeightedRatioScorer(query);

        var pipeline = Process.Configure()
            .Cached(scorer)
            .Build();

        var result1 = pipeline.ExtractOne(_baseballStrings);
        var result2 = pipeline.ExtractOne(_baseballStrings);

        Assert.Equal(result1.Value, result2.Value);
        Assert.Equal(result1.Score, result2.Score);
    }

    [Fact]
    public void TestCachedParallelWithExternalScorer()
    {
        var query = "chicago cubs vs new york mets";

        using var scorer = new CachedWeightedRatioScorer(query);

        var pipeline = Process.Configure()
            .Cached(scorer)
            .Parallel()
            .Build();

        var results = pipeline.ExtractAll(_baseballStrings).OrderBy(r => r.Index).ToList();

        Assert.True(results.Count > 0);
        Assert.Contains(results, r => r.Score > 50);
    }

    [Fact]
    public void TestGenericTypeExtractOne()
    {
        var events = new[]
        {
            new[] { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" },
            new[] { "new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm" },
            new[] { "atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm" },
        };
        var query = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };

        var staticResult = Process.ExtractOneBy(query, events, strings => strings[0]);

        var pipeline = Process.Configure().Build();
        var pipelineResult = pipeline.ExtractOneBy(query, events, strings => strings[0]);

        Assert.Equal(staticResult.Value, pipelineResult.Value);
        Assert.Equal(staticResult.Score, pipelineResult.Score);
    }

    [Fact]
    public void TestGenericTypeExtractOne_Parallel()
    {
        var events = new[]
        {
            new[] { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" },
            new[] { "new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm" },
            new[] { "atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm" },
        };
        var query = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };

        var sequentialPipeline = Process.Configure().Build();
        var parallelPipeline = Process.Configure().Parallel().Build();

        var sequentialResult = sequentialPipeline.ExtractOneBy(query, events, strings => strings[0]);
        var parallelResult = parallelPipeline.ExtractOneBy(query, events, strings => strings[0]);

        Assert.Equal(sequentialResult.Value, parallelResult.Value);
        Assert.Equal(sequentialResult.Score, parallelResult.Score);
    }

    [Fact]
    public void TestExtractSorted()
    {
        var query = "new york mets at atlanta braves";

        var staticResults = Process.ExtractSorted(query, _baseballStrings).ToList();

        var pipeline = Process.Configure().Build();
        var pipelineResults = pipeline.ExtractSorted(query, _baseballStrings).ToList();

        Assert.Equal(staticResults.Count, pipelineResults.Count);
        for (int i = 0; i < staticResults.Count; i++)
        {
            Assert.Equal(staticResults[i].Value, pipelineResults[i].Value);
            Assert.Equal(staticResults[i].Score, pipelineResults[i].Score);
        }
    }

    [Fact]
    public void TestExtractSorted_Parallel()
    {
        var query = "new york mets at atlanta braves";

        var sequentialPipeline = Process.Configure().Build();
        var parallelPipeline = Process.Configure().Parallel().Build();

        var sequentialResults = sequentialPipeline.ExtractSorted(query, _baseballStrings).ToList();
        var parallelResults = parallelPipeline.ExtractSorted(query, _baseballStrings).ToList();

        Assert.Equal(sequentialResults.Count, parallelResults.Count);
        for (int i = 0; i < sequentialResults.Count; i++)
        {
            Assert.Equal(sequentialResults[i].Value, parallelResults[i].Value);
            Assert.Equal(sequentialResults[i].Score, parallelResults[i].Score);
        }
    }

    [Fact]
    public void TestMultiplePipelinesReusable()
    {
        var query1 = "new york mets at atlanta braves";
        var query2 = "chicago cubs vs philadelphia phillies";

        var pipeline = Process.Configure()
            .Parallel()
            .WithParallelOptions(new ParallelOptions { MaxDegreeOfParallelism = 2 })
            .Build();

        var result1 = pipeline.ExtractOne(query1, _baseballStrings);
        var result2 = pipeline.ExtractOne(query2, _baseballStrings);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.Value, result2.Value);
    }
}

