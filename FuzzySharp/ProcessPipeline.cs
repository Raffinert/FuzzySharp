using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;
using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Immutable fuzzy string matching pipeline.
/// The scoring algorithm is configured at build time via <see cref="ProcessBuilder.WithScorer"/>.
/// When <see cref="ProcessOptions.UseCaching"/> is true, extraction methods automatically create
/// a cached scorer per call for improved performance.
/// </summary>
public readonly struct ProcessPipeline
{
    private readonly ProcessOptions _options;

    internal ProcessPipeline(ProcessOptions options)
    {
        _options = options;
    }

    #region ExtractAll

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractAll(
        string query,
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractAll(query.AsSpan(), choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractAll(query, choices, extractor, processor, cutoff, _options);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractAll(query, choices, extractor, processor, cutoff, _options);
    }

    #endregion

    #region ExtractTop

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractTop(
        string query,
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractTop(query, choices, processor, limit, cutoff, _options);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractTop(query, choices, extractor, processor, limit, cutoff, _options);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractTop(query, choices, extractor, processor, limit, cutoff, _options);
    }

    #endregion

    #region ExtractSorted

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractSorted(
        string query,
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractSorted(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractSorted(query, choices, extractor, processor, cutoff, _options);
    }

    #endregion

    #region ExtractOne

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(
        string query,
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractOne(query, choices, processor, cutoff, _options);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<T> ExtractOne<T>(
        T query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractOne(query, choices, extractor, processor, cutoff, _options);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<T> ExtractOne<T>(
        string query,
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return ProcessExecutor.ExtractOne(query, choices, extractor, processor, cutoff, _options);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(string query, params string[] choices)
    {
        return ProcessExecutor.ExtractOne(query, choices, null, 0, _options);
    }

    #endregion
}

/// <summary>
/// Immutable cached fuzzy string matching pipeline with an external <see cref="ICachedRatioScorer"/>.
/// The scorer is provided at build time and reused across all extraction calls.
/// The caller owns the scorer's lifecycle (creation and disposal).
/// </summary>
public readonly struct CachedScorerProcessPipeline
{
    private readonly CachedScorerProcessOptions _options;

    internal CachedScorerProcessPipeline(CachedScorerProcessOptions options)
    {
        _options = options;
    }

    #region ExtractAll

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractAll(
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractAll(
            choices, x => x, processor ?? StringPreprocessors.Full, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Creates a list of ExtractedResult which contain all the choices with
    /// their corresponding score where higher is more similar
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractAll<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractAll(
            choices, extractor, processor ?? StringPreprocessors.Full, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion

    #region ExtractTop

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractTop(
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractTop(
            choices, x => x, processor ?? StringPreprocessors.Full, _options.CachedScorer, limit, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult which contain the
    /// top limit most similar choices
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractTop<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int limit = 5,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractTop(
            choices, extractor, processor ?? StringPreprocessors.Full, _options.CachedScorer, limit, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion

    #region ExtractSorted

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<string>> ExtractSorted(
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractSorted(
            choices, null, processor ?? StringPreprocessors.Full, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Creates a sorted list of ExtractedResult with the closest matches first
    /// </summary>
    public IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        return CachedScorerProcessExecutor.ExtractSorted(
            choices, extractor, processor ?? StringPreprocessors.Full, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion

    #region ExtractOne

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(
        IEnumerable<string> choices,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        return CachedScorerProcessExecutor.ExtractOne(
            choices, null, processor ?? StringPreprocessors.Full, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<T> ExtractOne<T>(
        IEnumerable<T> choices,
        Func<T, string> extractor,
        Processor<char> processor = null,
        int cutoff = 0)
    {
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        return CachedScorerProcessExecutor.ExtractOne(
            choices, extractor, processor ?? StringPreprocessors.Full, _options.CachedScorer, cutoff,
            _options.UseParallel, _options.ParallelOptions);
    }

    /// <summary>
    /// Find the single best match above a score in a list of choices.
    /// </summary>
    public ExtractedResult<string> ExtractOne(params string[] choices)
    {
        return CachedScorerProcessExecutor.ExtractOne(
            choices, null, StringPreprocessors.Full, _options.CachedScorer, 0,
            _options.UseParallel, _options.ParallelOptions);
    }

    #endregion
}
