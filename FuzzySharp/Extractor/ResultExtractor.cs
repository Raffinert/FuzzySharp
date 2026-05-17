using Raffinert.FuzzySharp.Extensions;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Extractor;

public static partial class ResultExtractor
{
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
        return ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff).Max();
    }

    public static ExtractedResult<string> ExtractOne(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, processor, scorer, cutoff).Max();
    }

    public static ExtractedResult<T> ExtractOne<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff).Max();
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
        return ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff).MaxN(limit).Reverse();
    }

    public static IEnumerable<ExtractedResult<string>> ExtractTop(string query, IEnumerable<string> choices, Func<string, string> processor, IRatioScorer scorer, int limit, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, processor, scorer, cutoff).MaxN(limit).Reverse();
    }

    public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(string query, IEnumerable<T> choices, Func<T, string> extractor, Func<string, string> processor, IRatioScorer scorer, int limit, int cutoff = 0)
    {
        return ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff).MaxN(limit).Reverse();
    }
}