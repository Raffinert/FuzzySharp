using System;

namespace Raffinert.FuzzySharp.PreProcess;

public static class StringPreprocessors
{
    public static Processor<char> Full { get; } = RemoveNonAlphaNumerics;

    public static Processor<char> None { get; } = Ignore;

    private static void RemoveNonAlphaNumerics(ref ReadOnlySpan<char> str)
    {
        if (str.IsWhiteSpace() || str.IsEmpty)
        {
            str = ReadOnlySpan<char>.Empty;
            return;
        }

        var result = new char[str.Length].AsSpan();

        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];
            result[i] = char.IsLetterOrDigit(c) ? char.ToLower(c) : ' ';
        }

        str = result.Trim();
    }

    private static void Ignore(ref ReadOnlySpan<char> str) { }
}