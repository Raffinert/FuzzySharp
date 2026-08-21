using BenchmarkDotNet.Attributes;
using Raffinert.FuzzySharp.Benchmarks.Utils;
using FastLevenshtein = Fastenshtein.Levenshtein;
using FuzzLevenshtein = Raffinert.FuzzySharp.Levenshtein;
using FuzzLevenshteinClassic = FuzzySharp.Levenshtein;
using QuickLevenshtein = Quickenshtein.Levenshtein;

namespace Raffinert.FuzzySharp.Benchmarks.LevenshteinDistance;

[MemoryDiagnoser]
public class LevenshteinSmall
{
    private string[] _words = null!;

    [GlobalSetup]
    public void SetUp()
    {
        _words = RandomWords.Create(20, 64);
    }

    [Benchmark(Baseline = true)]
    public void NaiveDp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                LevenshteinBaseline.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void FuzzySharp()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                FuzzLevenshteinClassic.EditDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark]
    public void Fastenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var levenshtein = new FastLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                levenshtein.DistanceFrom(_words[j]);
            }
        }
    }

    [Benchmark]
    public void Quickenshtein()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            for (int j = 0; j < _words.Length; j++)
            {
                QuickLevenshtein.GetDistance(_words[i], _words[j]);
            }
        }
    }

    [Benchmark(Description = "Raffinert.FuzzySharp")]
    public void ThisLibrary()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            using var lev = new FuzzLevenshtein(_words[i]);
            for (int j = 0; j < _words.Length; j++)
            {
                lev.DistanceFrom(_words[j]);
            }
        }
    }
}