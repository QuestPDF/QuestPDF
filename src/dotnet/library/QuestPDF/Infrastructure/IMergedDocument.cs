using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestPDF.Infrastructure
{
    internal enum MergedDocumentPageNumberStrategy
    {
        Original,
        Continuous,
    }

    public sealed class MergedDocument : IDocument
    {
        internal IReadOnlyList<IDocument> Documents { get; }
        internal MergedDocumentPageNumberStrategy PageNumberStrategy { get; private set; } = MergedDocumentPageNumberStrategy.Original;
        
        internal DocumentMetadata Metadata { get; private set; } = DocumentMetadata.Default;
        internal DocumentSettings Settings { get; private set; } = DocumentSettings.Default;

        internal MergedDocument(IEnumerable<IDocument> documents)
        {
            Documents = documents?.ToList() ?? throw new NullReferenceException(nameof(documents));
        }

        public void Compose(IDocumentContainer container)
        {
            foreach (var document in Documents)
            {
                document.Compose(container);
            }
        }

        public DocumentMetadata GetMetadata()
        {
            return Metadata;
        }
        
        public DocumentSettings GetSettings()
        {
            return Settings;
        }

        /// <summary>
        /// Documents maintain their own page numbers upon merging, without continuity between them.
        /// As a result, APIs related to page numbers reflect individual documents, not the cumulative count.
        /// All documents are simply be merged together.
        /// </summary>
        /// <example>
        /// Merging a two-page document with a three-page document results in a sequence: 1, 2, 1, 2, 3.
        /// </example>
        public MergedDocument UseOriginalPageNumbers()
        {
            PageNumberStrategy = MergedDocumentPageNumberStrategy.Original;
            return this;
        }

        /// <summary>
        /// Consolidates the content from every document, creating a continuous seamless one.
        /// Page number APIs return a consecutive numbering for this unified document.
        /// </summary>
        /// <example>
        /// Merging a two-page document with a three-page document results in a sequence: 1, 2, 3, 4, 5.
        /// </example>
        public MergedDocument UseContinuousPageNumbers()
        {
            PageNumberStrategy = MergedDocumentPageNumberStrategy.Continuous;
            return this;
        }

        public MergedDocument WithMetadata(DocumentMetadata metadata)
        {
            Metadata = metadata ?? Metadata;
            return this;
        }

        public MergedDocument WithSettings(DocumentSettings settings)
        {
            Settings = settings ?? Settings;
            return this;
        }
    }
}