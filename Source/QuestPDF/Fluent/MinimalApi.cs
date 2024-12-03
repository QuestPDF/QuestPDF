using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Fluent
{
    public class Document : IDocument
    {
        static Document()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }
        
        private Action<IDocumentContainer> ContentSource { get; }
        private DocumentMetadata Metadata { get; set; } = DocumentMetadata.Default;
        private DocumentSettings Settings { get; set; } = DocumentSettings.Default;

        private Document(Action<IDocumentContainer> contentSource)
        {
            ContentSource = contentSource;
        }
        
        /// <summary>
        /// Creates a new empty document and provides handler to specify its content.
        /// </summary>
        /// <returns>A Document object with the specified content. This object allows to set metadata, configure generation parameters, and produce output files such as PDF, XPS, or images.</returns>
        public static Document Create(Action<IDocumentContainer> handler)
        {
            return new Document(handler);
        }

        /// <summary>
        /// Configures the metadata of the PDF document, such as title, author, keywords, etc.
        /// </summary>
        public Document WithMetadata(DocumentMetadata metadata)
        {
            Metadata = metadata ?? Metadata;
            return this;
        }
        
        /// <summary>
        /// Enables fine-tuning of the document generation process, influencing attributes of the resulting PDF such as target DPI, image compression, compliance with the PDF/A standard, etc.
        /// </summary>
        public Document WithSettings(DocumentSettings settings)
        {
            Settings = settings ?? Settings;
            return this;
        }
        
        /// <summary>
        /// Combines multiple documents together into a single one.
        /// </summary>
        /// <returns>A MergedDocument object that allows to set metadata, configure generation parameters, adjust merging strategy, and produce output files such as PDF, XPS, or images.</returns>
        public static MergedDocument Merge(IEnumerable<IDocument> documents)
        {
            return new MergedDocument(documents);
        }

        /// <summary>
        /// Combines multiple documents together into a single one.
        /// </summary>
        /// <returns>A MergedDocument object that allows to set metadata, configure generation parameters, adjust merging strategy, and produce output files such as PDF, XPS, or images.</returns>
        public static MergedDocument Merge(params IDocument[] documents)
        {
            return new MergedDocument(documents);
        }
        
        #region IDocument

        /// <summary>
        /// Implements the IDocument interface. Don't use within the Fluent API chain.
        /// </summary>
        public DocumentMetadata GetMetadata() => Metadata;
        
        /// <summary>
        /// Implements the IDocument interface. Don't use within the Fluent API chain.
        /// </summary>
        public DocumentSettings GetSettings() => Settings;
        
        /// <summary>
        /// Implements the IDocument interface. Don't use within the Fluent API chain.
        /// </summary>
        public void Compose(IDocumentContainer container) => ContentSource(container);

        #endregion
    }
}