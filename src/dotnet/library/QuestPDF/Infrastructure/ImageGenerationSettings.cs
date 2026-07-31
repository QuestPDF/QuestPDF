namespace QuestPDF.Infrastructure
{
    public sealed class ImageGenerationSettings
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
        /// The default value is 288.
        /// </summary>
        /// <example>
        /// Consider a document of dimensions 3x4 inches. Using a DPI value of 300, the final image resolution translates to 900x1200 pixels.
        /// </example>
        public int RasterDpi { get; set; } = DocumentSettings.DefaultRasterDpi * 4;

        /// <summary>
        /// When enabled, the generated images have a transparent background instead of a white one.
        /// Applies only to image formats that support transparency (PNG and WEBP); JPEG images are always generated with a white background.
        /// The default value is false, so that the output resembles the document as displayed in a PDF viewer.
        /// </summary>
        public bool UseTransparentBackground { get; set; } = false;

        public static ImageGenerationSettings Default => new ImageGenerationSettings();
    }
}