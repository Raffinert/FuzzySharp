using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialRatioBenchmarks
{
    [Benchmark]
    public int PartialRatio()
    {
        return Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public int PartialRatioClassic()
    {
        return Classic.Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }
}
