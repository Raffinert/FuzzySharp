using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Raffinert.FuzzySharp.Benchmarks;

var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.ShortRun);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);