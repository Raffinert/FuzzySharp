using System;
using System.Threading;
using Raffinert.FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal static class PartialRatioStrategy
{
    private static PartialRatioAccuracy _accuracy = PartialRatioAccuracy.Strict;

    internal delegate int PartialRatio(ReadOnlySpan<char> shorter, ReadOnlySpan<char> longer);

    private static PartialRatio _partialRatioImpl = PartialRatioStrategy<char>.Calculate;

    public static PartialRatioAccuracy Accuracy
    {
        get => _accuracy;
        set
        {
            if (_accuracy != value)
            {
                PartialRatio partialRatioImpl = value switch
                {
                    PartialRatioAccuracy.Strict => PartialRatioStrategy<char>.Calculate,
                    PartialRatioAccuracy.Fast   => FastPartialRatioStrategyT<char>.Calculate,
                    _               => throw new ArgumentOutOfRangeException(nameof(value), "Unsupported accuracy mode.")
                };

                Interlocked.Exchange(ref _partialRatioImpl, partialRatioImpl);
                _accuracy = value;
            }
        }
    }

    /// <summary>
    /// Searches for the optimal alignment of the shorter span in the longer span
    /// and returns the partial fuzz.ratio for that alignment, as a value in [0…100].
    /// </summary>
    public static int Calculate(string input1, string input2)
    {
        if (input1.Length == 0 || input2.Length == 0)
        {
            return 0;
        }

        var score = _partialRatioImpl(input1.AsSpan(), input2.AsSpan());

        return score;
    }
}