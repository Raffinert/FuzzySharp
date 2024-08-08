using BenchmarkDotNet.Attributes;
using FuzzySharp.PreProcess;

namespace FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class BenchmarkAll
{
    [Benchmark]
    public int Ratio1()
    {
        return Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int Ratio2()
    {
        return Fuzz.Ratio("mysmilarstring", "mysimilarstring");
    }

    [Benchmark]
    public int PartialRatio()
    {
        return Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public int TokenSortRatio()
    {
        return Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int PartialTokenSortRatio()
    {
        return Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSetRatio()
    {
        return Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatio()
    {
        return Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int WeightedRatio()
    {
        return Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int TokenInitialismRatio1()
    {
        return Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio2()
    {
        return Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio3()
    {
        return Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int PartialTokenInitialismRatio()
    {
        return Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int TokenAbbreviationRatio()
    {
        return Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
    }

    [Benchmark]
    public int PartialTokenAbbreviationRatio()
    {
        return Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
    }
}