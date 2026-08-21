using BenchmarkDotNet.Attributes;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialRatioLongBenchmarks
{
    private string _s1 = "";
    private string _s2 = "";

    [GlobalSetup]
    public void Setup()
    {
        // Short pattern in long text
        _s1 = "testing algorithm";

        // Generate 500-character string with pattern embedded
        var rnd = new Random(42);
        var text = GenerateString(250, rnd) + _s1 + GenerateString(250, rnd);
        _s2 = text;
    }

    private static string GenerateString(int length, Random rnd)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        var result = new char[length];
        for (int i = 0; i < length; i++)
            result[i] = chars[rnd.Next(chars.Length)];
        return new string(result);
    }

    [Benchmark]
    public double PartialRatio()
    {
        return Fuzz.PartialRatio(_s1, _s2);
    }

    [Benchmark]
    public double PartialRatioClassic()
    {
        return Classic.Fuzz.PartialRatio(_s1, _s2);
    }
}
