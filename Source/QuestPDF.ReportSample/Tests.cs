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
            
            ImagePlaceholder.Solid = true;
        }
        
        [Test] 
        public void GeneratePdfAndShow()
        {
            //Settings.EnableDebugging = true;
            Report.GeneratePdfAndShow();
            return;
            

            var times = Enumerable.Range(0, 1).Select(_ => MeasureTime()).ToList();
            Console.WriteLine(string.Join(",", times));
            Console.WriteLine(times.Sum());

            long MeasureTime()
            {
                var sw = new Stopwatch();
                sw.Start();
                Report.GeneratePdf();
                sw.Stop();

                return sw.ElapsedMilliseconds;
            }
        }
        
        [Test] 
        public void GenerateXpsAndShow()
        {
            Report.GenerateXpsAndShow();
        }
    }
}