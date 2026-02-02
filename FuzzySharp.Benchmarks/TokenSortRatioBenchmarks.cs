using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class TokenSortRatioBenchmarks
{
    [Benchmark]
    public int TokenSortRatio()
    {
        return Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSortRatioClassic()
    {
        return Classic.Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }
}
