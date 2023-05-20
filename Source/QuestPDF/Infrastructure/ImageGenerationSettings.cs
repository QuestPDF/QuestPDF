using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class ImageGenerationSettings
    {
        /// <summary>
        /// The file format used to encode the image(s).
        /// </summary>
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;

        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// The default value is "high quality".
        /// </summary>
        public ImageCompressionQuality ImageCompressionQuality { get; set; } = ImageCompressionQuality.VeryHigh;

        /// <summary>
        /// The DPI (pixels-per-inch) at which the document will be rasterized. This parameter controls the resolution of produced images.
        /// Default value is 144.
        /// </summary>
        public int RasterDpi { get; set; } = DocumentSettings.DefaultRasterDpi * 2;


        public static ImageGenerationSettings Default => new ImageGenerationSettings();
    }
}