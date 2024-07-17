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
            
            //ImagePlaceholder.Solid = true;
        }
        
        [Test]
        [Ignore("This test is for manual testing only.")]
        public void GeneratePdfAndShow()
        {
            Report.GeneratePdfAndShow();
        }
        
        [Test] 
        [Ignore("This test is for manual testing only.")]
        public void GenerateXpsAndShow()
        {
            Report.GenerateXpsAndShow();
        }
        
        [Test]
        public void GeneratePdfForManualVerificationTesting()
        {
            Report.GeneratePdf("report.pdf");
        }
    }
}