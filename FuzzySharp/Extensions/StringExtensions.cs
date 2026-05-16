using System;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Extensions;

internal static class StringExtensions
{
    public static List<string> ExtractTokens(this ReadOnlySpan<char> input)
    {
        if (input.IsEmpty || input.IsWhiteSpace())
            return [];

        var result = new List<string>();

        var start = 0;

        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsLetter(input[i]))
                continue;

            if (i > start)
                result.Add(input[start..i].ToString());

            start = i + 1;
        }

        if (input.Length > start)
            result.Add(input[start..].ToString());

        return result;
    }

    public static string GetInitials(this ReadOnlySpan<char> input)
    {
        if (input.IsEmpty || input.IsWhiteSpace())
            return string.Empty;

        var result = new char[input.Length];
        var takeNext = true;
        var resultIndex = 0;

        foreach (var c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                takeNext = true;
                continue;
            }

            if (!takeNext)
                continue;

            result[resultIndex++] = c;

            takeNext = false;
        }

        return resultIndex == 0
            ? string.Empty
            : result.AsSpan(0, resultIndex).ToString();
    }

    public static string[] SplitByAnySpace(this ReadOnlySpan<char> input)
    {
        if (input.IsEmpty || input.IsWhiteSpace())
            return [];

        var result = new List<string>();

        var start = -1;

        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsWhiteSpace(input[i]))
            {
                if (start >= 0)
                {
                    result.Add(input[start..i].ToString());
                    start = -1;
                }

                continue;
            }

            if (start < 0)
                start = i;
        }

        if (start >= 0)
            result.Add(input[start..].ToString());

        return result.Count == 0
            ? []
            : result.ToArray();
    }

    public static string[] GetSortedWords(this ReadOnlySpan<char> input)
    {
        var words = input.SplitByAnySpace();

        Array.Sort(words);

        return words;
    }

    public static ReadOnlyMemory<char> NormalizeSpacesAndSort(this ReadOnlySpan<char> input)
    {
        var words = input.GetSortedWords();

        if (words.Length == 0)
            return ReadOnlyMemory<char>.Empty;

        var length = words.Length - 1; // spaces between words

        foreach (var word in words)
            length += word.Length;

        var buffer = new char[length];

        var position = 0;

        for (var i = 0; i < words.Length; i++)
        {
            if (i > 0)
                buffer[position++] = ' ';

            var word = words[i];
            word.AsSpan().CopyTo(buffer.AsSpan(position));
            position += word.Length;
        }

        return buffer;
    }
}