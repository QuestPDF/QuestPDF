using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
        [Description("This test is important, as it checks if all IDisposables are properly disposed, and there are no memory leaks.")]
        public async Task CheckFinalizersStability()
        {
            Settings.EnableCaching = true;

            Report.GeneratePdf();
            Report.GenerateImages(new ImageGenerationSettings { RasterDpi = 72 });
            Report.GenerateSvg();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Report.GenerateXps();

            await Task.Delay(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            await Task.Delay(1000);
        }
        
        [Test]
        public void TestExtremelyLongParagraph()
        {
            Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));

                        page.Header()
                            .Text("Hello PDF!")
                            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(x =>
                            {
                                x.Item().Text(File.ReadAllText(@"attach.txt"));
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                })
                .GeneratePdf("TestExtremelyLongParagraph.pdf");
        }
    }
}