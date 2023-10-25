using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    internal record GetImageVersionRequest
    {
        internal ImageSize Resolution { get; set; }
        internal ImageCompressionQuality CompressionQuality { get; set; }
    }
    
    /// <summary>
    /// <para>Caches the image in local memory for efficient reuse.</para>
    /// <para>Optimizes the generation process, especially:</para>
    /// <para>- For images repeated in a single document to enhance performance and reduce output file size (e.g., an image used as list bullet icon).</para>
    /// <para>- When an image appears on multiple document types for increased generation performance (e.g., a company logo).</para>
    /// </summary>
    /// <remarks>
    /// This class is thread safe.
    /// </remarks>
    public class Image
    {
        internal SKImage SkImage { get; }
        internal ImageSize Size { get; }

        internal LinkedList<(GetImageVersionRequest request, SKImage image)> ScaledImageCache { get; } = new();
 
        internal Image(SKImage image)
        {
            SkImage = image;
            Size = new ImageSize(image.Width, image.Height);
        }
        
        ~Image()
        {
            SkImage.Dispose();
            
            foreach (var cacheKey in ScaledImageCache)
                cacheKey.image.Dispose();
        }
        
        #region Scaling Image

        internal SKImage GetVersionOfSize(GetImageVersionRequest request)
        {
            foreach (var cacheKey in ScaledImageCache)
            {
                if (cacheKey.request == request)
                    return cacheKey.image;
            }

            var result = SkImage.ResizeAndCompressImage(request.Resolution, request.CompressionQuality);
            ScaledImageCache.AddLast((request, result));
            return result;
        }

        #endregion

        #region public constructors

        private const string CannotDecodeExceptionMessage = "Cannot decode the provided image.";
        
        internal static Image FromSkImage(SKImage image)
        {
            return new Image(image);
        }

        /// <summary>
        /// Loads the image from binary data.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        public static Image FromBinaryData(byte[] imageData)
        {
            var image = SKImage.FromEncodedData(imageData);

            switch (image)
            {
                case null:
                    throw new DocumentComposeException(CannotDecodeExceptionMessage);
                default:
                    return new Image(image);
            }
        }

        /// <summary>
        /// Loads the image from a file with specified path.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        public static Image FromFile(string filePath)
        {
            var image = SKImage.FromEncodedData(filePath);

            switch (image)
            {
                case null:
                    throw File.Exists(filePath)
                                    ? new DocumentComposeException(CannotDecodeExceptionMessage)
                                    : new DocumentComposeException($"Cannot load provided image, file not found: ${filePath}");
                default:
                    return new Image(image);
            }
        }

        /// <summary>
        /// Loads the image from a stream.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        public static Image FromStream(Stream fileStream)
        {
            var image = SKImage.FromEncodedData(fileStream);

            return image switch
            {
                null => throw new DocumentComposeException(CannotDecodeExceptionMessage),
                _ => new Image(image),
            };
        }

        #endregion
    }
}