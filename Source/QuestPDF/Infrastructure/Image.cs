using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Skia;

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
    public sealed class Image : IDisposable
    {
        static Image()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }

        internal bool IsShared { get; set; } = true;
        internal SkImage SkImage { get; }
        internal ImageSize Size { get; }

        internal LinkedList<(GetImageVersionRequest request, SkImage image)> ScaledImageCache { get; } = new();
 
        internal Image(SkImage image)
        {
            SkImage = image;
            Size = new ImageSize(image.Width, image.Height);
        }
        
        ~Image()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            SkImage?.Dispose();
            
            foreach (var cacheKey in ScaledImageCache)
                cacheKey.image?.Dispose();
            
            GC.SuppressFinalize(this);
        }

        #region Scaling Image

        internal SkImage GetVersionOfSize(GetImageVersionRequest request)
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

        /// <summary>
        /// Loads the image from binary data.
        /// <a href="https://www.questpdf.com/api-reference/image/basics.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        public static Image FromBinaryData(byte[] imageBytes)
        {
            using var imageData = SkData.FromBinary(imageBytes);
            return StaticImageCache.DecodeImage(imageData, isShared: true);
        }

        /// <summary>
        /// Loads the image from a file with specified path.
        /// <a href="https://www.questpdf.com/api-reference/image/basics.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        public static Image FromFile(string filePath)
        {
            return StaticImageCache.DirectlyLoadFromFile(filePath, isShared: true);
        }

        /// <summary>
        /// Loads the image from a stream.
        /// <a href="https://www.questpdf.com/api-reference/image/basics.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        public static Image FromStream(Stream stream)
        {
            using var imageData = SkData.FromStream(stream);
            return StaticImageCache.DecodeImage(imageData, isShared: true);
        }

        #endregion
    }
}