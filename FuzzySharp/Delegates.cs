using System;

namespace Raffinert.FuzzySharp;

public delegate double Scorer(string input1, string input2);
public delegate double CachedScorer(string input2);
public delegate void Processor<T>(ref ReadOnlySpan<T> str) where T : IEquatable<T>;
