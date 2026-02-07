using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

public static partial class Process
{
    public static partial class Parallel
    {
        public static class Cached
        {
            public static IEnumerable<ExtractedResult<string>> ExtractAll(
                string query,
                IEnumerable<string> choices,
                Func<string, string> processor = null,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                processor ??= DefaultStringProcessor;
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer1, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
                T query,
                IEnumerable<T> choices,
                Func<T, string> processor,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer1, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
                string query,
                IEnumerable<T> choices,
                Func<T, string> processor,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(query);
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractWithoutOrder(choices, processor, scorer1, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static IEnumerable<ExtractedResult<string>> ExtractTop(
                string query,
                IEnumerable<string> choices,
                Func<string, string> processor = null,
                ICachedRatioScorer scorer = null,
                int limit = 5,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                processor ??= DefaultStringProcessor;
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractTop(choices, processor, scorer, limit, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractTop(choices, processor, scorer1, limit, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
                T query,
                IEnumerable<T> choices,
                Func<T, string> processor,
                ICachedRatioScorer scorer = null,
                int limit = 5,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractTop(choices, processor, scorer, limit, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractTop(choices, processor, scorer1, limit, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static IEnumerable<ExtractedResult<string>> ExtractSorted(
                string query,
                IEnumerable<string> choices,
                Func<string, string> processor = null,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                processor ??= DefaultStringProcessor;
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractSorted(choices, processor, scorer, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractSorted(choices, processor, scorer1, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
                T query,
                IEnumerable<T> choices,
                Func<T, string> processor,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                if (scorer != null)
                {
                    foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractSorted(choices, processor, scorer, cutoff, parallelOptions))
                    {
                        yield return extractedResult;
                    }
                    yield break;
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                foreach (var extractedResult in ResultExtractor.Parallel.Cached.ExtractSorted(choices, processor, scorer1, cutoff, parallelOptions))
                {
                    yield return extractedResult;
                }
            }

            public static ExtractedResult<string> ExtractOne(
                string query,
                IEnumerable<string> choices,
                Func<string, string> processor = null,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                processor ??= DefaultStringProcessor;
                if (scorer != null)
                {
                    return ResultExtractor.Parallel.Cached.ExtractOne(choices, processor, scorer, cutoff, parallelOptions);
                }

                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                return ResultExtractor.Parallel.Cached.ExtractOne(choices, processor, scorer1, cutoff, parallelOptions);
            }

            public static ExtractedResult<T> ExtractOne<T>(
                T query,
                IEnumerable<T> choices,
                Func<T, string> processor,
                ICachedRatioScorer scorer = null,
                int cutoff = 0,
                ParallelOptions parallelOptions = null)
            {
                if (scorer != null)
                {
                    return ResultExtractor.Parallel.Cached.ExtractOne(choices, processor, scorer, cutoff, parallelOptions);
                }
                using var scorer1 = new CachedWeightedRatioScorer(processor(query));
                return ResultExtractor.Parallel.Cached.ExtractOne(choices, processor, scorer1, cutoff, parallelOptions);
            }

            public static ExtractedResult<string> ExtractOne(string query, ParallelOptions parallelOptions = null, params string[] choices)
            {
                using var scorer = new CachedWeightedRatioScorer(DefaultStringProcessor(query));
                return ResultExtractor.Parallel.Cached.ExtractOne(choices, DefaultStringProcessor, scorer, parallelOptions: parallelOptions);
            }
        }
    }
}