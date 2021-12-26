using System;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 0, warmupCount: 1, targetCount: 16)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class TablePerformanceTest
    {
        private static Random Random { get; } = new Random();
        
        private PageContext PageContext { get; set; }
        private DocumentMetadata Metadata { get; set; }
        private Container Content { get; set; }

        [Test]
        public void Run()
        {
            var configuration = ManualConfig
                .Create(DefaultConfig.Instance)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);
            
            BenchmarkRunner.Run<TablePerformanceTest>(configuration);
        }
        
        [IterationSetup]
        [SetUp]
        public void GenerateReportData()
        {
            Metadata = new DocumentMetadata()
            {
                DocumentLayoutExceptionThreshold = 1000
            };
            
            var documentContainer = new DocumentContainer();

            documentContainer.Page(page =>
            {
                page.Size(PageSizes.A3);
                GeneratePerformanceStructure(page.Content(), 10_000);
            });
            
            Content = documentContainer.Compose();

            PageContext = new PageContext();
            DocumentGenerator.RenderPass(PageContext, new FreeCanvas(), Content, Metadata, null);

            Content.HandleVisitor(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
        }

        [Benchmark]
        [Test]
        public void GenerationTest()
        {
            DocumentGenerator.RenderPass(PageContext, new FreeCanvas(), Content, Metadata, null);
        }
        
        void GeneratePerformanceStructure(IContainer container, int itemsCount)
        {
            container
                .Padding(25)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var size in Enumerable.Range(0, 10))
                            columns.ConstantColumn(100);
                    });

                    foreach (var i in Enumerable.Range(1, itemsCount))
                    {
                        table
                            .Cell()
                            .RowSpan((uint)Random.Next(1, 5))
                            .ColumnSpan((uint)Random.Next(1, 5))
                            .Element(CreateBox(i.ToString()));
                    }
                });
        }
        
        private Action<IContainer> CreateBox(string label)
        {
            return container =>
            {
                var height = Random.Next(2, 6) * 20;
                    
                container
                    .Border(2)
                    .Background(Placeholders.BackgroundColor())
                    .AlignCenter()
                    .AlignMiddle()
                    .Height(height)
                    .Text($"{label}: {height}px");
            };
        }
    }
}