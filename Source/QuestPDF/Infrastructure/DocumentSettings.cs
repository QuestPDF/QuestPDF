using System;

namespace QuestPDF.Infrastructure
{
    public sealed class DocumentSettings
    {
        public const int DefaultRasterDpi = 72;

        [Obsolete("Please use the ConformanceLevel property instead.")]
        public bool PdfA
        {
            get => PDFA_Conformance != PDFA_Conformance.None;
            set => PDFA_Conformance = value ? PDFA_Conformance.PDFA_3B : PDFA_Conformance.None;
        }

        /// <summary>
        /// Gets or sets the PDF/A conformance level for the document.
        /// This property determines the adherence of the generated PDF to specific archival standards.
        /// </summary>
        public PDFA_Conformance PDFA_Conformance { get; set; } = PDFA_Conformance.None;

        /// <summary>
        /// Gets or sets the conformance level for PDF/UA (Universal Accessibility) compliance.
        /// Warning: this setting makes the document non-reproducable.
        /// </summary>
        public PDFUA_Conformance PDFUA_Conformance { get; set; } = PDFUA_Conformance.None;
        
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
    
    public enum PDFA_Conformance
    {
        None = 0,
        // PDFA_1A = 1,
        // PDFA_1B = 2,
        PDFA_2A = 3,
        PDFA_2B = 4,
        PDFA_2U = 5,
        PDFA_3A = 6,
        PDFA_3B = 7,
        PDFA_3U = 8
    }
    
    public enum PDFUA_Conformance
    {
        None = 0,
        PDFUA_1 = 1
    }
}