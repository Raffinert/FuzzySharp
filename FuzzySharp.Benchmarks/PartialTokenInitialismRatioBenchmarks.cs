using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.PreProcess;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialTokenInitialismRatioBenchmarks
{
    [Benchmark]
    public int PartialTokenInitialismRatio()
    {
        return Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int PartialTokenInitialismRatioClassic()
    {
        return Classic.Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }
}
