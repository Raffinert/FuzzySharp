using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.PreProcess;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
public class PartialTokenAbbreviationRatioBenchmarks
{
    [Benchmark]
    public int PartialTokenAbbreviationRatio()
    {
        return Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", StringPreprocessors.Full);
    }

    [Benchmark]
    public int PartialTokenAbbreviationRatioClassic()
    {
        return Classic.Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", Classic.PreProcess.PreprocessMode.Full);
    }
}
