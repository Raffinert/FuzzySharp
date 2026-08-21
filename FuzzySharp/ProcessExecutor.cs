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
        double cutoff,
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

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractAllCached(extractor(query), choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor)); 
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractAllCached(query, choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                query, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractAllCached<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processor(query));
        var results = CachedScorerProcessExecutor.ExtractAll(
            choices, extractor, processor, scorer, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    private static IEnumerable<ExtractedResult<string>> ExtractAllCached(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processor(query));
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
        double cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractTopCached(query, choices, processor, limit, cutoff, options);
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
        Func<T, string> extractor,
        Func<string, string> processor,
        int limit,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        var extractedQuery = extractor(query);

        if (options.UseCaching)
        {
            return ExtractTopCached(extractedQuery, choices, extractor, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                extractedQuery, choices, extractor, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(extractedQuery, choices, extractor, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        int limit,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractTopCached(query, choices, extractor, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                query, choices, extractor, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(query, choices, extractor, processor, scorer, limit, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractTopCached<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        int limit,
        double cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processor(query));
        var results = CachedScorerProcessExecutor.ExtractTop(
            choices, extractor, processor, scorer, limit, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    private static IEnumerable<ExtractedResult<string>> ExtractTopCached(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        int limit,
        double cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processor(query));
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
        double cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractSortedCached(query, choices, processor, cutoff, options);
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
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractSortedCached(extractor(query), choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                query, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, extractor, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            return ExtractSortedCached(query, choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                query, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, extractor, processor, scorer, cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractSortedCached<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processor(query));
        var results = CachedScorerProcessExecutor.ExtractSorted(
            choices, extractor, processor, scorer, cutoff,
            options.UseParallel, options.ParallelOptions);

        foreach (var result in results)
        {
            yield return result;
        }
    }

    private static IEnumerable<ExtractedResult<string>> ExtractSortedCached(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processor(query));
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
        double cutoff,
        ProcessOptions options)
    {
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            using var cachedScorer = new CachedWeightedRatioScorer(processor(query));
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
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            var processedQuery = extractor(query);
            using var cachedScorer = new CachedWeightedRatioScorer(processedQuery);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, extractor, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                query, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(query, choices, extractor, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        double cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (processor == null) throw new ArgumentNullException(nameof(processor));

        if (options.UseCaching)
        {
            using var cachedScorer = new CachedWeightedRatioScorer(query);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, extractor, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.DefaultScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                query, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(query, choices, extractor, processor, scorer, cutoff);
    }

    #endregion
}
