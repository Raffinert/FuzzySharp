#if FUZZYSHARP_PERFORMANCE_BASELINE
extern alias baseline;
using Baseline = baseline::Raffinert.FuzzySharp;
#endif
using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class PartialRatioRegressionBenchmarks
{
    [Params("short-real", "same-7", "same-64", "equal-7", "equal-8", "equal-9", "equal-16",
        "equal-64", "equal-65", "equal-257", "edge-65", "edge-1025", "long-65", "exact-65",
        "absent-7", "absent-65", "absent-1024", "unicode-7", "unicode-65")]
    public string Scenario { get; set; } = "";

    private string _first = "", _second = "";

    [GlobalSetup]
    public void Setup()
    {
        if (Scenario == "short-real")
        {
            (_first, _second) = ("similar", "somewhresimlrbetweenthisstring");
            return;
        }

        int length = int.Parse(Scenario.Split('-')[1]);
        string alphabet = Scenario.StartsWith("unicode") ? "абвгґдеє" : "abcdefgh";
        _first = RegressionBenchmarkData.RandomText(length, length, alphabet);
        _second = Scenario.Split('-')[0] switch
        {
            "same" => _first,
            "equal" or "unicode" => RegressionBenchmarkData.RandomText(length, length + 17, alphabet),
            "exact" => "zz" + _first + "zz",
            "long" => RegressionBenchmarkData.RandomText(512, 71) + RegressionBenchmarkData.RandomText(length, 72)
                + RegressionBenchmarkData.RandomText(512, 73),
            "absent" => new string('z', length),
            _ => RegressionBenchmarkData.RandomText(length + 17, length + 17)
        };
        if (Scenario.StartsWith("absent"))
            _first = new string('a', length);
    }

    [Benchmark] public double Latest() => Fuzz.PartialRatio(_first, _second);
#if FUZZYSHARP_PERFORMANCE_BASELINE
    [Benchmark(Baseline = true)] public double Before() => Baseline.Fuzz.PartialRatio(_first, _second);
#endif
}

[MemoryDiagnoser]
public class TokenSetRegressionBenchmarks
{
    [Params("small", "subset", "disjoint", "long-overlap")]
    public string Scenario { get; set; } = "";
    private string _first = "", _second = "";

    [GlobalSetup] public void Setup() => (_first, _second) = RegressionBenchmarkData.Tokens(Scenario);
    [Benchmark] public double Latest() => Fuzz.TokenSetRatio(_first, _second);
#if FUZZYSHARP_PERFORMANCE_BASELINE
    [Benchmark(Baseline = true)] public double Before() => Baseline.Fuzz.TokenSetRatio(_first, _second);
#endif
}

[MemoryDiagnoser]
public class CachedTokenSetRegressionBenchmarks
{
    [Params("small", "subset", "disjoint", "long-overlap")]
    public string Scenario { get; set; } = "";
    private string _second = "";
    private CachedTokenSetScorer _latest = null!;
#if FUZZYSHARP_PERFORMANCE_BASELINE
    private Baseline.SimilarityRatio.Scorer.StrategySensitive.CachedTokenSetScorer _before = null!;
#endif

    [GlobalSetup]
    public void Setup()
    {
        var (first, second) = RegressionBenchmarkData.Tokens(Scenario);
        _second = second;
        _latest = new CachedTokenSetScorer(first);
#if FUZZYSHARP_PERFORMANCE_BASELINE
        _before = new Baseline.SimilarityRatio.Scorer.StrategySensitive.CachedTokenSetScorer(first);
#endif
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _latest.Dispose();
#if FUZZYSHARP_PERFORMANCE_BASELINE
        _before.Dispose();
#endif
    }

    [Benchmark] public double Latest() => _latest.Score(_second);
#if FUZZYSHARP_PERFORMANCE_BASELINE
    [Benchmark(Baseline = true)] public double Before() => _before.Score(_second);
#endif
}

[MemoryDiagnoser]
public class LcsRegressionBenchmarks
{
    [Params(65, 257, 1025)] public int Length { get; set; }
    private string _first = "", _second = "";
    [GlobalSetup] public void Setup()
    {
        _first = RegressionBenchmarkData.RandomText(Length, 41);
        _second = RegressionBenchmarkData.RandomText(Length + 17, 43);
    }
    [Benchmark] public int Latest() => LongestCommonSubsequence.Similarity<char>(_first, _second);
#if FUZZYSHARP_PERFORMANCE_BASELINE
    [Benchmark(Baseline = true)] public int Before() => Baseline.LongestCommonSubsequence.Similarity<char>(_first, _second);
#endif
}

[MemoryDiagnoser]
public class MatrixRegressionBenchmarks
{
    [Params(129, 1025)] public int Length { get; set; }
    private string _first = "", _second = "";
    [GlobalSetup] public void Setup()
    {
        _first = RegressionBenchmarkData.RandomText(Length, 41);
        _second = RegressionBenchmarkData.RandomText(Length + 17, 43);
    }
    [Benchmark] public int Latest() => LongestCommonSubsequence.Matrix<char>(_first, _second).Sim;
#if FUZZYSHARP_PERFORMANCE_BASELINE
    [Benchmark(Baseline = true)] public int Before() => Baseline.LongestCommonSubsequence.Matrix<char>(_first, _second).Sim;
#endif
}

internal static class RegressionBenchmarkData
{
    internal static string RandomText(int length, int seed, string alphabet = "abcdefgh")
    {
        var random = new Random(seed);
        var chars = new char[length];
        for (int i = 0; i < chars.Length; i++)
            chars[i] = alphabet[random.Next(alphabet.Length)];
        return new string(chars);
    }

    internal static (string First, string Second) Tokens(string scenario) => scenario switch
    {
        "small" => ("fuzzy was a bear", "fuzzy fuzzy fuzzy bear"),
        "subset" => ("apple banana", "apple banana orange"),
        "disjoint" => ("apple banana cherry", "kiwi lemon pear"),
        _ => (string.Join(' ', Enumerable.Range(0, 32).Select(i => $"token{i:D3}")),
            string.Join(' ', Enumerable.Range(16, 32).Select(i => $"token{i:D3}")))
    };
}
