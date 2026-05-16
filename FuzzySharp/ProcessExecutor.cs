using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

internal static class ProcessExecutor
{
    private static string ProcessString(string value, Processor<char> processor)
    {
        var span = value.AsSpan();
        processor(ref span);
        return span.ToString();
    }

    #region ExtractAll

    public static IEnumerable<ExtractedResult<string>> ExtractAll(
        string query,
        IEnumerable<string> choices,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= StringPreprocessors.Full;

        var span = query.AsSpan();
        processor(ref span);

        if (options.UseCaching)
        {
            return ExtractAllCached(span.ToString(), choices, x => x, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                span.ToString(), choices, x => x, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(span, choices, extractor: null, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var processedQuery = ProcessString(extractor(query), processor);

        if (options.UseCaching)
        {
            return ExtractAllCached(processedQuery, choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                processedQuery, choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(
            query,
            choices,
            extractor,
            processor,
            scorer,
            cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var normalizedQuery = ProcessString(query, processor);

        if (options.UseCaching)
        {
            return ExtractAllCached(normalizedQuery, choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractWithoutOrder(
                normalizedQuery,
                choices,
                extractor,
                processor,
                scorer,
                cutoff,
                options.ParallelOptions);
        }

        return ResultExtractor.ExtractWithoutOrder(
            query,
            choices,
            extractor,
            processor,
            scorer,
            cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractAllCached<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        using var cachedScorer = new CachedWeightedRatioScorer(query);
        var results = CachedScorerProcessExecutor.ExtractAll(
            choices, extractor, processor, cachedScorer, cutoff,
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
        Processor<char> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= StringPreprocessors.Full;
        var querySpan = query.AsSpan();
        processor(ref querySpan);

        if (options.UseCaching)
        {
            return ExtractTopCached(querySpan.ToString(), choices, x => x, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                querySpan.ToString(), choices, x => x, processor, scorer, limit, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(query, choices, extractor: null, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var queryStr = extractor(query);
        var querySpan = queryStr.AsSpan();
        processor(ref querySpan);

        if (options.UseCaching)
        {
            return ExtractTopCached(querySpan.ToString(), choices, extractor, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                querySpan.ToString(),
                choices,
                extractor,
                processor,
                scorer,
                limit,
                cutoff,
                options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(
            query,
            choices,
            extractor,
            processor,
            scorer,
            limit,
            cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var normalizedQuery = ProcessString(query, processor);

        if (options.UseCaching)
        {
            return ExtractTopCached(normalizedQuery, choices, extractor, processor, limit, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractTop(
                normalizedQuery,
                choices,
                extractor,
                processor,
                scorer,
                limit,
                cutoff,
                options.ParallelOptions);
        }

        return ResultExtractor.ExtractTop(
            query,
            choices,
            extractor,
            processor,
            scorer,
            limit,
            cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractTopCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int limit,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractTop(
            choices, extractor, processor, scorer, limit, cutoff,
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
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= StringPreprocessors.Full;
        var querySpan = query.AsSpan();
        processor(ref querySpan);

        if (options.UseCaching)
        {
            return ExtractSortedCached(querySpan.ToString(), choices, x => x, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                querySpan.ToString(), choices, x => x, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(query, choices, extractor: null, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var queryStr = extractor(query);
        var querySpan = queryStr.AsSpan();
        processor(ref querySpan);

        if (options.UseCaching)
        {
            return ExtractSortedCached(querySpan.ToString(), choices, extractor, processor, cutoff, options);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractSorted(
                querySpan.ToString(), choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractSorted(
            query,
            choices,
            extractor,
            processor,
            scorer,
            cutoff);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractSortedCached<T>(
        string processedQuery,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        using var scorer = new CachedWeightedRatioScorer(processedQuery);
        var results = CachedScorerProcessExecutor.ExtractSorted(
            choices, extractor, processor, scorer, cutoff,
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
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        processor ??= StringPreprocessors.Full;
        var querySpan = query.AsSpan();
        processor(ref querySpan);

        if (options.UseCaching)
        {
            using var cachedScorer = new CachedWeightedRatioScorer(querySpan.ToString());
            return CachedScorerProcessExecutor.ExtractOne(
                choices, x => x, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                querySpan.ToString(), choices, x => x, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(querySpan.ToString(), choices, extractor: null, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var queryStr = extractor(query);
        var querySpan = queryStr.AsSpan();
        processor(ref querySpan);

        if (options.UseCaching)
        {
            using var cachedScorer = new CachedWeightedRatioScorer(querySpan.ToString());
            return CachedScorerProcessExecutor.ExtractOne(
                choices, extractor, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                querySpan.ToString(), choices, extractor, processor, scorer, cutoff, options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(
            querySpan.ToString(),
            choices,
            extractor,
            processor,
            scorer,
            cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor,
        int cutoff,
        ProcessOptions options)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        processor ??= StringPreprocessors.Full;

        var normalizedQuery = ProcessString(query, processor);

        if (options.UseCaching)
        {
            using var cachedScorer = new CachedWeightedRatioScorer(normalizedQuery);
            return CachedScorerProcessExecutor.ExtractOne(
                choices, extractor, processor, cachedScorer, cutoff,
                options.UseParallel, options.ParallelOptions);
        }

        var scorer = options.Scorer ?? Process.WeightedRatioScorer;

        if (options.UseParallel)
        {
            return ResultExtractor.Parallel.ExtractOne(
                normalizedQuery,
                choices,
                extractor,
                processor,
                scorer,
                cutoff,
                options.ParallelOptions);
        }

        return ResultExtractor.ExtractOne(
            normalizedQuery,
            choices,
            extractor,
            processor,
            scorer,
            cutoff);
    }

    #endregion
}
