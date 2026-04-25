using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

public interface ICachedStrategy : IDisposable
{
    int Calculate(string input2);
}