using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class ApproximateSubstringRatioBenchmarks
{
    private string _pattern = string.Empty;
    private string _text = string.Empty;
    private CachedApproximateSubstringRatioScorer _cachedScorer = null!;

    [Params(64, 256, 1024)]
    public int PatternLength { get; set; }

    [Params(1024, 4096)]
    public int TextLength { get; set; }

    [Params(
        BenchmarkDataSet.RepeatedApproximateAndExact,
        BenchmarkDataSet.RandomLowSimilarity)]
    public BenchmarkDataSet DataSet { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _pattern = GenerateString(PatternLength, random, "abcdefghijklmnopqrstuvwxyz");
        int actualTextLength = Math.Max(TextLength, PatternLength * 2);
        var characters = GenerateString(actualTextLength, random, "abcdefghijklmnopqrstuvwxyz").ToCharArray();

        switch (DataSet)
        {
            case BenchmarkDataSet.ExactNearStart:
                CopyPattern(characters, _pattern, 1);
                break;

            case BenchmarkDataSet.ExactNearEnd:
                CopyPattern(characters, _pattern, characters.Length - PatternLength - 1);
                break;

            case BenchmarkDataSet.NoExactMatch:
                _pattern = GenerateString(PatternLength, random, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                break;

            case BenchmarkDataSet.RepeatedApproximateAndExact:
                CopyPattern(characters, _pattern, 1);
                characters[1 + PatternLength / 2] = '#';
                CopyPattern(characters, _pattern, characters.Length - PatternLength - 1);
                break;

            case BenchmarkDataSet.HighSimilarity:
                CopyPattern(characters, _pattern, characters.Length / 2 - PatternLength / 2);
                characters[characters.Length / 2] = '#';
                break;
        }

        _text = new string(characters);
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
    public int CachedApproximateSubstringRatio()
    {
        return _cachedScorer.Score(_text);
    }

    [Benchmark]
    public IndelSubstringMatch ScalarDynamicProgrammingBaseline()
    {
        return ScalarBestSubstringMatch(_pattern.AsSpan(), _text.AsSpan());
    }

    [Benchmark]
    public int PartialRatioReference()
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
    NoExactMatch,
    RepeatedApproximateAndExact,
    RandomLowSimilarity,
    HighSimilarity
}
