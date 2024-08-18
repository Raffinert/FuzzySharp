using BenchmarkDotNet.Running;
using Raffinert.FuzzySharp.Benchmarks;
//using Raffinert.FuzzySharp;
//using Raffinert.FuzzySharp.SimilarityRatio;
//using Raffinert.FuzzySharp.SimilarityRatio.Scorer.Composite;
//using Classic = FuzzySharp;

BenchmarkRunner.Run(typeof(Program).Assembly);

//var input1 = "+30.0% Damage to Close Enemies [30.01%";
//var input2Collection = new[]
//{
//    "+#% Damage",
//    "+#% Damage to Crowd Controlled Enemies",
//    "+#% Damage to Close Enemies",
//    "+#% Damage to Chilled Enemies",
//    "+#% Damage to Poisoned Enemies",
//    "#% Block Chance#% Blocked Damage Reduction",
//    "#% Damage Reduction from Bleeding Enemies",
//    "#% Damage Reduction",
//    "+#% Cold Damage"
//};

//var classicScorer = Classic.SimilarityRatio.ScorerCache.Get<Classic.SimilarityRatio.Scorer.Composite.WeightedRatioScorer>();

//Func<string, int> classicScorerFunc = input2 => classicScorer.Score(input1, input2);

//var classicResult = input2Collection.Select(classicScorerFunc).ToList();

//var scorer = ScorerCache.Get<WeightedRatioScorer>();

//Func<string, int> scorerFunc = input2 => scorer.Score(input1, input2);

//var result = input2Collection.Select(scorerFunc).ToList();

//Console.WriteLine();

//Console.WriteLine(Fuzz.WeightedRatio("The quick brown fox jimps ofver the small lazy dog", "the quick brown fox jumps over the small lazy dog"));