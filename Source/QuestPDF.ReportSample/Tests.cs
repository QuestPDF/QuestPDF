using NUnit.Framework;
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
        public void GeneratePdfAndShow()
        {
            Report.GeneratePdfAndShow();
        }
        
        [Test] 
        public void GenerateXpsAndShow()
        {
            Report.GenerateXpsAndShow();
        }
    }
}