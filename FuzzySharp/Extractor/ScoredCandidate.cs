using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Extractor;

internal readonly struct ScoredCandidate<T>(T value, int score, int index)
{
    public T Value { get; } = value;

    public int Score { get; } = score;

    public int Index { get; } = index;
}

internal sealed class ScoredCandidateComparer<T> : Comparer<ScoredCandidate<T>>
{
    public static ScoredCandidateComparer<T> Instance { get; } = new();

    public override int Compare(ScoredCandidate<T> x, ScoredCandidate<T> y)
    {
        return x.Score.CompareTo(y.Score);
    }
}

internal struct BestCandidate<T>
{
    public ScoredCandidate<T> Candidate { get; private set; }

    public bool HasValue { get; private set; }

    public void Consider(T value, int score, int index, int cutoff)
    {
        if (score < cutoff)
        {
            return;
        }

        if (!HasValue || score > Candidate.Score || (score == Candidate.Score && index < Candidate.Index))
        {
            Candidate = new ScoredCandidate<T>(value, score, index);
            HasValue = true;
        }
    }

    public void Consider(BestCandidate<T> other)
    {
        if (other.HasValue)
        {
            Consider(other.Candidate.Value, other.Candidate.Score, other.Candidate.Index, int.MinValue);
        }
    }
}
