using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp.Benchmarks;

/// <summary>
/// Reproducible approximate-substring benchmark matrix. Equal-length and
/// shorter-candidate data sets deliberately normalize the requested text length
/// to preserve their named relationship to <see cref="PatternLength"/>.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class ApproximateSubstringRatioBenchmarks
{
    private const string Alphabet = "abcdef";
    private string _pattern = string.Empty;
    private string _text = string.Empty;
    private CachedApproximateSubstringRatioScorer _cachedScorer = null!;

    [Params(32, 63, 64, 65, 127, 128, 129, 256, 1024, 2048, 2049)]
    public int PatternLength { get; set; }

    [Params(128, 1024, 4096, 16384)]
    public int TextLength { get; set; }

    [ParamsAllValues]
    public BenchmarkDataSet DataSet { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _pattern = GenerateString(PatternLength, random, Alphabet);

        if (DataSet == BenchmarkDataSet.CandidateShorterThanCachedQuery)
        {
            _text = GenerateString(
                Math.Max(1, Math.Min(TextLength, PatternLength - 1)),
                random,
                Alphabet);
        }
        else if (DataSet == BenchmarkDataSet.EqualLengthHighSimilarity ||
                 DataSet == BenchmarkDataSet.EqualLengthLowSimilarity)
        {
            char[] equalLengthText = _pattern.ToCharArray();
            if (DataSet == BenchmarkDataSet.EqualLengthHighSimilarity)
            {
                equalLengthText[PatternLength / 2] = '#';
            }
            else
            {
                for (int index = 0; index < equalLengthText.Length; index++)
                {
                    equalLengthText[index] = 'z';
                }
            }

            _text = new string(equalLengthText);
        }
        else
        {
            int actualTextLength = Math.Max(TextLength, PatternLength + 2);
            char[] text = GenerateString(actualTextLength, random, Alphabet).ToCharArray();

            switch (DataSet)
            {
                case BenchmarkDataSet.ExactNearStart:
                    CopyPattern(text, _pattern, 1);
                    break;
                case BenchmarkDataSet.ExactNearEnd:
                    CopyPattern(text, _pattern, text.Length - PatternLength - 1);
                    break;
                case BenchmarkDataSet.ApproximateNearStart:
                    CopyApproximatePattern(text, _pattern, 1);
                    break;
                case BenchmarkDataSet.ApproximateNearEnd:
                    CopyApproximatePattern(text, _pattern, text.Length - PatternLength - 1);
                    break;
                case BenchmarkDataSet.NoExactMatch:
                case BenchmarkDataSet.RandomLowSimilarity:
                    _pattern = GenerateString(PatternLength, random, "ABCDEF");
                    break;
                case BenchmarkDataSet.RepeatedApproximateAndExact:
                    CopyApproximatePattern(text, _pattern, 1);
                    CopyPattern(text, _pattern, text.Length - PatternLength - 1);
                    break;
            }

            _text = new string(text);
        }

        _cachedScorer = new CachedApproximateSubstringRatioScorer(_pattern);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _cachedScorer.Dispose();
    }

    [Benchmark(Baseline = true)]
    public IndelSubstringMatch BestSubstringMatch()
    {
        return Indel.BestSubstringMatch(_pattern.AsSpan(), _text.AsSpan());
    }

    [Benchmark]
    public int ApproximateSubstringRatio()
    {
        return Fuzz.ApproximateSubstringRatio(_pattern, _text);
    }

    [Benchmark]
    public int CachedApproximateSubstringRatioScorerScore()
    {
        return _cachedScorer.Score(_text);
    }

    [Benchmark]
    public IndelSubstringMatch ScalarDynamicProgrammingOracle()
    {
        return ScalarBestSubstringMatch(_pattern.AsSpan(), _text.AsSpan());
    }

    [Benchmark]
    public int PartialRatio()
    {
        return Fuzz.PartialRatio(_pattern, _text);
    }

    private static IndelSubstringMatch ScalarBestSubstringMatch(
        ReadOnlySpan<char> pattern,
        ReadOnlySpan<char> text)
    {
        int[] previous = new int[pattern.Length + 1];
        int[] current = new int[pattern.Length + 1];

        for (int patternIndex = 0; patternIndex <= pattern.Length; patternIndex++)
        {
            previous[patternIndex] = patternIndex;
        }

        int bestDistance = pattern.Length;
        int bestEndIndex = -1;
        for (int textIndex = 0; textIndex < text.Length; textIndex++)
        {
            current[0] = 0;
            for (int patternIndex = 1; patternIndex <= pattern.Length; patternIndex++)
            {
                int substitutionCost = pattern[patternIndex - 1] == text[textIndex] ? 0 : 2;
                current[patternIndex] = Math.Min(
                    Math.Min(previous[patternIndex] + 1, current[patternIndex - 1] + 1),
                    previous[patternIndex - 1] + substitutionCost);
            }

            if (current[pattern.Length] < bestDistance)
            {
                bestDistance = current[pattern.Length];
                bestEndIndex = textIndex;
            }

            (previous, current) = (current, previous);
        }

        return new IndelSubstringMatch(bestDistance, bestEndIndex);
    }

    private static void CopyApproximatePattern(char[] destination, string pattern, int startIndex)
    {
        CopyPattern(destination, pattern, startIndex);
        destination[startIndex + pattern.Length / 2] = '#';
    }

    private static void CopyPattern(char[] destination, string pattern, int startIndex)
    {
        pattern.CopyTo(0, destination, startIndex, pattern.Length);
    }

    private static string GenerateString(int length, Random random, string alphabet)
    {
        var characters = new char[length];
        for (int index = 0; index < characters.Length; index++)
        {
            characters[index] = alphabet[random.Next(alphabet.Length)];
        }

        return new string(characters);
    }
}

public enum BenchmarkDataSet
{
    ExactNearStart,
    ExactNearEnd,
    ApproximateNearStart,
    ApproximateNearEnd,
    NoExactMatch,
    RandomLowSimilarity,
    RepeatedApproximateAndExact,
    EqualLengthHighSimilarity,
    EqualLengthLowSimilarity,
    CandidateShorterThanCachedQuery
}
