using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class Image : IDisposable
    {
        internal SKImage SkImage { get; }
        
        internal int? TargetDpi { get; set; }
        internal int? TargetQuality { get; set; }
        internal bool IsDocumentScoped { get; set; }

        public int Width => SkImage.Width;
        public int Height => SkImage.Height;

        private Image(SKImage image)
        {
            SkImage = image;
        }

        public void Dispose()
        {
            SkImage.Dispose();
        }

        #region Fluent API
        
        /// <summary>
        /// When enabled, the image object is disposed automatically after document generation, and you don't need to call the Dispose method yourself.
        /// </summary>
        public Image DisposeAfterDocumentGeneration()
        {
            IsDocumentScoped = true;
            return this;
        }

        /// <summary>
        /// The DPI (pixels-per-inch) at which images and features without native PDF support will be rasterized.
        /// A larger DPI would create a PDF that reflects the original intent with better fidelity, but it can make for larger PDF files too, which would use more memory while rendering, and it would be slower to be processed or sent online or to printer.
        /// When generating images, this parameter also controls the resolution of the generated content.
        /// Default value is 72.
        /// </summary>
        public Image WithRasterDpi(int dpi)
        {
            TargetDpi = dpi;
            return this;
        }

        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// The value 101 corresponds to lossless encoding.
        /// If this value is set to a value between 1 and 100, and the image is opaque, it will be encoded using the JPEG format with that quality setting.
        /// The default value is 90 (very high quality).
        /// </summary>
        public Image WithQuality(int value)
        {
            if (value is not (>= 1 and <= 101))
                throw new DocumentComposeException("Image quality must be between 1 and 101.");
            
            TargetQuality = value;
            return this;
        }

        #endregion
        
        #region public constructors

        internal static Image FromSkImage(SKImage image)
        {
            return CreateImage(image);
        }

        public static Image FromBinaryData(byte[] imageData)
        {
            return CreateImage(SKImage.FromEncodedData(imageData));
        }

        public static Image FromFile(string filePath)
        {
            return CreateImage(SKImage.FromEncodedData(filePath));
        }

        public static Image FromStream(Stream fileStream)
        {
            return CreateImage(SKImage.FromEncodedData(fileStream));
        }

        private static Image CreateImage(SKImage? image)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");

            return new Image(image);
        }

        #endregion
    }
}