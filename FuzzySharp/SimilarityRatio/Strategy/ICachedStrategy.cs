using System;

namespace Raffinert.FuzzySharp.SimilarityRatio.Strategy;

public interface ICachedStrategy : IDisposable
{
    double Calculate(string input2);
}
