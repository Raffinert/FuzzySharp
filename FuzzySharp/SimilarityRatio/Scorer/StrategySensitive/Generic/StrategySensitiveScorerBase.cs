using System;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Generic;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive.Generic
{
    public abstract class StrategySensitiveScorerBase<T> : ScorerBase<T> where T : IEquatable<T>
    {
        protected abstract Func<T[], T[], int> Scorer { get; }
    }
}
