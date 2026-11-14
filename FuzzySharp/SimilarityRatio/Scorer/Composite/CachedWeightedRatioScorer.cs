using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public sealed class CachedWeightedRatioScorer : ICachedRatioScorer
{
    private const double UnbaseScale = .95;
    private const double PartialScale = .90;

    private readonly ICachedStrategy _strategy;
    private readonly CachedDefaultRatioScorer _baseRatioScorer;
    private readonly string _input1;
    private readonly CachedTokenSortScorer _tokenSortScorer;
    private readonly CachedTokenSetScorer _tokenSetScorer;
    private bool _disposed;

    public CachedWeightedRatioScorer(string input1)
    {
        _input1 = input1;
        _strategy = new CachedDefaultRatioStrategy(input1);
        _baseRatioScorer = new CachedDefaultRatioScorer(_strategy);
        _tokenSortScorer = new CachedTokenSortScorer(_input1);
        _tokenSetScorer = new CachedTokenSetScorer(_input1);
    }

    public double Score(string input2)
    {
        int len1 = _input1.Length;
        int len2 = input2.Length;

        if (len1 == 0 || len2 == 0)
        {
            return 0;
        }

        double baseRatio = _baseRatioScorer.Score(input2);
        // Every other branch is scaled to at most 95.
        if (baseRatio >= UnbaseScale * 100)
            return baseRatio;

        double lenRatio = (double)Math.Max(len1, len2) / Math.Min(len1, len2);
        double partialScale = lenRatio > 8 ? .6 : PartialScale;

        if (lenRatio >= 1.5)
        {
            double partial = Fuzz.PartialRatio(_input1, input2) * partialScale;
            double partialSor = _tokenSortScorer.Score(input2) * UnbaseScale * partialScale;
            double partialSet = _tokenSetScorer.Score(input2) * UnbaseScale * partialScale;

            return Math.Max(baseRatio, Math.Max(partial, Math.Max(partialSor, partialSet)));
        }

        double tokenSort = _tokenSortScorer.Score(input2) * UnbaseScale;
        double tokenSet = _tokenSetScorer.Score(input2) * UnbaseScale;
        return Math.Max(baseRatio, Math.Max(tokenSort, tokenSet));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _strategy.Dispose();
        _tokenSortScorer.Dispose();
    }
}
