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
            QuestPDF.Settings.License = LicenseType.Community;
            
            var model = DataSource.GetReport();
            Report = new StandardReport(model);
        }
        
        [Test] 
        public void GenerateAndShowPdf()
        {
            //ImagePlaceholder.Solid = true;
        
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
    }
}