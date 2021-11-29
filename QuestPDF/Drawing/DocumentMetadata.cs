using System;

namespace QuestPDF.Drawing
{
    public class DocumentMetadata
    {
        public int ImageQuality { get; set; } = 101;
        public int RasterDpi { get; set; } = 72;
        public bool PdfA { get; set; }
        
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Subject { get; set; }
        public string? Keywords { get; set; }
        public string? Creator { get; set; }
        public string? Producer { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// If the number of generated pages exceeds this threshold
        /// (likely due to infinite layout), the exception is thrown.
        /// </summary>
        public int DocumentLayoutExceptionThreshold { get; set; } = 250;

        public bool ApplyCaching { get; set; } = !System.Diagnostics.Debugger.IsAttached;
        public bool ApplyDebugging { get; set; } = System.Diagnostics.Debugger.IsAttached;

        public static DocumentMetadata Default => new DocumentMetadata();
    }
}