using System;
using System.IO;
using System.Linq;

namespace QuestPDF.Tests.Shared;

public static class PdfValidator
{
    public static void ValidateFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new InvalidOperationException("Expected PDF file was not created: " + filePath);

        ValidateBytes(File.ReadAllBytes(filePath), filePath);
    }

    public static void ValidateBytes(byte[] pdf, string description)
    {
        if (pdf.Length < 1024)
            throw new InvalidOperationException(description + " is too small to be a meaningful PDF. Size: " + pdf.Length + " bytes.");

        var header = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };

        if (!pdf.Take(header.Length).SequenceEqual(header))
            throw new InvalidOperationException(description + " does not start with the PDF header.");

        var tail = System.Text.Encoding.ASCII.GetString(pdf, Math.Max(0, pdf.Length - 2048), Math.Min(2048, pdf.Length));

        if (!tail.Contains("%%EOF"))
            throw new InvalidOperationException(description + " does not contain the PDF EOF marker.");
    }
}
