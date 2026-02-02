using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class RatioBenchmarks
{
    [Benchmark]
    public int Ratio()
    {
        return Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int RatioClassic()
    {
        return Classic.Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }
}
