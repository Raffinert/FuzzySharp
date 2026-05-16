using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
    public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer scorer, int cutoff = 0)
    {
        int index = 0;
        processor(ref query);
        
        var results = new List<ExtractedResult<T>>();
        foreach (var choice in choices)
        {
            var choiceStr = (extractor?.Invoke(choice) ?? choice as string).AsSpan();
            processor?.Invoke(ref choiceStr);
            int score = scorer.Score(query, choiceStr);
            if (score >= cutoff)
            {
                results.Add(new ExtractedResult<T>(choice, score, index));
            }
            index++;
        }
        return results;
    }

    public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer scorer, int cutoff = 0)
    {
        var extractedQuery = (extractor?.Invoke(query) ?? query as string).AsSpan();
        processor?.Invoke(ref extractedQuery);
        return ExtractWithoutOrder(extractedQuery, choices, extractor, processor, scorer, cutoff);
    }

    public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).Max();
    }

    public static ExtractedResult<T> ExtractOne<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).Max();
    }

    public static ExtractedResult<T> ExtractOne<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).Max();
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).OrderByDescending(r => r.Score);
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int limit, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).MaxN(limit).Reverse();
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(ReadOnlySpan<char> query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int limit, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).MaxN(limit).Reverse();
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Processor<char> processor, IRatioScorer calculator, int limit, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, calculator, cutoff).MaxN(limit).Reverse();
    }
}