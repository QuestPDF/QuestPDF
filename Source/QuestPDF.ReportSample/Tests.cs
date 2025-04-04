using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        
        [Test]
        public void StabilityTesting()
        {
            Parallel.ForEach(Enumerable.Range(0, 1000), i =>
            {
                var model = DataSource.GetReport();
                var report = new StandardReport(model);
                report.GeneratePdf();
            });
        }
        
        [Test]
        public async Task CheckFinalizersStability()
        {
            Settings.EnableCaching = true;

            Report.GeneratePdf();
            Report.GenerateImages();
            Report.GenerateSvg();

            await Task.Delay(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            await Task.Delay(1000);
        }
    }
}