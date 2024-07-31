using System;
using System.Collections.Generic;

namespace FuzzySharp.Extensions
{
    internal static class StringExtensions
    {
        public static List<string> ExtractLetterOnlyWords(this string input)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(input))
                return result;

            var span = input.AsSpan();

            int start = 0;
            for (var i = 0; i < span.Length; i++)
            {
                if (!char.IsLetter(span[i]))
                {
                    if (i - start > 0)
                    {
                        result.Add(span.Slice(start, i - start).ToString());
                    }

                    start = i+1;
                }
            }

            if (span.Length - start > 0)
                result.Add(span.Slice(start, span.Length - start).ToString());

            return result;
        }

        public static string[] SplitByAnySpace(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<string>();

            var words = input.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

            return words;
        }

        public static string[] GetSortedWords(this string input)
        {
            var words = SplitByAnySpace(input);

            Array.Sort(words);

            return words;
        }

        public static string NormalizeSpacesAndSort(this string input)
        {
            var words = GetSortedWords(input);

            return string.Join(' ', words);
        }
    }
}
