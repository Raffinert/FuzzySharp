using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    private static readonly ParallelOptions DefaultParallelOptions = new ParallelOptions();

    public static partial class Parallel
    {
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(string query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var result = new ExtractedResult<T>[materializedChoices.Count];

            System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
            {
                int score = scorer.Score(query, processor(choice));
                if (score >= cutoff)
                {
                    result[index] = new ExtractedResult<T>(choice, score, (int)index);
                }
            });
            return result.Where(r => r != null);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var processedQuery = processor(query);
            return ExtractWithoutOrder(processedQuery, choices, processor, scorer, cutoff);
        }

        public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).Max();
        }

        public static ExtractedResult<T> ExtractOne<T>(string query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).Max();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(string query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(string query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
        }
    }
}