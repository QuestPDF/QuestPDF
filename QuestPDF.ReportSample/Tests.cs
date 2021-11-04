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
            ImagePlaceholder.Solid = true;
            
            var model = DataSource.GetReport();
            Report = new StandardReport(model);
            
            ImagePlaceholder.Solid = true;
        }
        
        [Test] 
        public void GenerateAndShow()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"test_result.pdf");
            Report.GeneratePdf(path);
            
            Process.Start("explorer.exe", path);
        }
        
        [Test] 
        public void Profile()
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