using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.PreProcess;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class TokenAbbreviationRatioBenchmarks
{
    [Benchmark]
    public double TokenAbbreviationRatio()
    {
        return Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessor.Full);
    }

    [Benchmark]
    public double TokenAbbreviationRatioClassic()
    {
        return Classic.Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", Classic.PreProcess.PreprocessMode.Full);
    }
}
