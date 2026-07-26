using System;
using Raffinert.FuzzySharp.PreProcess;
using Raffinert.FuzzySharp.Utils;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

internal sealed class CachedApproximateSubstringRatioStrategy : ICachedStrategy
{
    private readonly Func<string, string> _preprocessor;
    private readonly string _processedInput1;
    private readonly IPatternMatchVector<char> _input1PatternVector;
    private bool _disposed;

    public CachedApproximateSubstringRatioStrategy(
        string input1,
        Func<string, string> preprocessor = null)
    {
        _preprocessor = preprocessor ?? StringPreprocessor.None;
        _processedInput1 = _preprocessor(input1);
        _input1PatternVector = PatternMatchVector.Create(_processedInput1.AsSpan());
    }

    public int Calculate(string input2)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(CachedApproximateSubstringRatioStrategy));
        }

        string processedInput2 = _preprocessor(input2);

        if (_processedInput1.Length == 0 || processedInput2.Length == 0)
        {
            return _processedInput1.Length == 0 && processedInput2.Length == 0
                ? 100
                : 0;
        }

        ReadOnlySpan<char> pattern = _processedInput1.Length <= processedInput2.Length
            ? _processedInput1.AsSpan()
            : processedInput2.AsSpan();
        ReadOnlySpan<char> text = _processedInput1.Length <= processedInput2.Length
            ? processedInput2.AsSpan()
            : _processedInput1.AsSpan();
        if (text.IndexOf(pattern) >= 0)
        {
            return 100;
        }

        if (_processedInput1.Length < processedInput2.Length)
        {
            return Score(_input1PatternVector, processedInput2.AsSpan());
        }

        if (processedInput2.Length < _processedInput1.Length)
        {
            using var input2PatternVector =
                PatternMatchVector.Create(processedInput2.AsSpan());
            return Score(input2PatternVector, _processedInput1.AsSpan());
        }

        int forward = Score(_input1PatternVector, processedInput2.AsSpan());
        using var reversePatternVector =
            PatternMatchVector.Create(processedInput2.AsSpan());
        int reverse = Score(reversePatternVector, _processedInput1.AsSpan());
        return Math.Max(forward, reverse);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _input1PatternVector.Dispose();
        _disposed = true;
    }

    private static int Score(
        IPatternMatchVector<char> patternVector,
        ReadOnlySpan<char> text)
    {
        IndelSubstringMatch match =
            Indel.BestSubstringMatchImpl(patternVector, text);
        return ApproximateSubstringScore.FromDistance(
            match.Distance,
            patternVector.Length);
    }
}
