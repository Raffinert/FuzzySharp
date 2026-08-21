using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialTokenSortRatioBenchmarks
{
    [Benchmark]
    public double PartialTokenSortRatio()
    {
        return Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public double PartialTokenSortRatioClassic()
    {
        return Classic.Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }
}
