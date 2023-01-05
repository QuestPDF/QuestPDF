using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.ReportSample.Layouts;

namespace QuestPDF.ReportSample
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 0, warmupCount: 1, targetCount: 256)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class PerformanceTests
    {
        private PageContext PageContext { get; set; }
        private DocumentMetadata Metadata { get; set; }
        private Container Content { get; set; }

        [Test]
        public void Run()
        {
            ImagePlaceholder.Solid = true;
            
            var configuration = ManualConfig
                .Create(DefaultConfig.Instance)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);
            
            BenchmarkRunner.Run<PerformanceTests>(configuration);
        }
        
        [IterationSetup]
        public void GenerateReportData()
        {
            ImagePlaceholder.Solid = true;
            
            var model = DataSource.GetReport();
            var report = new StandardReport(model);
            Metadata = report.GetMetadata();
            
            var documentContainer = new DocumentContainer();
            report.Compose(documentContainer);
            Content = documentContainer.Compose();

            PageContext = new PageContext();
            DocumentGenerator.RenderPass(PageContext, new FreeCanvas(), Content, null);

            var sw = new Stopwatch();
            sw.Start();

            Content.VisitChildren(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
            
            sw.Stop();
            Console.WriteLine($"Creating cache took: {sw.ElapsedMilliseconds}");
        }

        [Benchmark]
        public void GenerationTest()
        {
            DocumentGenerator.RenderPass(PageContext, new FreeCanvas(), Content, null);
        }
    }
}