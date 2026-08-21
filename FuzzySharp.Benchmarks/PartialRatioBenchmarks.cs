using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialRatioBenchmarks
{
    [Benchmark]
    public double PartialRatio()
    {
        return Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public double PartialRatioClassic()
    {
        return Classic.Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }
}
