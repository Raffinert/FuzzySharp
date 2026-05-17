using System;

namespace Raffinert.FuzzySharp.PreProcess;

public static class StringPreprocessor
{
    public static Func<string, string> Full { get; } = Default;
    public static Func<string, string> None { get; } = static s => s;

    private static string Default(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var result = new char[input.Length].AsSpan();

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            result[i] = char.IsLetterOrDigit(c) ? char.ToLower(c) : ' ';
        }

        return result.Trim().ToString();
    }
}