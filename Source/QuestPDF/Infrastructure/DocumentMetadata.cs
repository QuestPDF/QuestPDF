using System;

namespace QuestPDF.Infrastructure
{
    public class DocumentMetadata
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Subject { get; set; }
        public string? Keywords { get; set; }
        public string? Creator { get; set; }
        public string? Producer { get; set; }

        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.Now;

        public static DocumentMetadata Default => new DocumentMetadata();
        
        #region Deprecated properties
        
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
        
        [Obsolete("This API has been moved since version 2023.5. Please use the QuestPDF.Infrastructure.DocumentSettings API.")]
        public int? ImageQuality { get; set; }
        
        [Obsolete("This API has been moved since version 2023.5. Please use the QuestPDF.Infrastructure.DocumentSettings API.")]
        public int? RasterDpi { get; set; }
        
        [Obsolete("This API has been moved since version 2023.5. Please use the QuestPDF.Infrastructure.DocumentSettings API.")]
        public bool? PdfA { get; set; }
        
        #endregion
    }
}