using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Internal execution logic for Process operations.
/// When <see cref="ProcessOptions.UseCaching"/> is true, creates a
/// <see cref="CachedWeightedRatioScorer"/> per extraction call and delegates
/// to <see cref="CachedScorerProcessExecutor"/>.
/// Otherwise dispatches to sequential or parallel ResultExtractor paths.
/// </summary>
internal static class ProcessExecutor
{
    #region ExtractAll

    public static IEnumerable<ExtractedResult<string>> ExtractAll(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            return ExtractAllCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractAllCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractAllCached(query, choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractAllCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractAll(
            choices, processor, scorer, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    #endregion

    #region ExtractTop

    public static IEnumerable<ExtractedResult<string>> ExtractTop(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            return ExtractTopCached(processor(query), choices, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                query, choices, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractTopCached(processor(query), choices, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                query, choices, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractTopCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractTop(
            choices, processor, scorer, limit, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    #endregion

    #region ExtractSorted

    public static IEnumerable<ExtractedResult<string>> ExtractSorted(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            return ExtractSortedCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractSortedCached(processor(query), choices, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractSortedCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractSorted(
            choices, processor, scorer, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    #endregion

    #region ExtractOne

    public static ExtractedResult<string> ExtractOne(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= Process.DefaultStringProcessor;

        if (options.UseCaching)
        {
            var processedQuery = processor(query);
            using var cachedScorer = new CachedWeightedRatioScorer(processedQuery);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            var processedQuery = processor(query);
            using var cachedScorer = new CachedWeightedRatioScorer(processedQuery);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                query, choices, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    #endregion
}
