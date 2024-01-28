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
        /// Gets or sets a value indicating whether the generated document should be additionally compressed. May greatly reduce file size with a small increase in generation time.
        /// </summary>
        public bool CompressDocument { get; set; } = true;
        
        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// When the image is opaque, it will be encoded using the JPEG format with the selected quality setting.
        /// When the image contains an alpha channel, it is always encoded using the PNG format and this option is ignored.
        /// The default value is "high quality".
        /// </summary>

        /// <remarks>
        /// This setting is taken into account only when the image is in the JPG format, otherwise it is ignored.
        /// </remarks>
        public ImageCompressionQuality ImageCompressionQuality { get; set; } = ImageCompressionQuality.High;

        /// <summary>
        /// The DPI (pixels-per-inch) at which images and features without native PDF support will be rasterized.
        /// A larger DPI would create a PDF that reflects the original intent with better fidelity, but it can make for larger PDF files too, which would use more memory while rendering, and it would be slower to be processed or sent online or to printer.
        /// When generating images, this parameter also controls the resolution of the generated content.
        /// Default value is 288.
        /// </summary>
        public int ImageRasterDpi { get; set; } = DefaultRasterDpi * 4;
 
        public ContentDirection ContentDirection { get; set; } = ContentDirection.LeftToRight;
        
        public static DocumentSettings Default => new DocumentSettings();
    }
}