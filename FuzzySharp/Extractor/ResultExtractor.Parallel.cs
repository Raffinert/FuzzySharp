using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    private static readonly ParallelOptions DefaultParallelOptions = new ParallelOptions();

    public static partial class Parallel
    {
        private static IList<T> GetIndexedChoices<T>(IEnumerable<T> choices) =>
            choices as IList<T> ?? choices.ToList();

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
            Func<T, double> scoreSelector,
            int limit,
            double cutoff,
            ParallelOptions parallelOptions)
        {
            ValidateLimit(limit);
            if (limit == 0)
                return Enumerable.Empty<ExtractedResult<T>>();

            if (choices.Count == 0)
                return Enumerable.Empty<ExtractedResult<T>>();

            var scores = ArrayPool<double>.Shared.Rent(choices.Count);
            var comparer = ScoredCandidateComparer<int>.Instance;
            try
            {
                System.Threading.Tasks.Parallel.For(0, choices.Count, parallelOptions ?? DefaultParallelOptions, index =>
                {
                    scores[index] = scoreSelector(choices[index]);
                });

                var heap = new MinHeap<ScoredCandidate<int>>(comparer);
                for (var index = 0; index < choices.Count; index++)
                {
                    var score = scores[index];
                    if (score >= cutoff)
                        AddTopCandidate(heap, comparer, new ScoredCandidate<int>(index, score, index), limit);
                }

                var selected = new ScoredCandidate<int>[heap.Count];
                for (var i = selected.Length - 1; i >= 0; i--)
                    selected[i] = heap.ExtractDominating();

                return CreateParallelTopResults(choices, selected);
            }
            finally
            {
                ArrayPool<double>.Shared.Return(scores);
            }
        }

        private static IEnumerable<ExtractedResult<T>> CreateParallelTopResults<T>(
            IList<T> choices,
            ScoredCandidate<int>[] selected)
        {
            foreach (var candidate in selected)
            {
                yield return new ExtractedResult<T>(choices[candidate.Index], candidate.Score, candidate.Index);
            }
        }

        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = GetIndexedChoices(choices);
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
            var materializedChoices = GetIndexedChoices(choices);
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
            var materializedChoices = GetIndexedChoices(choices);
            var processedQuery = processor(extractor(query));
            return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), cutoff, parallelOptions);
        }

        public static ExtractedResult<string> ExtractOne(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = GetIndexedChoices(choices);
            var processedQuery = processor(query);
            return ExtractOneParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(choice)), cutoff, parallelOptions);
        }

        public static ExtractedResult<T> ExtractOne<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            var materializedChoices = GetIndexedChoices(choices);
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
            ValidateLimit(limit);
            if (limit == 0)
                return Enumerable.Empty<ExtractedResult<T>>();

            var materializedChoices = GetIndexedChoices(choices);
            var processedQuery = processor(extractor(query));
            return ExtractTopParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), limit, cutoff, parallelOptions);
        }

        public static IEnumerable<ExtractedResult<string>> ExtractTop(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            ValidateLimit(limit);
            if (limit == 0)
                return Enumerable.Empty<ExtractedResult<string>>();

            var materializedChoices = GetIndexedChoices(choices);
            var processedQuery = processor(query);
            return ExtractTopParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(choice)), limit, cutoff, parallelOptions);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer calculator, int limit, double cutoff = 0, ParallelOptions parallelOptions = null)
        {
            ValidateLimit(limit);
            if (limit == 0)
                return Enumerable.Empty<ExtractedResult<T>>();

            var materializedChoices = GetIndexedChoices(choices);
            var processedQuery = processor(query);
            return ExtractTopParallelCore(materializedChoices, choice => calculator.Score(processedQuery, processor(extractor(choice))), limit, cutoff, parallelOptions);
        }
    }
}
