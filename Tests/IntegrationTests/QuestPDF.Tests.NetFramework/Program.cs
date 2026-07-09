using System;
using QuestPDF.Tests.Shared;

namespace QuestPDF.Tests.NetFramework
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var platformTarget = args.Length > 0 ? args[0] : null;

            if (string.Equals(platformTarget, "x64", StringComparison.OrdinalIgnoreCase) && !Environment.Is64BitProcess)
                throw new InvalidOperationException("Expected the test application to run as a 64-bit process.");

            if (string.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase) && Environment.Is64BitProcess)
                throw new InvalidOperationException("Expected the test application to run as a 32-bit process.");

            PdfSmokeTests.GenerateAllSupportedFiles(Environment.CurrentDirectory);

            return 0;
        }
    }
}
