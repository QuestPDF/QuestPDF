using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class GenerateExtensions
    {
        #region PDF
        
        public static byte[] GeneratePdf(this IDocument document)
        {
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }
        
        public static void GeneratePdf(this IDocument document, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            document.GeneratePdf(stream);
        }

        public static void GeneratePdf(this IDocument document, Stream stream)
        {
            DocumentGenerator.GeneratePdf(stream, document);
        }
        
        private static int GenerateAndShowCounter = 0;
        
        public static void GeneratePdfAndShow(this IDocument document)
        {
            GenerateAndShowCounter++;
            
            var filePath = Path.Combine(Path.GetTempPath(), $"QuestPDF Document {GenerateAndShowCounter}.pdf");
            document.GeneratePdf(filePath);
            OpenFileUsingDefaultProgram(filePath);
        }

        #endregion

        #region XPS
        
        public static byte[] GenerateXps(this IDocument document)
        {
            using var stream = new MemoryStream();
            document.GenerateXps(stream);
            return stream.ToArray();
        }
        
        public static void GenerateXps(this IDocument document, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            document.GenerateXps(stream);
        }

        public static void GenerateXps(this IDocument document, Stream stream)
        {
            DocumentGenerator.GenerateXps(stream, document);
        }
        
        public static void GenerateXpsAndShow(this IDocument document)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"QuestPDF Document.xps");
            document.GenerateXps(filePath);
            OpenFileUsingDefaultProgram(filePath);
        }
        
        #endregion

        #region Images

        public static IEnumerable<byte[]> GenerateImages(this IDocument document)
        {
            return document.GenerateImages(ImageGenerationSettings.Default);
        }


        public static IEnumerable<byte[]> GenerateImages(this IDocument document, ImageGenerationSettings settings)
        {
            return DocumentGenerator.GenerateImages(document, settings);
        }

        /// <param name="filePath">Method should return fileName for given index</param>
        public static void GenerateImages(this IDocument document, Func<int, string> filePath)
        {
            document.GenerateImages(filePath, ImageGenerationSettings.Default);
        }

        /// <param name="filePath">Method should return fileName for given index</param>
        public static void GenerateImages(this IDocument document, Func<int, string> filePath, ImageGenerationSettings settings)
        {
            var index = 0;

            foreach (var imageData in document.GenerateImages(settings))
            {
                var path = filePath(index);

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllBytes(path, imageData);
                index++;
            }
        }

        #endregion

        #region Helpers

        private static void OpenFileUsingDefaultProgram(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };

            process.Start();
        }
        
        #endregion
    }
}