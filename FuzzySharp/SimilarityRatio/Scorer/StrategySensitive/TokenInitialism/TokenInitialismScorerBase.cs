using Raffinert.FuzzySharp.Extensions;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

using Utils;
using System;

public abstract class TokenInitialismScorerBase : StrategySensitiveScorerBase
{
    public override int Score(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2)
    {
        SequenceUtils.SwapIfSourceIsLonger(ref input1, ref input2);

        if (input1.Length == 0) return 0;

        double lenRatio = (double)input2.Length / input1.Length;

        // if longer isn't at least 3 times longer than the other, then it's probably not an initialism
        if (lenRatio < 3) return 0;

        var initials = input2.GetInitials();

        return Scorer(initials, input1);
    }
}