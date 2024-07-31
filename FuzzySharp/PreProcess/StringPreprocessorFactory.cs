using System;

namespace FuzzySharp.PreProcess
{
    internal static class StringPreprocessorFactory
    {
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

            return result.ToString().Trim();
        }

        public static Func<string, string> GetPreprocessor(PreprocessMode mode)
        {
            switch (mode)
            {
                case PreprocessMode.Full:
                    return Default;
                case PreprocessMode.None:
                    return s => s;
                default:
                    throw new InvalidOperationException($"Invalid string preprocessor mode: {mode}");
            }
        }
    }
}
