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
            public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer scorer, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                var result = new ExtractedResult<T>[materializedChoices.Count];

                System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
                {
                    double score = scorer.Score(processor(extractor(choice)));
                    if (score >= cutoff)
                    {
                        result[index] = new ExtractedResult<T>(choice, score, (int)index);
                    }
                });
                return result.Where(r => r != null);
            }

            public static IEnumerable<ExtractedResult<string>> ExtractWithoutOrder(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer scorer, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                var result = new ExtractedResult<string>[materializedChoices.Count];

                System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
                {
                    double score = scorer.Score(processor(choice));
                    if (score >= cutoff)
                    {
                        result[index] = new ExtractedResult<string>(choice, score, (int)index);
                    }
                });
                return result.Where(r => r != null);
            }

            public static ExtractedResult<T> ExtractOne<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processor(extractor(choice))), cutoff, parallelOptions);
            }

            public static ExtractedResult<string> ExtractOne(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processor(choice)), cutoff, parallelOptions);
            }

            public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, extractor, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
            }

            public static IEnumerable<ExtractedResult<string>> ExtractSorted(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                return ExtractWithoutOrder(choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
            }

            public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, ICachedRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                var scores = ScoreParallel(materializedChoices, choice => calculator.Score(processor(extractor(choice))), parallelOptions);
                return ExtractTopParallelCore(materializedChoices, scores, limit, cutoff);
            }

            public static IEnumerable<ExtractedResult<string>> ExtractTop(IEnumerable<string> choices, Func<string, string> processor, ICachedRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
            {
                var materializedChoices = choices.ToList();
                var scores = ScoreParallel(materializedChoices, choice => calculator.Score(processor(choice)), parallelOptions);
                return ExtractTopParallelCore(materializedChoices, scores, limit, cutoff);
            }
        }
    }
}
