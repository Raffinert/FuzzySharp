using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Raffinert.FuzzySharp.Benchmarks;

var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.ShortRun);

BenchmarkSwitcher.FromAssembly(typeof(BenchmarkFastPartial).Assembly).Run(args, config);