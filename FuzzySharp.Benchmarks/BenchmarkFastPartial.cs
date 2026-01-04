using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.PreProcess;

namespace Raffinert.FuzzySharp.Benchmarks;

[MemoryDiagnoser]
public class BenchmarkFastPartial
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        GlobalConfig.PartialRatioAccuracy = PartialRatioAccuracy.Fast;
    }

    [Benchmark]
    public int PartialRatio()
    {
        return Fuzz.PartialRatio("Supplier: ACME Corp. International, Address: 221B Baker St., London NW1 6XE", "Order: PO-100923, Supplier: Acme Corporation International, Address: 221B Baker Street, London NW1 6XE, VAT: GB123456789, Contact: accounting@acme.example");
    }

    [Benchmark]
    public int PartialTokenSortRatio()
    {
        return Fuzz.PartialTokenSortRatio("order words out of", "  words out of order");
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
    public int PartialTokenInitialismRatio()
    {
        return Fuzz.PartialTokenInitialismRatio("NASA", "National Aeronautics Space Administration, Kennedy Space Center, Cape Canaveral, Florida 32899");
    }

    [Benchmark]
    public int PartialTokenAbbreviationRatio()
    {
        return Fuzz.PartialTokenAbbreviationRatio("bl 420", "Baseline section 420", PreprocessMode.Full);
    }
}