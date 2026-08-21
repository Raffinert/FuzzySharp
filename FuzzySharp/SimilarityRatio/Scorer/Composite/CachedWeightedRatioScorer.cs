using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public sealed class CachedWeightedRatioScorer : ICachedRatioScorer
{
    private static readonly double UNBASE_SCALE = .95;
    private static readonly double PARTIAL_SCALE = .90;
    private static readonly bool TRY_PARTIALS = true;

    private readonly ICachedStrategy _strategy;
    private readonly CachedDefaultRatioScorer _baseRatioScorer;
    private readonly string _input1;
    private readonly CachedTokenSortScorer _tokenSortScorer;
    private readonly CachedTokenSetScorer _tokenSetScorer;

    public CachedWeightedRatioScorer(string input1)
    {
        _input1 = input1;
        _strategy = new CachedDefaultRatioStrategy(input1);
        _baseRatioScorer = new CachedDefaultRatioScorer(_strategy);
        _tokenSortScorer = new CachedTokenSortScorer(_strategy);
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

        bool tryPartials = TRY_PARTIALS;
        double unbaseScale = UNBASE_SCALE;
        double partialScale = PARTIAL_SCALE;

        double baseRatio = _baseRatioScorer.Score(input2);
        double lenRatio = (double)Math.Max(len1, len2) / Math.Min(len1, len2);

        // if strings are similar length don't use partials
        if (lenRatio < 1.5) tryPartials = false;

        // if one string is much shorter than the other
        if (lenRatio > 8) partialScale = .6;

        if (tryPartials)
        {
            double partial = Fuzz.PartialRatio(_input1, input2) * partialScale;
            double partialSor = _tokenSortScorer.Score(input2) * unbaseScale * partialScale;
            double partialSet = _tokenSetScorer.Score(input2) * unbaseScale * partialScale;

            return Math.Max(baseRatio, Math.Max(partial, Math.Max(partialSor, partialSet)));
        }

        double tokenSort = _tokenSortScorer.Score(input2) * unbaseScale;
        double tokenSet = _tokenSetScorer.Score(input2) * unbaseScale;
        return Math.Max(baseRatio, Math.Max(tokenSort, tokenSet));
    }

    public void Dispose()
    {
        _strategy.Dispose();
    }
}
