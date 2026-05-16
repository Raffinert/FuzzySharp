using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public class WeightedRatioScorer : ScorerBase
{
    private const double UnbaseScale = .95;
    private const double PartialScale = .90;

    public override int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        var len1 = input1.Length;
        var len2 = input2.Length;

        if (len1 == 0 || len2 == 0)
        {
            return 0;
        }

        var baseRatio = Fuzz.Ratio(input1, input2);
        var lenRatio = (double)Math.Max(len1, len2) / Math.Min(len1, len2);

        // if strings are similar length don't use partials
        if (lenRatio >= 1.5)
        {
            var partialScale = lenRatio > 8 // if one string is much shorter than the other
                ? .6 
                : PartialScale;

            var partial = Fuzz.PartialRatio(input1, input2) * partialScale;
            var sortRatio = Fuzz.TokenSortRatio(input1, input2) * UnbaseScale * partialScale;
            var setRatio = Fuzz.TokenSetRatio(input1, input2) * UnbaseScale * partialScale;

            return (int)Math.Round(Math.Max(baseRatio, Math.Max(partial, Math.Max(sortRatio, setRatio))));
        }

        var tokenSort = Fuzz.TokenSortRatio(input1, input2) * UnbaseScale;
        var tokenSet = Fuzz.TokenSetRatio(input1, input2) * UnbaseScale;
        return (int)Math.Round(Math.Max(baseRatio, Math.Max(tokenSort, tokenSet)));
    }
}