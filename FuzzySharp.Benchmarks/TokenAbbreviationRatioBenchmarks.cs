using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.PreProcess;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class TokenAbbreviationRatioBenchmarks
{
    [Benchmark]
    public int TokenAbbreviationRatio()
    {
        return Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessors.Full);
    }

    [Benchmark]
    public int TokenAbbreviationRatioClassic()
    {
        return Classic.Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", Classic.PreProcess.PreprocessMode.Full);
    }
}
