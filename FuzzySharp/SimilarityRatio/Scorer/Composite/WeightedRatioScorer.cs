using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;

public class WeightedRatioScorer : ScorerBase
{
    private const double UnbaseScale = .95;
    private const double PartialScale = .90;

    public override double Score(string input1, string input2)
    {
        int len1 = input1.Length;
        int len2 = input2.Length;

        if (len1 == 0 || len2 == 0)
        {
            return 0;
        }

        double baseRatio = Fuzz.Ratio(input1, input2);
        // Every other branch is scaled to at most 95.
        if (baseRatio >= UnbaseScale * 100)
            return baseRatio;

        double lenRatio = (double)Math.Max(len1, len2) / Math.Min(len1, len2);
        double partialScale = lenRatio > 8 ? .6 : PartialScale;

        if (lenRatio >= 1.5)
        {
            double partial = Fuzz.PartialRatio(input1, input2) * partialScale;
            // Keep the legacy full TokenSort/TokenSet partial branch for compatibility.
            double partialSor = Fuzz.TokenSortRatio(input1, input2) * UnbaseScale * partialScale;
            double partialSet = Fuzz.TokenSetRatio(input1, input2) * UnbaseScale * partialScale;

            return Math.Max(baseRatio, Math.Max(partial, Math.Max(partialSor, partialSet)));
        }

        double tokenSort = Fuzz.TokenSortRatio(input1, input2) * UnbaseScale;
        double tokenSet = Fuzz.TokenSetRatio(input1, input2) * UnbaseScale;
        return Math.Max(baseRatio, Math.Max(tokenSort, tokenSet));
    }
}
