using System;
using Raffinert.FuzzySharp.Utils;

namespace Raffinert.FuzzySharp.SimilarityRatio.NewAbstraction;

public class IndelFuzzyScorer<T> : IFuzzyScorer<T> where T : IEquatable<T>
{
    private readonly ReadOnlyMemory<T> _input1;
    private readonly CharMaskBuffer<T> _charMask;

    public IndelFuzzyScorer(ReadOnlyMemory<T> input1)
    {
        _input1 = input1;
        var blocks = (input1.Length + 63) >> 6;
        _charMask = new CharMaskBuffer<T>(64, blocks);
        var input1Span = input1.Span;
        for (var i = 0; i < input1.Length; i++)
        {
            _charMask.AddBit(input1Span[i], i);
        }
    }

    public static IFuzzyScorer<T> Create(ReadOnlyMemory<T> input1)
    {
        return new IndelFuzzyScorer<T>(input1);
    }

    public int ScoreFrom(ReadOnlySpan<T> input2)
    {
        return Indel.DistanceImpl(_input1.Span, input2, _charMask);
    }

    public static int Score(ReadOnlySpan<T> s1, ReadOnlySpan<T> s2)
    {
        return Indel.Distance(s1, s2);
    }

    public static int Score(ReadOnlySpan<T> input1, ReadOnlySpan<T> input2, Processor<T> processor)
    {
        if (processor != null)
        {
            processor(ref input1);
            processor(ref input2);
        }
        return Score(input1, input2);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _charMask?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}