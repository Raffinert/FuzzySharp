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
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, ICachedRatioScorer scorer, int cutoff = 0)
        {
            int index = 0;
            foreach (var choice in choices)
            {
                var choiceSpan = (extractor?.Invoke(choice) ?? choice as string).AsSpan();
                processor?.Invoke(ref choiceSpan);
                int score = scorer.Score(choiceSpan);
                if (score >= cutoff)
                {
                    yield return new ExtractedResult<T>(choice, score, index);
                }
                index++;
            }
        }

        public static ExtractedResult<T> ExtractOne<T>(IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, ICachedRatioScorer calculator, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, extractor, processor, calculator, cutoff).Max();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, ICachedRatioScorer calculator, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, extractor, processor, calculator, cutoff).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, ICachedRatioScorer calculator, int limit, int cutoff = 0)
        {
            return ExtractWithoutOrder(choices, extractor, processor, calculator, cutoff).MaxN(limit).Reverse();
        }
    }
}