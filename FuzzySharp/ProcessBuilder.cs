using System;
using System.Threading.Tasks;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Fluent builder for configuring fuzzy string matching pipelines.
/// Call <see cref="Cached()"/> to enable auto-caching, or <see cref="Cached(ICachedRatioScorer)"/>
/// to provide an external cached scorer.
/// </summary>
public sealed class ProcessBuilder
{
    private bool _useParallel;
    private ParallelOptions _parallelOptions;
    private IRatioScorer _scorer;
    /// <summary>
    /// Enables caching mode with automatic scorer creation per extraction call.
    /// Returns a <see cref="CachedProcessBuilder"/> for further configuration.
    /// Order independent - can be called before or after Parallel().
    /// Note: If <see cref="WithScorer"/> was called prior to Cached(), the custom scorer is silently
    /// ignored — a <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.CachedWeightedRatioScorer"/>
    /// is used instead. To retain a custom scorer in cached mode, use <see cref="Cached(ICachedRatioScorer)"/>.
    /// </summary>
    public CachedProcessBuilder Cached()
    {
        return new CachedProcessBuilder(_useParallel, _parallelOptions);
    }

    /// <summary>
    /// Enables caching mode with an external cached scorer instance for across-run caching.
    /// Returns a <see cref="CachedScorerProcessBuilder"/> for further configuration.
    /// When using with parallel execution, the caller is responsible for ensuring the scorer is thread-safe.
    /// Order independent - can be called before or after Parallel().
    /// Note: If <see cref="WithScorer"/> was called prior to this method, the custom scorer is silently
    /// ignored in favor of the provided <paramref name="scorer"/>.
    /// </summary>
    /// <param name="scorer">The external cached scorer instance. Must not be null.</param>
    public CachedScorerProcessBuilder Cached(ICachedRatioScorer scorer)
    {
        if (scorer == null) throw new ArgumentNullException(nameof(scorer));
        return new CachedScorerProcessBuilder(scorer, _useParallel, _parallelOptions);
    }

    /// <summary>
    /// Enables parallel execution for improved performance on multi-core systems.
    /// Order independent - can be called before or after Cached().
    /// </summary>
    /// <param name="parallelOptions">Optional parallel execution options (max degree of parallelism, cancellation token, etc.)</param>
    public ProcessBuilder Parallel(ParallelOptions parallelOptions = null)
    {
        _useParallel = true;
        if (parallelOptions != null)
        {
            _parallelOptions = parallelOptions;
        }
        return this;
    }

    /// <summary>
    /// Configures parallel execution options (max degree of parallelism, cancellation token, etc.).
    /// Implicitly enables parallel mode.
    /// </summary>
    /// <param name="parallelOptions">Parallel execution options</param>
    public ProcessBuilder WithParallelOptions(ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions ?? throw new ArgumentNullException(nameof(parallelOptions));
        _useParallel = true;
        return this;
    }

    /// <summary>
    /// Sets the scoring algorithm for fuzzy matching.
    /// If not set, defaults to <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.WeightedRatioScorer"/>.
    /// </summary>
    /// <param name="scorer">The ratio scorer instance to use</param>
    public ProcessBuilder WithScorer(IRatioScorer scorer)
    {
        _scorer = scorer ?? throw new ArgumentNullException(nameof(scorer));
        return this;
    }

    /// <summary>
    /// Builds an immutable ProcessPipeline with the configured options.
    /// </summary>
    public ProcessPipeline Build()
    {
        return new ProcessPipeline(new ProcessOptions(_useParallel, _parallelOptions, _scorer));
    }
}

/// <summary>
/// Fluent builder for configuring cached fuzzy string matching pipelines with automatic scorer creation.
/// Produces a <see cref="ProcessPipeline"/> with caching enabled — a
/// <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.CachedWeightedRatioScorer"/>
/// is created automatically per extraction call.
/// </summary>
public sealed class CachedProcessBuilder
{
    private bool _useParallel;
    private ParallelOptions _parallelOptions;

    internal CachedProcessBuilder(bool useParallel, ParallelOptions parallelOptions)
    {
        _useParallel = useParallel;
        _parallelOptions = parallelOptions;
    }

    /// <summary>
    /// Enables parallel execution for improved performance on multi-core systems.
    /// </summary>
    /// <param name="parallelOptions">Optional parallel execution options (max degree of parallelism, cancellation token, etc.)</param>
    public CachedProcessBuilder Parallel(ParallelOptions parallelOptions = null)
    {
        _useParallel = true;
        if (parallelOptions != null)
        {
            _parallelOptions = parallelOptions;
        }
        return this;
    }

    /// <summary>
    /// Configures parallel execution options (max degree of parallelism, cancellation token, etc.).
    /// Implicitly enables parallel mode.
    /// </summary>
    /// <param name="parallelOptions">Parallel execution options</param>
    public CachedProcessBuilder WithParallelOptions(ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions ?? throw new ArgumentNullException(nameof(parallelOptions));
        _useParallel = true;
        return this;
    }

    /// <summary>
    /// Builds an immutable ProcessPipeline with caching enabled.
    /// </summary>
    public ProcessPipeline Build()
    {
        return new ProcessPipeline(new ProcessOptions(_useParallel, _parallelOptions, scorer: null, useCaching: true));
    }
}

/// <summary>
/// Fluent builder for configuring cached fuzzy string matching pipelines with an external scorer.
/// Produces a <see cref="CachedScorerProcessPipeline"/> that reuses the provided
/// <see cref="ICachedRatioScorer"/> across all extraction calls.
/// </summary>
public sealed class CachedScorerProcessBuilder
{
    private bool _useParallel;
    private ParallelOptions _parallelOptions;
    private readonly ICachedRatioScorer _cachedScorer;

    internal CachedScorerProcessBuilder(ICachedRatioScorer cachedScorer, bool useParallel, ParallelOptions parallelOptions)
    {
        _cachedScorer = cachedScorer;
        _useParallel = useParallel;
        _parallelOptions = parallelOptions;
    }

    /// <summary>
    /// Enables parallel execution for improved performance on multi-core systems.
    /// </summary>
    /// <param name="parallelOptions">Optional parallel execution options (max degree of parallelism, cancellation token, etc.)</param>
    public CachedScorerProcessBuilder Parallel(ParallelOptions parallelOptions = null)
    {
        _useParallel = true;
        if (parallelOptions != null)
        {
            _parallelOptions = parallelOptions;
        }
        return this;
    }

    /// <summary>
    /// Configures parallel execution options (max degree of parallelism, cancellation token, etc.).
    /// Implicitly enables parallel mode.
    /// </summary>
    /// <param name="parallelOptions">Parallel execution options</param>
    public CachedScorerProcessBuilder WithParallelOptions(ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions ?? throw new ArgumentNullException(nameof(parallelOptions));
        _useParallel = true;
        return this;
    }

    /// <summary>
    /// Builds an immutable CachedScorerProcessPipeline with the configured options.
    /// </summary>
    public CachedScorerProcessPipeline Build()
    {
        return new CachedScorerProcessPipeline(new CachedScorerProcessOptions(
            _useParallel,
            _parallelOptions,
            _cachedScorer
        ));
    }
}
