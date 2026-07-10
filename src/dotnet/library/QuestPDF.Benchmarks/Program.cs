using BenchmarkDotNet.Running;
using QuestPDF.Benchmarks;

// BenchmarkSwitcher shows an interactive prompt when no filter is given, which would hang in CI.
var benchmarkArgs = args.Length == 0 ? ["--filter", "*"] : args;

BenchmarkSwitcher
    .FromTypes([typeof(ImageBenchmarks), typeof(TextBenchmarks)])
    .Run(benchmarkArgs, BenchmarkConfigs.Create());
