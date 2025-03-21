using System;

namespace QuestPDF.Infrastructure
{
    public sealed class DocumentMetadata
    {
        /// <summary>
        /// Represents the main heading or name of the document, often displayed as a prominent identifier or label in PDF metadata.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Specifies the individual or entity responsible for creating the document.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Provides a brief description or main topic related to the document content.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Defines a collection of terms or phrases that describe the document's content or purpose.
        /// Improves categorization and searchability of the document.
        /// </summary>
        public string? Keywords { get; set; }

        /// <summary>
        /// Identifies the software or system that generated the document.
        /// </summary>
        public string? Creator { get; set; }

        /// <summary>
        /// Specifies the name of the application or library that generated the document.
        /// </summary>
        public string? Producer { get; set; }

        /// <summary>
        /// Specifies the language of the document content, defined using language tags such as "en-US" for American English.
        /// </summary>
        public string? Language { get; set; }
        
        /// <summary>
        /// Represents the date and time when the document was created.
        /// This property is used to specify the creation timestamp.
        /// </summary>
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Stores the most recent date and time when the content or metadata of the document was updated.
        /// It is used to provide metadata information about the last revision of the document.
        /// </summary>
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