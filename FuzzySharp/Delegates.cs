using System;

namespace Raffinert.FuzzySharp;

public delegate int Scorer(ReadOnlySpan<char> input1, ReadOnlySpan<char> input2);
public delegate int CachedScorer(ReadOnlySpan<char> input2);
public delegate void Processor<T>(ref ReadOnlySpan<T> str) where T : IEquatable<T>;