using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public sealed class CachedWeightedRatioScorer : ICachedRatioScorer
{
    private static readonly double UnbaseScale = .95;
    private static readonly double PartialScale = .90;

    private readonly ICachedStrategy _strategy;
    private readonly CachedDefaultRatioScorer _baseRatioScorer;
    private readonly CachedTokenSortScorer _tokenSortScorer;
    private readonly CachedTokenSetScorer _tokenSetScorer;
    private readonly string _input1;

    public CachedWeightedRatioScorer(string input1)
    {
        _input1 = input1;
        var span = input1.AsSpan();
        _strategy = new CachedDefaultRatioStrategy(span);
        _baseRatioScorer = new CachedDefaultRatioScorer(_strategy);
        _tokenSortScorer = new CachedTokenSortScorer(_strategy);
        _tokenSetScorer = new CachedTokenSetScorer(span);
    }

    public int Score(ReadOnlySpan<char> input2)
    {
        var len2 = input2.Length;

        if (_input1.Length == 0 || len2 == 0)
        {
            return 0;
        }

        var baseRatio = _baseRatioScorer.Score(input2);
        var lenRatio = (double)Math.Max(_input1.Length, len2) / Math.Min(_input1.Length, len2);

        // if strings are similar length don't use partials
        if (lenRatio >= 1.5)
        {
            var partialScale = lenRatio > 8 ? .6 : PartialScale;

            var partial = Fuzz.PartialRatio(_input1.AsSpan(), input2) * partialScale;
            var partialSor = _tokenSortScorer.Score(input2) * UnbaseScale * partialScale;
            var partialSet = _tokenSetScorer.Score(input2) * UnbaseScale * partialScale;

            return (int)Math.Round(Math.Max(baseRatio, Math.Max(partial, Math.Max(partialSor, partialSet))));
        }

        var tokenSort = _tokenSortScorer.Score(input2) * UnbaseScale;
        var tokenSet = _tokenSetScorer.Score(input2) * UnbaseScale;
        return (int)Math.Round(Math.Max(baseRatio, Math.Max(tokenSort, tokenSet)));
    }

    public void Dispose()
    {
        _strategy.Dispose();
    }
}