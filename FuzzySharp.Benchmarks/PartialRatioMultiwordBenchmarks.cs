using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class PartialRatioMultiwordBenchmarks
{
    [Params(65, 257, 1025)]
    public int Length { get; set; }

    [Params("ASCII", "Cyrillic")]
    public string Alphabet { get; set; } = "ASCII";

    private string _first = "";
    private string _second = "";

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Length * 7919);
        int start = Alphabet == "Cyrillic" ? 0x400 : 'a';
        _first = new string(Enumerable.Range(0, Length)
            .Select(_ => (char)(start + random.Next(16))).ToArray());
        _second = new string(Enumerable.Range(0, Length)
            .Select(_ => (char)(start + random.Next(16))).ToArray());
    }

    [Benchmark]
    public double PartialRatio() => Fuzz.PartialRatio(_first, _second);

    [Benchmark(Baseline = true)]
    public double PartialRatioClassic() => Classic.Fuzz.PartialRatio(_first, _second);
}
