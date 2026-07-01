using System;
using QuestPDF.Tests.Shared;

namespace QuestPDF.Tests.NetFramework
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var outputDirectory = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            var fileName = args.Length > 1 ? args[1] : "questpdf-integration-netframework.pdf";
            var platformTarget = args.Length > 2 ? args[2] : null;

            if (string.Equals(platformTarget, "x64", StringComparison.OrdinalIgnoreCase) && !Environment.Is64BitProcess)
                throw new InvalidOperationException("Expected the test application to run as a 64-bit process.");

            if (string.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase) && Environment.Is64BitProcess)
                throw new InvalidOperationException("Expected the test application to run as a 32-bit process.");

            var pdfPath = PdfSmokeTests.GeneratePdfFile(outputDirectory, fileName);

            Console.WriteLine(Environment.Is64BitProcess ? "Process architecture: x64" : "Process architecture: x86");
            Console.WriteLine(pdfPath);
            return 0;
        }
    }
}
