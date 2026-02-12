using System.Threading.Tasks;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer;

namespace Raffinert.FuzzySharp;

/// <summary>
/// Internal options for process execution.
/// When <see cref="UseCaching"/> is true, the executor creates a
/// <see cref="Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite.CachedWeightedRatioScorer"/>
/// per extraction call automatically.
/// </summary>
internal readonly struct ProcessOptions
{
    public ProcessOptions(bool useParallel, ParallelOptions parallelOptions, IRatioScorer scorer, bool useCaching = false)
    {
        UseParallel = useParallel;
        ParallelOptions = parallelOptions;
        Scorer = scorer;
        UseCaching = useCaching;
    }

    public bool UseParallel { get; }
    public ParallelOptions ParallelOptions { get; }
    public IRatioScorer Scorer { get; }
    public bool UseCaching { get; }
}

/// <summary>
/// Internal options for cached process execution with an external scorer.
/// </summary>
internal readonly struct CachedScorerProcessOptions
{
    public CachedScorerProcessOptions(bool useParallel, ParallelOptions parallelOptions, ICachedRatioScorer cachedScorer)
    {
        UseParallel = useParallel;
        ParallelOptions = parallelOptions;
        CachedScorer = cachedScorer;
    }

    public bool UseParallel { get; }
    public ParallelOptions ParallelOptions { get; }
    public ICachedRatioScorer CachedScorer { get; }
}
