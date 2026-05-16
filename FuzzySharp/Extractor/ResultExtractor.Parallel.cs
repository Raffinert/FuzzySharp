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
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            ReadOnlyMemory<char> queryStr = query.ToArray();
            var materializedChoices = choices.ToList();
            var result = new ExtractedResult<T>[materializedChoices.Count];

            System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
            {
                var choiceStr = extractor(choice).AsSpan();
                processor(ref choiceStr);
                int score = scorer.Score(queryStr.Span, choiceStr);
                if (score >= cutoff)
                {
                    result[index] = new ExtractedResult<T>(choice, score, (int)index);
                }
            });
            return result.Where(r => r != null);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer scorer, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var extractedQuery = extractor(query).AsSpan();
            processor(ref extractedQuery);
            return ExtractWithoutOrder(extractedQuery, choices, extractor, processor, scorer, cutoff);
        }

        public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).Max();
        }

        public static ExtractedResult<T> ExtractOne<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            processor(ref query);
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).Max();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            processor(ref query);
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int limit, int cutoff = 0, ParallelOptions parallelOptions = null)
        {
            processor(ref query);
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).MaxN(limit).Reverse();
        }
    }
}