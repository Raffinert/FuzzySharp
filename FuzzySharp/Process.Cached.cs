using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp;

public static partial class Process
{
    public static class Cached
    {
        public static IEnumerable<ExtractedResult<string>> ExtractAll(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            string query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(query);
            foreach (var extractedResult in ResultExtractor.ExtractWithoutOrder(choices, processor, scorer1, cutoff))
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
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer, limit, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer1, limit, cutoff))
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
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer, limit, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractTop(choices, processor, scorer1, limit, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<string>> ExtractSorted(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer, cutoff))
                {
                    yield return extractedResult;
                }
                yield break;
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            foreach (var extractedResult in ResultExtractor.ExtractSorted(choices, processor, scorer1, cutoff))
            {
                yield return extractedResult;
            }
        }

        public static ExtractedResult<string> ExtractOne(
            string query,
            IEnumerable<string> choices,
            Func<string, string> processor = null,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            processor ??= DefaultStringProcessor;
            if (scorer != null)
            {
                return ResultExtractor.ExtractOne(choices, processor, scorer, cutoff);
            }

            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            return ResultExtractor.ExtractOne(choices, processor, scorer1, cutoff);
        }

        public static ExtractedResult<T> ExtractOne<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            ICachedRatioScorer scorer = null,
            int cutoff = 0)
        {
            if (scorer != null)
            {
                return ResultExtractor.ExtractOne(choices, processor, scorer, cutoff);
            }
            using var scorer1 = new CachedWeightedRatioScorer(processor(query));
            return ResultExtractor.ExtractOne(choices, processor, scorer1, cutoff);
        }

        public static ExtractedResult<string> ExtractOne(string query, params string[] choices)
        {
            using var scorer = new CachedWeightedRatioScorer(DefaultStringProcessor(query));
            return ResultExtractor.ExtractOne(choices, DefaultStringProcessor, scorer);
        }
    }
}