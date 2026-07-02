using System;
using QuestPDF.Tests.Shared;

namespace QuestPDF.Tests.NetFramework
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var outputDirectory = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            var skiaPdfFileName = args.Length > 1 ? args[1] : "questpdf-integration-netframework-skia.pdf";
            var qpdfPdfFileName = args.Length > 2 ? args[2] : "questpdf-integration-netframework-qpdf.pdf";
            var xpsFileName = args.Length > 3 && args[3].EndsWith(".xps", StringComparison.OrdinalIgnoreCase) ? args[3] : null;
            var platformTarget = args.Length > 4 ? args[4] : args.Length > 3 && xpsFileName == null ? args[3] : null;

            if (string.Equals(platformTarget, "x64", StringComparison.OrdinalIgnoreCase) && !Environment.Is64BitProcess)
                throw new InvalidOperationException("Expected the test application to run as a 64-bit process.");

            if (string.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase) && Environment.Is64BitProcess)
                throw new InvalidOperationException("Expected the test application to run as a 32-bit process.");

            var pdfOutput = PdfSmokeTests.GeneratePdfFiles(outputDirectory, skiaPdfFileName, qpdfPdfFileName);

            Console.WriteLine(Environment.Is64BitProcess ? "Process architecture: x64" : "Process architecture: x86");
            Console.WriteLine(pdfOutput.SkiaPdfPath);
            Console.WriteLine(pdfOutput.QpdfPdfPath);

            if (xpsFileName != null)
            {
                var xpsPath = PdfSmokeTests.GenerateXpsFile(outputDirectory, xpsFileName);
                Console.WriteLine(xpsPath);
            }

            return 0;
        }
    }
}
