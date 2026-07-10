using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;

namespace QuestPDF.Benchmarks;

internal static class BenchmarkConfigs
{
    private static readonly Runtime[] TargetRuntimes =
    [
        CoreRuntime.Core60,
        CoreRuntime.Core80,
        CoreRuntime.Core10_0
    ];

    public static IConfig Create()
    {
        var config = ManualConfig
            .Create(DefaultConfig.Instance)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(JsonExporter.Full);

        foreach (var runtime in TargetRuntimes)
        {
            config.AddJob(Job.Default
                .WithRuntime(runtime)
                .WithId(runtime.Name));
        }

        return config;
    }
}
