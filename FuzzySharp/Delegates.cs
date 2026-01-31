using System;

namespace Raffinert.FuzzySharp;

public delegate int Scorer(string input1, string input2);
public delegate int CachedScorer(string input2);
public delegate void Processor<T>(ref ReadOnlySpan<T> str) where T : IEquatable<T>;