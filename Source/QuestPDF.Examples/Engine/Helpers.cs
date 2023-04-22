using System;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public static class Helpers
    {
        public static void GeneratePdfAndOpen(this Document document, string? fileName = null)
        {
            fileName ??= $"{Guid.NewGuid():D}.pdf";

            var filePath = Path.GetTempPath() + fileName;
            var documentData = document.GeneratePdf();
            File.WriteAllBytes(filePath, documentData);
            
            Process.Start("explorer", filePath);
        }
    }
}