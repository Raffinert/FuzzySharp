using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Extractor;
using Raffinert.FuzzySharp.PreProcess;
using Classic = FuzzySharp;

namespace Raffinert.FuzzySharp.Benchmarks;

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

    [Benchmark]
    public int Ratio1Classic()
    {
        return Classic.Fuzz.Ratio("mysmilarstring", "myawfullysimilarstirng");
    }

    [Benchmark]
    public int Ratio2Classic()
    {
        return Classic.Fuzz.Ratio("mysmilarstring", "mysimilarstring");
    }

    [Benchmark]
    public int PartialRatioClassic()
    {
        return Classic.Fuzz.PartialRatio("similar", "somewhresimlrbetweenthisstring");
    }

    [Benchmark]
    public int TokenSortRatioClassic()
    {
        return Classic.Fuzz.TokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int PartialTokenSortRatioClassic()
    {
        return Classic.Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
    }

    [Benchmark]
    public int TokenSetRatioClassic()
    {
        return Classic.Fuzz.TokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int PartialTokenSetRatioClassic()
    {
        return Classic.Fuzz.PartialTokenSetRatio("fuzzy was a bear", "fuzzy fuzzy fuzzy bear");
    }

    [Benchmark]
    public int WeightedRatioClassic()
    {
        return Classic.Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog");
    }

    [Benchmark]
    public int TokenInitialismRatio1Classic()
    {
        return Classic.Fuzz.TokenInitialismRatio("NASA", "National Aeronautics and Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio2Classic()
    {
        return Classic.Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration");
    }

    [Benchmark]
    public int TokenInitialismRatio3Classic()
    {
        return Classic.Fuzz.TokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int PartialTokenInitialismRatioClassic()
    {
        return Classic.Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int TokenAbbreviationRatioClassic()
    {
        return Classic.Fuzz.TokenAbbreviationRatio("bl 420", "Baseline section 420", Classic.PreProcess.PreprocessMode.Full);
    }

    [Benchmark]
    public int PartialTokenAbbreviationRatioClassic()
    {
        return Classic.Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", Classic.PreProcess.PreprocessMode.Full);
    }

    private static readonly string[][] Events =
    [
        ["chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm"],
        ["new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm"],
        ["atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm"]
    ];

    private static readonly string[] Query = ["new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm"];

    [Benchmark]
    public ExtractedResult<string[]> ExtractOne()
    {
        return Process.ExtractOne(Query, Events, static strings => strings[0]);
    }

    [Benchmark]
    public Classic.Extractor.ExtractedResult<string[]> ExtractOneClassic()
    {
        return Classic.Process.ExtractOne(Query, Events, static strings => strings[0]);
    }
}