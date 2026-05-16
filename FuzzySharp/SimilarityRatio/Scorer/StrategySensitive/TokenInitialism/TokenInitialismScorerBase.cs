using Raffinert.FuzzySharp.Extensions;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

using System;

public abstract class TokenInitialismScorerBase : StrategySensitiveScorerBase
{
    public override int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        ReadOnlySpan<char> shorter;
        ReadOnlySpan<char> longer;

        if (input1.Length < input2.Length)
        {
            shorter = input1;
            longer = input2;
        }
        else
        {
            shorter = input2;
            longer = input1;
        }

        double lenRatio = (double)longer.Length / shorter.Length;

        // if longer isn't at least 3 times longer than the other, then it's probably not an initialism
        if (lenRatio < 3) return 0;

        var initials = longer.GetInitials();

        return Scorer(initials.Span, shorter);
    }
}