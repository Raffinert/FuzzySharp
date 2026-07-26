using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class ApproximateSubstringScore
{
    public static int FromDistance(int distance, int patternLength)
    {
        if (patternLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(patternLength));
        }

        double similarity = 1.0 - distance / (double)patternLength;
        int score = (int)Math.Round(100.0 * similarity);

        return Math.Max(0, Math.Min(100, score));
    }
}
