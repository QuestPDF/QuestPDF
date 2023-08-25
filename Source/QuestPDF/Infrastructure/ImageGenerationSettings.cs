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
        /// The default value is "high".
        /// </summary>
        public ImageCompressionQuality ImageCompressionQuality { get; set; } = ImageCompressionQuality.High;

        /// <summary>
        /// The DPI (pixels-per-inch) at which the document will be rasterized. This parameter controls the resolution of produced images.
        /// Higher DPI results in superior image quality but may increase the output file size.
        /// Default value is 288.
        /// </summary>
        /// <example>
        /// Consider a document of dimensions 3x4 inches. Using a DPI value of 300, the final image resolution translates to 900x1200 pixels.
        /// </example>
        public int RasterDpi { get; set; } = DocumentSettings.DefaultRasterDpi * 4;


        public static ImageGenerationSettings Default => new ImageGenerationSettings();
    }
}