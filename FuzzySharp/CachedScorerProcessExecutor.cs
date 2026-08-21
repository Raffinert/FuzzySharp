using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Internal execution logic for cached Process operations with a pre-initialized scorer.
/// The scorer already has the query baked in — methods here take no query parameter.
/// Dispatches to sequential or parallel cached ResultExtractor paths.
/// </summary>
internal static class CachedScorerProcessExecutor
{
    public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractWithoutOrder(
                choices, extractor, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractWithoutOrder(choices, extractor, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<string>> ExtractAll(
        IEnumerable<string> choices,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractWithoutOrder(choices, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        int limit,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractTop(
                choices, extractor, processor, scorer, limit, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractTop(choices, extractor, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<string>> ExtractTop(
        IEnumerable<string> choices,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        int limit,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractTop(
                choices, processor, scorer, limit, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractTop(choices, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractSorted(
                choices, extractor, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractSorted(choices, extractor, processor, scorer, cutoff);
    }

    public static IEnumerable<ExtractedResult<string>> ExtractSorted(
            IEnumerable<string> choices,
            Func<string, string> processor,
            ICachedRatioScorer scorer,
            double cutoff,
            bool useParallel,
            ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractSorted(choices, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractSorted(choices, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractOne(
                choices, extractor, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractOne(choices, extractor, processor, scorer, cutoff);
    }

    public static ExtractedResult<string> ExtractOne(
        IEnumerable<string> choices,
        Func<string, string> processor,
        ICachedRatioScorer scorer,
        double cutoff,
        bool useParallel,
        ParallelOptions parallelOptions)
    {
        if (useParallel)
        {
            return ResultExtractor.Parallel.Cached.ExtractOne(
                choices, processor, scorer, cutoff, parallelOptions);
        }

        return ResultExtractor.Cached.ExtractOne(choices, processor, scorer, cutoff);
    }
}
