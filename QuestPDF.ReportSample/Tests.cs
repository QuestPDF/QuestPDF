using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.ReportSample.Layouts;

namespace QuestPDF.ReportSample
{
    public class ReportGeneration
    {
        [Test] 
        public void GenerateAndShow()
        {
            var reportModel = DataSource.GetReport();
            var report = new StandardReport(reportModel);
            
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"test_result.pdf");
            using var stream = new FileStream(path, FileMode.Create);
            report.Generate(stream);
            
            Process.Start("explorer.exe", path);
        }

        [Test] 
        public void PerformanceBenchmark()
        {
            // test size
            const int testSize = 1000;
            const decimal performanceTarget = 5; // documents per second

            // create report models
            var reports = Enumerable
                .Range(0, testSize)
                .Select(x =>
                {
                    var reportModel = DataSource.GetReport();
                    return new StandardReport(reportModel);
                })
                .ToList();

            // generate documents
            var sw = new Stopwatch();
            
            sw.Start();
            var totalSize = reports.Select(x => x.Generate()).Sum(x => (long)x.Length);
            sw.Stop();

            // show summary
            Console.WriteLine($"Total size: {totalSize:N0} bytes");
            
            var performance = sw.ElapsedMilliseconds / (decimal)testSize;
            var speed = 1000M / performance;
            Console.WriteLine($"Test time: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Time per document: {performance:N} ms");
            Console.WriteLine($"Documents per second: {speed:N} d/s");

            if (speed < performanceTarget)
                throw new Exception("Rendering algorithm is too slow.");
        }
    }
}