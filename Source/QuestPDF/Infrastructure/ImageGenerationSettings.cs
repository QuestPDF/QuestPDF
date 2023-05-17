using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class ImageGenerationSettings
    {
        /// <summary>
        /// The file format used to encode the image(s).
        /// </summary>
        public SKEncodedImageFormat Format { get; set; } = SKEncodedImageFormat.Png;

        /// <summary>
        /// The quality level to use for the image(s). This is in the range from 0-100.
        /// </summary>
        public int Quality { get; set; } = 100;

        /// <summary>
        /// The DPI (pixels-per-inch) at which images and features without native PDF support will be rasterized.
        /// A larger DPI would create a PDF that reflects the original intent with better fidelity, but it can make for larger PDF files too, which would use more memory while rendering, and it would be slower to be processed or sent online or to printer.
        /// When generating images, this parameter also controls the resolution of the generated content.
        /// Default value is 144.
        /// </summary>
        public int RasterDpi { get; set; } = 144;


        public static ImageGenerationSettings Default => new ImageGenerationSettings();
    }
}