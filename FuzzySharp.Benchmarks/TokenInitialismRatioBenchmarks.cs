using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class TokenInitialismRatioBenchmarks
{
    [Benchmark]
    public double TokenInitialismRatio1()
    {
        return Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
    }

    [Benchmark]
    public double TokenInitialismRatio1Classic()
    {
        return Classic.Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
    }

    [Benchmark]
    public double TokenInitialismRatio2()
    {
        return Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
    }

    [Benchmark]
    public double TokenInitialismRatio2Classic()
    {
        return Classic.Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
    }

    [Benchmark]
    public double TokenInitialismRatio3()
    {
        return Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public double TokenInitialismRatio3Classic()
    {
        return Classic.Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }
}
