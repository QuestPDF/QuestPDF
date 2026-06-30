using System;
using QuestPDF.Tests.Shared;

namespace QuestPDF.Tests.NetFramework
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var outputDirectory = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            var pdfPath = PdfSmokeTests.GeneratePdfFile(outputDirectory, "questpdf-integration-net472.pdf");

            Console.WriteLine(pdfPath);
            return 0;
        }
    }
}
