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
        
        /// <summary>
        /// Generates the document in PDF format and returns it as a byte array.
        /// </summary>
        public static byte[] GeneratePdf(this IDocument document)
        {
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }
        
        /// <summary>
        /// Generates the document in PDF format and saves it to the specified file path.
        /// </summary>
        public static void GeneratePdf(this IDocument document, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            document.GeneratePdf(stream);
        }

        /// <summary>
        /// Generates the document in PDF format and outputs it to a provided stream.
        /// </summary>
        public static void GeneratePdf(this IDocument document, Stream stream)
        {
            DocumentGenerator.GeneratePdf(stream, document);
        }
        
        private static int GenerateAndShowCounter = 0;
        
        /// <summary>
        /// Generates the document in PDF format, saves it in temporary file, and then opens it with the default application.
        /// </summary>
        public static void GeneratePdfAndShow(this IDocument document)
        {
            GenerateAndShowCounter++;
            
            var filePath = Path.Combine(Path.GetTempPath(), $"QuestPDF Document {GenerateAndShowCounter}.pdf");
            document.GeneratePdf(filePath);
            OpenFileUsingDefaultProgram(filePath);
        }

        #endregion

        #region XPS
        
        /// <summary>
        /// Generates the document in XPS format and returns it as a byte array.
        /// </summary>
        public static byte[] GenerateXps(this IDocument document)
        {
            using var stream = new MemoryStream();
            document.GenerateXps(stream);
            return stream.ToArray();
        }
        
        /// <summary>
        /// Generates the document in XPS format and saves it to the specified file path.
        /// </summary>
        public static void GenerateXps(this IDocument document, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            document.GenerateXps(stream);
        }

        /// <summary>
        /// Generates the document in XPS format and outputs it to a provided stream.
        /// </summary>
        public static void GenerateXps(this IDocument document, Stream stream)
        {
            DocumentGenerator.GenerateXps(stream, document);
        }
        
        /// <summary>
        /// Generates the document in XPS format, saves it in temporary file, and then opens it with the default application.
        /// </summary>
        public static void GenerateXpsAndShow(this IDocument document)
        {
            GenerateAndShowCounter++;
            
            var filePath = Path.Combine(Path.GetTempPath(), $"QuestPDF Document {GenerateAndShowCounter}.xps");
            document.GenerateXps(filePath);
            OpenFileUsingDefaultProgram(filePath);
        }
        
        #endregion

        #region Images

        /// <summary>
        /// Generates the document as a series of images and returns them as a collection of byte arrays.
        /// </summary>
        /// <param name="settings">Optional settings to customize the output images, such as resolution, compression ratio, and more.</param>
        public static IEnumerable<byte[]> GenerateImages(this IDocument document, ImageGenerationSettings? settings = null)
        {
            settings ??= ImageGenerationSettings.Default;
            return DocumentGenerator.GenerateImages(document, settings);
        }

        /// <param name="imageIndex">Specifies the index of the generated image from the document, starting at 0.</param>
        /// <returns>The file path where the image should be saved.</returns>
        public delegate string GenerateDocumentImagePath(int imageIndex);
        
        /// <summary>
        /// Generates the document as a sequence of images, saving them to paths determined by the <paramref name="imagePathSource"/> delegate.
        /// </summary>
        /// <param name="imagePathSource">A delegate that gets image index as an input, and returns file path where it should be saved.</param>
        /// <param name="settings">Optional settings for fine-tuning the generated images, such as resolution, compression ratio, etc.</param>
        public static void GenerateImages(this IDocument document, GenerateDocumentImagePath imagePathSource, ImageGenerationSettings? settings = null)
        {
            settings ??= ImageGenerationSettings.Default;
            
            var index = 0;

            foreach (var imageData in document.GenerateImages(settings))
            {
                var path = imagePathSource(index);

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllBytes(path, imageData);
                index++;
            }
        }

        #endregion

        #region Helpers

        internal static void OpenFileUsingDefaultProgram(string filePath)
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