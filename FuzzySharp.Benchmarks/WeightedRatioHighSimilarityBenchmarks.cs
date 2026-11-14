using BenchmarkDotNet.Attributes;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class WeightedRatioHighSimilarityBenchmarks
{
    private const string Query = "an almost exact weighted ratio match";

    [Params("an almost exact weighted ratio match", "an almost exact weighted ratio matci")]
    public string Candidate { get; set; } = "";

    [Benchmark]
    public double EarlyExit()
    {
        return Fuzz.WeightedRatio(Query, Candidate);
    }

    [Benchmark(Baseline = true)]
    public double FullEvaluation()
    {
        var baseRatio = Fuzz.Ratio(Query, Candidate);
        var tokenSort = Fuzz.TokenSortRatio(Query, Candidate) * .95;
        var tokenSet = Fuzz.TokenSetRatio(Query, Candidate) * .95;
        return Math.Max(baseRatio, Math.Max(tokenSort, tokenSet));
    }
}
