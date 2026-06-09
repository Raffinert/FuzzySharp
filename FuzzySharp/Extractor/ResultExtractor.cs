using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    private static ExtractedResult<T> ExtractOneCore<T>(
        IEnumerable<T> choices,
        Func<T, int> scoreSelector,
        int cutoff)
    {
        var index = 0;
        var bestIndex = 0;
        var bestScore = 0;
        T bestValue = default;
        var hasBest = false;

        foreach (var choice in choices)
        {
            var score = scoreSelector(choice);
            if (score >= cutoff && (!hasBest || score > bestScore))
            {
                bestValue = choice;
                bestScore = score;
                bestIndex = index;
                hasBest = true;
            }

            index++;
        }

        if (!hasBest)
        {
            return Enumerable.Empty<ExtractedResult<T>>().Max();
        }

        return new ExtractedResult<T>(bestValue, bestScore, bestIndex);
    }

    private static IEnumerable<ExtractedResult<T>> ExtractTopCore<T>(
        IEnumerable<T> choices,
        Func<T, int> scoreSelector,
        int limit,
        int cutoff)
    {
        var heap = new MinHeap<ScoredCandidate<T>>(ScoredCandidateComparer<T>.Instance);
        var comparer = ScoredCandidateComparer<T>.Instance;
        var index = 0;

        foreach (var choice in choices)
        {
            var score = scoreSelector(choice);
            if (score >= cutoff)
            {
                AddTopCandidate(heap, comparer, new ScoredCandidate<T>(choice, score, index), limit);
            }

            index++;
        }

        foreach (var result in CreateTopResults(heap))
        {
            yield return result;
        }
    }

    private static void AddTopCandidate<T>(
        MinHeap<ScoredCandidate<T>> heap,
        Comparer<ScoredCandidate<T>> comparer,
        ScoredCandidate<T> candidate,
        int limit)
    {
        if (heap.Count < limit)
        {
            heap.Add(candidate);
        }
        else if (comparer.Compare(candidate, heap.GetMin()) > 0)
        {
            heap.ExtractDominating();
            heap.Add(candidate);
        }
    }

    private static IEnumerable<ExtractedResult<T>> CreateTopResults<T>(MinHeap<ScoredCandidate<T>> heap)
    {
        var results = new ExtractedResult<T>[heap.Count];

        for (var i = results.Length - 1; i >= 0; i--)
        {
            var candidate = heap.ExtractDominating();
            results[i] = new ExtractedResult<T>(candidate.Value, candidate.Score, candidate.Index);
        }

        return results;
    }

    public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        int index = 0;
        processor ??= Process.DefaultStringProcessor;
        var processedQuery = processor(query);

        foreach (var choice in choices)
        {
            int score = scorer.Score(processedQuery, processor(extractor(choice)));
            if (score >= cutoff)
            {
                yield return new ExtractedResult<T>(choice, score, index);
            }
            index++;
        }
    }

    public static IEnumerable<ExtractedResult<string>> ExtractWithoutOrder(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        int index = 0;
        processor ??= Process.DefaultStringProcessor;
        var processedQuery = processor(query);

        foreach (var choice in choices)
        {
            int score = scorer.Score(processedQuery, processor(choice));
            if (score >= cutoff)
            {
                yield return new ExtractedResult<string>(choice, score, index);
            }
            index++;
        }
    }

    public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        var extracted = extractor(query);
        return ExtractWithoutOrder(extracted, choices, extractor, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        processor ??= Process.DefaultStringProcessor;
        var processedQuery = processor(extractor(query));
        return ExtractOneCore(choices, choice => scorer.Score(processedQuery, processor(extractor(choice))), cutoff);
    }

    public static ExtractedResult<string> ExtractOne(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        processor ??= Process.DefaultStringProcessor;
        var processedQuery = processor(query);
        return ExtractOneCore(choices, choice => scorer.Score(processedQuery, processor(choice)), cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        processor ??= Process.DefaultStringProcessor;
        var processedQuery = processor(query);
        return ExtractOneCore(choices, choice => scorer.Score(processedQuery, processor(extractor(choice))), cutoff);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<string>> ExtractSorted(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, processor, scorer, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int limit, int cutoff = 0)
    {
        var extracted = extractor(query);
        return ExtractTop(extracted, choices, extractor, processor, scorer, limit, cutoff);
    }

    public static IEnumerable<ExtractedResult<string>> ExtractTop(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, int limit, int cutoff = 0)
    {
        processor ??= Process.DefaultStringProcessor;
        return ExtractTopIterator();

        IEnumerable<ExtractedResult<string>> ExtractTopIterator()
        {
            var processedQuery = processor(query);
            foreach (var result in ExtractTopCore(choices, choice => scorer.Score(processedQuery, processor(choice)), limit, cutoff))
            {
                yield return result;
            }
        }
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int limit, int cutoff = 0)
    {
        processor ??= Process.DefaultStringProcessor;
        return ExtractTopIterator();

        IEnumerable<ExtractedResult<T>> ExtractTopIterator()
        {
            var processedQuery = processor(query);
            foreach (var result in ExtractTopCore(choices, choice => scorer.Score(processedQuery, processor(extractor(choice))), limit, cutoff))
            {
                yield return result;
            }
        }
    }
}
