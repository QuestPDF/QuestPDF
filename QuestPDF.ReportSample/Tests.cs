using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.ReportSample.Layouts;

namespace QuestPDF.ReportSample
{
    public class ReportGeneration
    {
        private StandardReport Report { get; set; }
        
        [SetUp]
        public void SetUp()
        {
            var model = DataSource.GetReport();
            Report = new StandardReport(model);
        }
        
        [Test] 
        public void GenerateAndShowPdf()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"test_result.pdf");
            Report.GeneratePdf(path);
            Process.Start("explorer.exe", path);
        }
        
        [Test] 
        public void GenerateAndShowXps()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"test_result.xps");
            Report.GenerateXps(path);
            Process.Start("explorer.exe", path);
        }
        
        [Test] 
        public void Profile()
        {
            ImagePlaceholder.Solid = true;
            
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