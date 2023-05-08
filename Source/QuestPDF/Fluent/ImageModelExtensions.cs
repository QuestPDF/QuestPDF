using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ImageModelExtensions
    {
        /// <summary>
        /// When enabled, the image object is disposed automatically after document generation, and you don't need to call the Dispose method yourself.
        /// </summary>
        public static Image DisposeAfterDocumentGeneration(this Image image)
        {
            image.IsDocumentScoped = true;
            return image;
        }

        /// <summary>
        /// When enabled, the library will not attempt to resize the image to fit the target DPI, nor save it with target image quality.
        /// </summary>
        public static Image UseOriginalImage(this Image image)
        {
            image.UseOriginalImage = true;
            return image;
        }
        
        /// <summary>
        /// The DPI (pixels-per-inch) at which images and features without native PDF support will be rasterized.
        /// A larger DPI would create a PDF that reflects the original intent with better fidelity, but it can make for larger PDF files too, which would use more memory while rendering, and it would be slower to be processed or sent online or to printer.
        /// When generating images, this parameter also controls the resolution of the generated content.
        /// Default value is 72.
        /// </summary>
        public static Image WithRasterDpi(this Image image, int dpi)
        {
            image.TargetDpi = dpi;
            return image;
        }

        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// The value 101 corresponds to lossless encoding.
        /// If this value is set to a value between 1 and 100, and the image is opaque, it will be encoded using the JPEG format with that quality setting.
        /// The default value is 90 (very high quality).
        /// </summary>
        public static Image WithCompressionQuality(this Image image, ImageCompressionQuality quality)
        {
            image.CompressionQuality = quality;
            return image;
        }
        
        public static Image WithScalingQuality(this Image image, ImageScalingQuality strategy)
        {
            image.ScalingQuality = strategy;
            return image;
        }
        
        public static Image WithScalingStrategy(this Image image, ImageScalingStrategy strategy)
        {
            image.ScalingStrategy = strategy;
            return image;
        }
    }
}