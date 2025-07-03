using NUnit.Framework;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Raffinert.FuzzySharp.Test;

public static class RandomWordPairs
{
    public static IEnumerable<TestCaseData> GetWordPairs()
    {
        var words = RandomWords.Create(50, 1024);

        var result = from word1 in words
                     from word2 in words
                     select new TestCaseData(word1, word2);

        return result;
    }
}

// original https://github.com/DanHarltey/Fastenshtein/blob/master/benchmarks/Fastenshtein.Benchmarking/RandomWords.cs
public static class RandomWords
{
    private static readonly char[] Chars = Enumerable.Range(char.MinValue, char.MaxValue + 1)
        .Select(c => (char)c)
        .Where(char.IsLetterOrDigit)
        .ToArray();


    public static string[] Create(int count, int maxWordSize)
    {
        var words = new string[count];

        // using a const seed to make sure runs of the performance tests are consistent.
        var random = new Random(37);

        for (var i = 0; i < words.Length; i++)
        {
            var wordSize = random.Next(3, maxWordSize);

            words[i] = StringCompat.Create(wordSize, random, static (word, r) =>
            {
                for (var j = 0; j < word.Length; j++)
                {
                    var index = r.Next(0, Chars.Length);
                    word[j] = Chars[index];
                }
            });
        }

        return words;
    }

    public static class StringCompat
    {
        public static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
        {
#if NETCOREAPP
            return string.Create(length, state, action);
#else

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var chars = new char[length];
            action(chars.AsSpan(), state);
            return new string(chars);
#endif
        }
    }
#if !NETCOREAPP
    public delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);
#endif
}