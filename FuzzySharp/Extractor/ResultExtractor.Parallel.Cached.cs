using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    public static partial class Parallel
    {
        public static class Cached
        {
            public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                var result = new ExtractedResult<T>[materializedChoices.Count];

                System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
                {
                    int score = scorer.Score(processor(choice));
                    if (score >= cutoff)
                    {
                        result[index] = new ExtractedResult<T>(choice, score, (int)index);
                    }
                });
                return result.Where(r => r != null);
            }

            public static ExtractedResult<T> ExtractOne<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).Max();
            }

            public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
            }

            public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(IEnumerable<T> choices, Func<T, string> processor, ICachedRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
            }
        }
    }
}