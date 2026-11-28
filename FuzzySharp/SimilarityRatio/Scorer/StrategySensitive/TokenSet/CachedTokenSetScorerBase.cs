using System;
using System.Collections.Generic;
using Raffinert.FuzzySharp.Extensions;

namespace Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

public abstract class CachedTokenSetScorerBase(string input1) : ICachedRatioScorer
{
    private readonly HashSet<string> _tokens1 = new(input1.SplitByAnySpace());
    protected abstract FuzzySharp.Scorer Scorer { get; }

    public double Score(string input2)
    {
        var tokens2 = new HashSet<string>(input2.SplitByAnySpace());

        if (_tokens1.Count == 0 || tokens2.Count == 0)
        {
            return 0;
        }

        var intersection = new List<string>(Math.Min(_tokens1.Count, tokens2.Count));
        var difference1 = new List<string>();
        foreach (var token in _tokens1)
        {
            if (tokens2.Remove(token))
                intersection.Add(token);
            else
                difference1.Add(token);
        }

        return TokenSetScorerHelpers.Score(Scorer, UsesBuiltInIndelRatio, intersection, difference1, tokens2);
    }

    /// <summary>
    /// True only when the scorer strategy is the built-in Indel ratio,
    /// which lets token-set scoring use exact length-based shortcuts.
    /// Custom and partial scorers keep calling their own strategy.
    /// </summary>
    internal virtual bool UsesBuiltInIndelRatio => false;

    public void Dispose()
    {
    }
}
