namespace QuestPDF.Infrastructure
{
    public class DocumentSettings
    {
        public const int DefaultRasterDpi = 72;
        
        /// <summary>
        /// Gets or sets a value indicating whether or not make the document PDF/A-2b conformant.
        /// If true, include XMP metadata, a document UUID, and sRGB output intent information.
        /// This adds length to the document and makes it non-reproducable, but are necessary features for PDF/A-2b conformance.
        /// </summary>
        public bool PdfA { get; set; } = false;

        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// The value 101 corresponds to lossless encoding.
        /// If this value is set to a value between 1 and 100, and the image is opaque, it will be encoded using the JPEG format with that quality setting.
        /// The default value is 90 (very high quality).
        /// </summary>
        public ImageCompressionQuality ImageCompressionQuality { get; set; } = ImageCompressionQuality.VeryHigh;
        
        // TODO: add comments
        public ImageScalingStrategy ImageScalingStrategy { get; set; } = ImageScalingStrategy.ScaleOnlyToSignificantlySmallerResolution;
        
        public ImageScalingQuality ImageScalingQuality { get; set; } = ImageScalingQuality.High;
        
        /// <summary>
        /// The DPI (pixels-per-inch) at which images and features without native PDF support will be rasterized.
        /// A larger DPI would create a PDF that reflects the original intent with better fidelity, but it can make for larger PDF files too, which would use more memory while rendering, and it would be slower to be processed or sent online or to printer.
        /// When generating images, this parameter also controls the resolution of the generated content.
        /// Default value is 72.
        /// </summary>
        public int RasterDpi { get; set; } = DefaultRasterDpi;
 
        public ContentDirection ContentDirection { get; set; } = ContentDirection.LeftToRight;
        
        public static DocumentSettings Default => new DocumentSettings();
    }
}