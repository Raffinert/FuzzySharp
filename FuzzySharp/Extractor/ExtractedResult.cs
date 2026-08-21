using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Extractor;

public class ExtractedResult<T>(T value, double score, int index) : IComparable<ExtractedResult<T>>
{
    public readonly T Value = value;
    public readonly double Score = score;
    public readonly int Index = index;

    public ExtractedResult(T value, double score) : this(value, score, 0)
    { }

    public int CompareTo(ExtractedResult<T> other)
    {
        return Comparer<double>.Default.Compare(this.Score, other.Score);
    }

    public override string ToString()
    {
        if (typeof(T) == typeof(string))
        {
            return $"(string: {Value}, score: {Score}, index: {Index})";
        }
        return $"(value: {Value}, score: {Score}, index: {Index})";
    }
}
