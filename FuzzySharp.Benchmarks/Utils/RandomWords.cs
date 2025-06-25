namespace Raffinert.FuzzySharp.Benchmarks.Utils;

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

            words[i] = string.Create(wordSize, random, static (word, r) =>
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
}