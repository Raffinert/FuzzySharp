using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class PartialTokenSortRatioBenchmarks
{
    [Benchmark]
    public int PartialTokenSortRatio()
    {
        return Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int PartialTokenSortRatioClassic()
    {
        return Classic.Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }
}
