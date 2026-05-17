using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace Raffinert.FuzzySharp;

public static class Process
{
    internal static readonly WeightedRatioScorer DefaultScorer = (WeightedRatioScorer)ScorerCache.Get<WeightedRatioScorer>();
    internal static readonly Func<string, string> DefaultStringProcessor = StringPreprocessor.Full;

    /// <summary>
    /// Creates a new fluent builder for configuring a fuzzy string matching pipeline.
    /// Supports caching, parallel execution, and custom configuration.
    /// </summary>
    /// <returns>A new ProcessBuilder instance</returns>
    public static ProcessBuilder Configure() => new ProcessBuilder();

    #region ExtractAll
    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<string>> ExtractAll(
        string query, 
        IEnumerable<string> choices, 
        Func<string, string> processor = null, 
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractWithoutOrder(query, choices, processor, processor, scorer, cutoff);
    }


    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractAllBy<T>(
        T query, 
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractAllBy<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;  
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractWithoutOrder(query, choices, extractor, processor, scorer, cutoff);
    }

    #endregion

    #region ExtractTop
    /// <summary>
    /// Creates a sorted list of ExtractedResult  which contain the
    /// top limit most similar choices
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="limit"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<string>> ExtractTop(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int limit = 5,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult  which contain the
    /// top limit most similar choices
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="limit"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractTopBy<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int limit = 5,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractTop(query, choices, extractor, processor, scorer, limit, cutoff);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult  which contain the
    /// top limit most similar choices
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="limit"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractTopBy<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int limit = 5,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractTop(query, choices, extractor, processor, scorer, limit, cutoff);
    }

    #endregion

    #region ExtractSorted
    
    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<string>> ExtractSorted(
        string query,
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractSortedBy<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractSorted(query, choices, extractor, processor, scorer, cutoff);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static IEnumerable<ExtractedResult<T>> ExtractSortedBy<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractSorted(query, choices, extractor, processor, scorer, cutoff);
    }

    #endregion

    #region ExtractOne
    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static ExtractedResult<string> ExtractOne(
        string query, 
        IEnumerable<string> choices,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static ExtractedResult<T> ExtractOneBy<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractOne(query, choices, extractor, processor, scorer, cutoff);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <param name="extractor"></param>
    /// <param name="processor"></param>
    /// <param name="scorer"></param>
    /// <param name="cutoff"></param>
    /// <returns></returns>
    public static ExtractedResult<T> ExtractOneBy<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Func<string, string> processor = null,
        IRatioScorer scorer = null,
        int cutoff = 0)
    {
        processor ??= DefaultStringProcessor;
        scorer ??= DefaultScorer;
        return ResultExtractor.ExtractOne(query, choices, extractor, processor, scorer, cutoff);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="choices"></param>
    /// <returns></returns>
    public static ExtractedResult<string> ExtractOne(string query, params string[] choices)
    {
        return ResultExtractor.ExtractOne(query, choices, DefaultStringProcessor, DefaultScorer);
    }

    #endregion
}