using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

#if LODESTAR_COMPARISON
if (args.Contains("--probe-lodestar-ties"))
{
    foreach (int count in new[] { 100, 1000 })
    {
        try
        {
            new Raffinert.FuzzySharp.Benchmarks.LodestarExtractionBenchmarks { Count = count, Limit = 5, UniqueTop = false }.Setup();
            Console.WriteLine($"{count} candidates: same ranking");
        }
        catch (InvalidOperationException e) { Console.WriteLine($"{count} candidates: {e.Message}"); }
    }
    return;
}
if (args.Contains("--verify-lodestar-extraction"))
{
    foreach (int count in new[] { 100, 1000 })
    foreach (int limit in new[] { 1, 5 })
        new Raffinert.FuzzySharp.Benchmarks.LodestarExtractionBenchmarks { Count = count, Limit = limit }.Setup();
    Console.WriteLine("PASS: extraction candidates, values, scores and rankings");
    return;
}
if (args.Contains("--verify-lodestar"))
{
    Raffinert.FuzzySharp.Benchmarks.LodestarComparisonData.Verify();
    return;
}
#endif

var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.ShortRun);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
