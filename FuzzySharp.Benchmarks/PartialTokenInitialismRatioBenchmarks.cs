using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialTokenInitialismRatioBenchmarks
{
    [Benchmark]
    public double PartialTokenInitialismRatio()
    {
        return Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public double PartialTokenInitialismRatioClassic()
    {
        return Classic.Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }
}
