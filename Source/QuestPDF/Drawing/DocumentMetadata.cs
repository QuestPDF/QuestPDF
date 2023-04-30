using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    public class DocumentMetadata
    {
        public const int DefaultPdfDpi = 72;
        
        public int ImageQuality { get; set; } = 101;
        public int RasterDpi { get; set; } = DefaultPdfDpi;
        public bool PdfA { get; set; }
        
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Subject { get; set; }
        public string? Keywords { get; set; }
        public string? Creator { get; set; }
        public string? Producer { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        [Obsolete("This API has been moved since version 2022.9. Please use the QuestPDF.Settings.DocumentLayoutExceptionThreshold static property.")]
        public int DocumentLayoutExceptionThreshold
        {
            get => Settings.DocumentLayoutExceptionThreshold;
            set => Settings.DocumentLayoutExceptionThreshold = value;
        }

        [Obsolete("This API has been moved since version 2022.9. Please use the QuestPDF.Settings.EnableCaching static property.")]
        public bool ApplyCaching
        {
            get => Settings.EnableCaching;
            set => Settings.EnableCaching = value;
        }
        
        [Obsolete("This API has been moved since version 2022.9. Please use the QuestPDF.Settings.EnableDebugging static property.")]
        public bool ApplyDebugging
        {
            get => Settings.EnableDebugging;
            set => Settings.EnableDebugging = value;
        }

        public static DocumentMetadata Default => new DocumentMetadata();
    }
}