using BenchmarkDotNet.Attributes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

public class ImageBenchmarks
{
    private byte[][] Images { get; set; } = [];

    [GlobalSetup]
    public void Setup()
    {
        BenchmarkEnvironment.ConfigureQuestPdf();

        Images = Enumerable
            .Range(1, 3)
            .Select(x => Path.Combine("Resources", $"image{x}.jpg"))
            .Select(File.ReadAllBytes)
            .ToArray();
    }

    [Benchmark]
    public byte[] GeneratePdf()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(1500, 2000);
                    page.Content().Image(Images[0]);
                });
                
                document.Page(page =>
                {
                    page.Size(750, 1000);
                    page.Content().Image(Images[1]);
                });
                
                document.Page(page =>
                {
                    page.Size(300, 400);
                    page.Content().Image(Images[2]);
                });
            })
            .WithSettings(new DocumentSettings()
            {
                ImageRasterDpi = 72 // 1pt = 1px
            })
            .GeneratePdf();
    }
}
