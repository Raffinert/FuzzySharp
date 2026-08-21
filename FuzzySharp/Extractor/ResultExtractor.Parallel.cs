using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.Utils;
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
        private static ExtractedResult<T> ExtractOneParallelCore<T>(
            IList<T> choices,
            Func<T, double> scoreSelector,
            double cutoff,
            ParallelOptions parallelOptions)
        {
            var sync = new object();
            var globalBest = new BestCandidate<T>();

            System.Threading.Tasks.Parallel.For(
                0,
                choices.Count,
                parallelOptions ?? DefaultParallelOptions,
                () => new BestCandidate<T>(),
                (index, _, localBest) =>
                {
                    var choice = choices[index];
                    localBest.Consider(choice, scoreSelector(choice), index, cutoff);
                    return localBest;
                },
                localBest =>
                {
                    if (localBest.HasValue)
                    {
                        lock (sync)
                        {
                            globalBest.Consider(localBest);
                        }
                    }
                });

            return globalBest.HasValue 
                ? new ExtractedResult<T>(globalBest.Candidate.Value, globalBest.Candidate.Score, globalBest.Candidate.Index) 
                : null;
        }

        private static IEnumerable<ExtractedResult<T>> ExtractTopParallelCore<T>(
            IList<T> choices,
            double[] scores,
            int limit,
            double cutoff)
        {
            var heap = new MinHeap<ScoredCandidate<T>>(ScoredCandidateComparer<T>.Instance);
            var comparer = ScoredCandidateComparer<T>.Instance;

            for (var index = 0; index < scores.Length; index++)
            {
                var score = scores[index];
                if (score >= cutoff)
                {
                    AddTopCandidate(heap, comparer, new ScoredCandidate<T>(choices[index], score, index), limit);
                }
            }

            foreach (var result in CreateTopResults(heap))
            {
                yield return result;
            }
        }

        private static double[] ScoreParallel<T>(
            IList<T> choices,
            Func<T, double> scoreSelector,
            ParallelOptions parallelOptions)
        {
            var scores = new double[choices.Count];

            System.Threading.Tasks.Parallel.For(0, choices.Count, parallelOptions ?? DefaultParallelOptions, index =>
            {
                scores[index] = scoreSelector(choices[index]);
            });

            return scores;
        }

        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var result = new ExtractedResult<T>[materializedChoices.Count];
            var processedQuery = processor(query);

            System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
            {
                double score = scorer.Score(processedQuery, processor(extractor(choice)));
                if (score >= cutoff)
                {
                    result[index] = new ExtractedResult<T>(choice, score, (int)index);
                }
            });
            return result.Where(r => r != null);
        }

        public static IEnumerable<ExtractedResult<string>> ExtractWithoutOrder(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var result = new ExtractedResult<string>[materializedChoices.Count];
            var processedQuery = processor(query);

            System.Threading.Tasks.Parallel.ForEach(materializedChoices, parallelOptions ?? DefaultParallelOptions, (choice, _, index) =>
            {
                double score = scorer.Score(processedQuery, processor(choice));
                if (score >= cutoff)
                {
                    result[index] = new ExtractedResult<string>(choice, score, (int)index);
                }
            });
            return result.Where(r => r != null);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(extractor(query), choices, extractor, processor, scorer, cutoff, parallelOptions);
        }

        public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var processedQuery = processor(extractor(query));
            return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), cutoff, parallelOptions);
        }

        public static ExtractedResult<string> ExtractOne(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var processedQuery = processor(query);
            return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(choice)), cutoff, parallelOptions);
        }

        public static ExtractedResult<T> ExtractOne<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var processedQuery = processor(query);
            return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), cutoff, parallelOptions);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<string>> ExtractSorted(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff, parallelOptions).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var processedQuery = processor(extractor(query));
            var scores = ScoreParallel(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), parallelOptions);
            return ExtractTopParallelCore(materializedChoices, scores, limit, cutoff);
        }

        public static IEnumerable<ExtractedResult<string>> ExtractTop(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var processedQuery = processor(query);
            var scores = ScoreParallel(materializedChoices, choice => calculator.Score(processedQuery, processor(choice)), parallelOptions);
            return ExtractTopParallelCore(materializedChoices, scores, limit, cutoff);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = choices.ToList();
            var processedQuery = processor(query);
            var scores = ScoreParallel(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), parallelOptions);
            return ExtractTopParallelCore(materializedChoices, scores, limit, cutoff);
        }
    }
}
