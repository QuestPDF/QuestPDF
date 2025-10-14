using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Fluent
{
    public static class GenerateExtensions
    {
        static GenerateExtensions()
        {
            ClearGenerateAndShowFiles();
        }
        
        internal static void GenerateAndDiscard(this IDocument document)
        {
            DocumentGenerator.GenerateAndDiscard(document);
        }
        
        #region Genearate And Show Configuration

        private static readonly Random Random = new();
        private const string GenerateAndShowNamePrefix = "QuestPDF Preview";

        private static void ClearGenerateAndShowFiles()
        {
            var legacyPreviewFiles = Directory
                .GetFiles(TemporaryStorage.GetPath(), $"{GenerateAndShowNamePrefix} *")
                .Where(x => DateTime.UtcNow - new FileInfo(x).LastAccessTimeUtc > TimeSpan.FromHours(1));
            
            foreach (var legacyPreviewFile in legacyPreviewFiles)
            {
                try
                {
                    if (File.Exists(legacyPreviewFile))
                        File.Delete(legacyPreviewFile);
                }
                catch
                {
                    // ignored
                }
            }
        }

        #endregion
        
        #region PDF
        
        /// <summary>
        /// Generates the document in PDF format and returns it as a byte array.
        /// </summary>
        public static byte[] GeneratePdf(this IDocument document)
        {
            using var memoryStream = new MemoryStream();
            document.GeneratePdf(memoryStream);
            return memoryStream.ToArray();
        }
        
        /// <summary>
        /// Generates the document in PDF format and saves it to the specified file path.
        /// </summary>
        public static void GeneratePdf(this IDocument document, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            using var fileStream = File.Create(filePath);
            document.GeneratePdf(fileStream);
        }

        /// <summary>
        /// Generates the document in PDF format and outputs it to a provided stream.
        /// </summary>
        public static void GeneratePdf(this IDocument document, Stream stream)
        {
            using var skiaStream = new SkWriteStream(stream);
            DocumentGenerator.GeneratePdf(skiaStream, document);
            skiaStream.Flush();
        }
        
        /// <summary>
        /// Generates the document in PDF format, saves it in temporary file, and then opens it with the default application.
        /// </summary>
        public static void GeneratePdfAndShow(this IDocument document)
        {
            var filePath = Path.Combine(TemporaryStorage.GetPath(), $"{GenerateAndShowNamePrefix} {Random.Next()}.pdf");
            
            document.GeneratePdf(filePath);
            Helpers.Helpers.OpenFileUsingDefaultProgram(filePath);
        }

        #endregion

        #region XPS
        
        /// <summary>
        /// Generates the document in XPS format and returns it as a byte array.
        /// </summary>
        /// <remarks>
        /// Supported only on the Windows platform.
        /// </remarks>
        public static byte[] GenerateXps(this IDocument document)
        {
            using var memoryStream = new MemoryStream();
            document.GenerateXps(memoryStream);
            return memoryStream.ToArray();
        }
        
        /// <summary>
        /// Generates the document in XPS format and saves it to the specified file path.
        /// </summary>
        /// <remarks>
        /// Supported only on the Windows platform.
        /// </remarks>
        public static void GenerateXps(this IDocument document, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            using var fileStream = File.Create(filePath);
            document.GenerateXps(fileStream);
        }

        /// <summary>
        /// Generates the document in XPS format and outputs it to a provided stream.
        /// </summary>
        /// <remarks>
        /// Supported only on the Windows platform.
        /// </remarks>
        public static void GenerateXps(this IDocument document, Stream stream)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException("XPS generation is only supported on the Windows platform.");
            
            using var skiaStream = new SkWriteStream(stream);
            DocumentGenerator.GenerateXps(skiaStream, document);
            skiaStream.Flush();
        }
        
        /// <summary>
        /// Generates the document in XPS format, saves it in temporary file, and then opens it with the default application.
        /// </summary>
        /// <remarks>
        /// Supported only on the Windows platform.
        /// </remarks>
        public static void GenerateXpsAndShow(this IDocument document)
        {
            var filePath = Path.Combine(TemporaryStorage.GetPath(), $"{GenerateAndShowNamePrefix} {Random.Next()}.xps");
            document.GenerateXps(filePath);
            Helpers.Helpers.OpenFileUsingDefaultProgram(filePath);
        }
        
        #endregion

        #region Images

        /// <summary>
        /// Generates the document as a series of images and returns them as a collection of byte arrays.
        /// </summary>
        /// <param name="settings">Optional settings to customize the generation process, such as image resolution, compression ratio, and more.</param>
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
        
        #region SVG

        /// <summary>
        /// Generates the document as a series of SVG images and returns them as a collection of strings.
        /// </summary>
        public static ICollection<string> GenerateSvg(this IDocument document)
        {
            return DocumentGenerator.GenerateSvg(document);
        }

        #endregion
    }
}