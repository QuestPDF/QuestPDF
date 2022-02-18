using System;
using System.Collections.Generic;
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
        
        #endregion

        #region Images

        public static IEnumerable<byte[]> GenerateImages(this IDocument document)
        {
            return DocumentGenerator.GenerateImages(document);
        }
        
        /// <param name="filePath">Method should return fileName for given index</param>
        public static void GenerateImages(this IDocument document, Func<int, string> filePath)
        {
            var index = 0;
            
            foreach (var imageData in document.GenerateImages())
            {
                var path = filePath(index);
                
                if (File.Exists(path))
                    File.Delete(path);
                
                File.WriteAllBytes(path, imageData);
                index++;
            }
        }

        #endregion
    }
}