#if LODESTAR_COMPARISON
using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Rival = Lodestar.Fuzzy;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class LodestarExtractionBenchmarks
{
    [Params(100, 1000)] public int Count { get; set; }
    [Params(1, 5)] public int Limit { get; set; }
    public bool UniqueTop { get; set; } = true;
    private const string Query = "the quick brown fox jumps over the lazy dog";
    private string[] _choices = [];
    private readonly DefaultRatioScorer _scorer = new();
    private static readonly Func<string, string> Identity = static s => s;
    private static readonly Func<string, string, double> RivalScorer = Rival.Fuzz.Ratio;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(390039);
        _choices = Enumerable.Range(0, Count).Select(_ => new string(
            Enumerable.Range(0, Query.Length).Select(_ => (char)('a' + random.Next(16))).ToArray())).ToArray();
        for (int i = 0; i < (UniqueTop ? 5 : 1); i++)
            _choices[Count / 2 + i] = Query[..^(i + 1)] + new string('!', i + 1);
        foreach (string choice in _choices)
            if (Math.Abs(Fuzz.Ratio(Query, choice) - Rival.Fuzz.Ratio(Query, choice)) > 1e-9)
                throw new InvalidOperationException("Candidate score mismatch");
        if (Limit == 1)
        {
            var ours = Process.ExtractOne(Query, _choices, Identity, _scorer);
            var theirs = Rival.Process.ExtractOne(Query, _choices, RivalScorer)!.Value;
            if (ours.Index != theirs.Index || ours.Value != theirs.Choice || Math.Abs(ours.Score - theirs.Score) > 1e-9)
                throw new InvalidOperationException("ExtractOne mismatch");
        }
        else
        {
            var ours = Process.ExtractTop(Query, _choices, processor: Identity, scorer: _scorer, limit: Limit).ToArray();
            var theirs = Rival.Process.Extract(Query, _choices, scorer: RivalScorer, limit: Limit);
            if (ours.Length != theirs.Count) throw new InvalidOperationException("Top count mismatch");
            for (int i = 0; i < ours.Length; i++)
                if (ours[i].Index != theirs[i].Index || ours[i].Value != theirs[i].Choice || Math.Abs(ours[i].Score - theirs[i].Score) > 1e-9)
                    throw new InvalidOperationException($"Top ranking mismatch at {i}: ours {ours[i]}, theirs {theirs[i]}");
        }
    }

    [Benchmark]
    public double Raffinert() => Limit == 1
        ? Process.ExtractOne(Query, _choices, Identity, _scorer).Score
        : Process.ExtractTop(Query, _choices, processor: Identity, scorer: _scorer, limit: Limit).ToArray()[0].Score;

    [Benchmark(Baseline = true)]
    public double Lodestar() => Limit == 1
        ? Rival.Process.ExtractOne(Query, _choices, RivalScorer)!.Value.Score
        : Rival.Process.Extract(Query, _choices, scorer: RivalScorer, limit: Limit)[0].Score;
}
#endif
