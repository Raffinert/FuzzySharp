using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.PreProcess;
using RaffinertFuzz = Raffinert.FuzzySharp.Fuzz;
using ClassicFuzz = FuzzySharp.Fuzz;
using ClassicPreprocess = FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class BenchmarkAll
{
    [Benchmark]
    public int Ratio1()
    {
        return RaffinertFuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int Ratio2()
    {
        return RaffinertFuzz.Ratio("mysmilarstring", "mysimilarstring");
    }

    [Benchmark]
    public int PartialRatio()
    {
        return RaffinertFuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public int TokenSortRatio()
    {
        return RaffinertFuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int PartialTokenSortRatio()
    {
        return RaffinertFuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSetRatio()
    {
        return RaffinertFuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatio()
    {
        return RaffinertFuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int WeightedRatio()
    {
        return RaffinertFuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int TokenInitialismRatio1()
    {
        return RaffinertFuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio2()
    {
        return RaffinertFuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio3()
    {
        return RaffinertFuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int PartialTokenInitialismRatio()
    {
        return RaffinertFuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int TokenAbbreviationRatio()
    {
        return RaffinertFuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
    }

    [Benchmark]
    public int PartialTokenAbbreviationRatio()
    {
        return RaffinertFuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
    }

    [Benchmark]
    public int Ratio1Classic()
    {
        return ClassicFuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int Ratio2Classic()
    {
        return ClassicFuzz.Ratio("mysmilarstring", "mysimilarstring");
    }

    [Benchmark]
    public int PartialRatioClassic()
    {
        return ClassicFuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public int TokenSortRatioClassic()
    {
        return ClassicFuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int PartialTokenSortRatioClassic()
    {
        return ClassicFuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSetRatioClassic()
    {
        return ClassicFuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatioClassic()
    {
        return ClassicFuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int WeightedRatioClassic()
    {
        return ClassicFuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int TokenInitialismRatio1Classic()
    {
        return ClassicFuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio2Classic()
    {
        return ClassicFuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio3Classic()
    {
        return ClassicFuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int PartialTokenInitialismRatioClassic()
    {
        return ClassicFuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int TokenAbbreviationRatioClassic()
    {
        return ClassicFuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", ClassicPreprocess.PreprocessMode.Full);
    }

    [Benchmark]
    public int PartialTokenAbbreviationRatioClassic()
    {
        return ClassicFuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", ClassicPreprocess.PreprocessMode.Full);
    }
}