using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace QuestPDF.Benchmarks;

public class Config : ManualConfig
{
    public Config()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        
        AddJob(Job.Default);
    }
}