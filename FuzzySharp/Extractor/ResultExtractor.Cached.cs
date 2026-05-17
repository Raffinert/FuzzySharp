using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    public static class Cached
    {
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            int index = 0;

            foreach (var choice in choices)
            {
                int score = scorer.Score(processor(extractor(choice)));
                if (score >= cutoff)
                {
                    yield return new ExtractedResult<T>(choice, score, index);
                }
                index++;
            }
        }

        public static IEnumerable<ExtractedResult<string>> ExtractWithoutOrder(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            int index = 0;

            foreach (var choice in choices)
            {
                int score = scorer.Score(processor(choice));
                if (score >= cutoff)
                {
                    yield return new ExtractedResult<string>(choice, score, index);
                }
                index++;
            }
        }

        public static ExtractedResult<T> ExtractOne<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, extractor, processor, scorer, cutoff).Max();
        }

        public static ExtractedResult<string> ExtractOne(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, processor, scorer, cutoff).Max();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, extractor, processor, scorer, cutoff).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<string>> ExtractSorted(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, processor, scorer, cutoff).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer scorer, int limit, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, extractor, processor, scorer, cutoff).MaxN(limit).Reverse();
        }

        public static IEnumerable<ExtractedResult<string>> ExtractTop(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer scorer, int limit, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, processor, scorer, cutoff).MaxN(limit).Reverse();
        }
    }
}