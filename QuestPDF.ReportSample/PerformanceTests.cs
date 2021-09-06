using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.ReportSample.Layouts;

namespace QuestPDF.ReportSample
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 0, warmupCount: 1, targetCount: 100)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class PerformanceTests
    {
        private StandardReport Report { get; set; }

        [Test]
        public void Run()
        {
            BenchmarkRunner.Run<PerformanceTests>();
        }
        
        [IterationSetup]
        public void GenerateReportData()
        {
            var model = DataSource.GetReport();
            Report = new StandardReport(model);
        }

        [Benchmark]
        public void GenerationTest()
        {
            var container = new DocumentContainer();
            Report.Compose(container);
            var content = container.Compose();
            
            var metadata = Report.GetMetadata();
            var pageContext = new PageContext();

            DocumentGenerator.RenderPass(pageContext, new FreeCanvas(), content, metadata, null);
            DocumentGenerator.RenderPass(pageContext, new FreeCanvas(), content, metadata, null);
        }
    }
}