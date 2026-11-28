#if LODESTAR_COMPARISON
using BenchmarkDotNet.Attributes;
using Rival = Lodestar.Fuzzy.Fuzz;

namespace Raffinert.FuzzySharp.Benchmarks;

public enum LodestarOperation { Ratio, PartialRatio, TokenSetRatio, WeightedRatio }

public static class LodestarComparisonData
{
    public static IEnumerable<string> Cases()
    {
        yield return "Original";
        foreach (int length in new[] { 8, 16, 32, 64, 65, 128, 256, 1024 })
        foreach (string alphabet in new[] { "ASCII", "Cyrillic", "Mixed" })
        foreach (string kind in new[] { "Equal", "Exact", "Near", "Absent" })
            yield return $"{length:D4}-{alphabet}-{kind}";
    }

    public static (string First, string Second) Inputs(string scenario)
    {
        if (scenario == "Original")
            return ("the quick brown fox jumps over the lazy dog", "the lazy dog jumps over the quick brown fox");
        var parts = scenario.Split('-');
        int length = int.Parse(parts[0]);
        string alphabet = parts[1] switch
        {
            "ASCII" => "abcdefghijklmnop",
            "Cyrillic" => "абвгґдеєжзиіїйклм",
            _ => "abґє漢字éΩ" // Valid BMP Unicode; UTF-16 units on both APIs.
        };
        var random = new Random(390039 + length);
        string Text(int count) => new(Enumerable.Range(0, count).Select(_ => alphabet[random.Next(alphabet.Length)]).ToArray());
        string first = Text(length);
        string near = first[..(length / 2)] + "!" + first[(length / 2 + 1)..];
        string second = parts[2] switch
        {
            "Equal" => Text(length),
            "Exact" => "##" + Text(length) + first + Text(length) + "##",
            "Near" => "##" + Text(length) + near + Text(length) + "##",
            _ => new string('!', length * 3)
        };
        return (first, second);
    }

    public static double Score(bool rival, LodestarOperation operation, string a, string b) => (rival, operation) switch
    {
        (true, LodestarOperation.Ratio) => Rival.Ratio(a, b),
        (true, LodestarOperation.PartialRatio) => Rival.PartialRatio(a, b),
        (true, LodestarOperation.TokenSetRatio) => Rival.TokenSetRatio(a, b),
        (true, _) => Rival.WRatio(a, b),
        (false, LodestarOperation.Ratio) => Fuzz.Ratio(a, b),
        (false, LodestarOperation.PartialRatio) => Fuzz.PartialRatio(a, b),
        (false, LodestarOperation.TokenSetRatio) => Fuzz.TokenSetRatio(a, b),
        _ => Fuzz.WeightedRatio(a, b)
    };

    public static void Verify()
    {
        Console.WriteLine("Scenario,Operation,Raffinert,Lodestar,Equal");
        int differences = 0;
        int comparisons = 0;
        foreach (string scenario in Cases())
        foreach (var operation in Enum.GetValues<LodestarOperation>())
        {
            var (a, b) = Inputs(scenario);
            double ours = Score(false, operation, a, b), theirs = Score(true, operation, a, b);
            bool same = Math.Abs(ours - theirs) <= 1e-9;
            comparisons++;
            Console.WriteLine(FormattableString.Invariant($"{scenario},{operation},{ours:R},{theirs:R},{same}"));
            if (!same) differences++;
        }
        Console.Error.WriteLine($"Verified {comparisons} pairs; score differences: {differences}. Different scores are refused by benchmark setup.");
    }
}

[MemoryDiagnoser]
public class LodestarComparisonBenchmarks
{
    // Representative measured suite; the full 388-pair semantic matrix is always verified.
    [Params("Original", "0008-ASCII-Near", "0064-ASCII-Near", "0065-Cyrillic-Near",
        "1024-Mixed-Near", "0065-ASCII-Exact", "0065-Cyrillic-Absent", "0256-Mixed-Equal",
        "0016-Cyrillic-Equal", "0032-Mixed-Exact", "0128-ASCII-Near")]
    public string Scenario { get; set; } = "Original";

    [Params(LodestarOperation.Ratio, LodestarOperation.PartialRatio, LodestarOperation.TokenSetRatio,
        LodestarOperation.WeightedRatio)]
    public LodestarOperation Operation { get; set; }
    private string _a = "", _b = "";

    [GlobalSetup]
    public void Setup()
    {
        (_a, _b) = LodestarComparisonData.Inputs(Scenario);
        double ours = Raffinert(), theirs = Lodestar();
        if (Math.Abs(ours - theirs) > 1e-9)
            throw new InvalidOperationException($"Score mismatch: {Scenario}/{Operation}: {ours:R} vs {theirs:R}");
    }

    [Benchmark] public double Raffinert() => LodestarComparisonData.Score(false, Operation, _a, _b);
    [Benchmark(Baseline = true)] public double Lodestar() => LodestarComparisonData.Score(true, Operation, _a, _b);
}
#endif
