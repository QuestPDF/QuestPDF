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
        /// Each document is considered as separate in terms of page numbering.
        /// That means, all page number related APIs will return values based on original documents.
        /// All documents will simply be merged together.
        /// For example: let's suppose that two documents are merged, first with 2 pages and second with 3 pages.
        /// The resulting document will have 5 pages, and page numbers will be: 1, 2, 1, 2, 3.
        /// </summary>
        public MergedDocument UseOriginalPageNumbers()
        {
            PageNumberStrategy = MergedDocumentPageNumberStrategy.Original;
            return this;
        }

        /// <summary>
        /// Content from all documents will be merged together, and considered as one/single document.
        /// That means, all page number related APIs will return continuous numbers.
        /// For example: let's suppose that two documents are merged, first with 2 pages and second with 3 pages.
        /// The resulting document will have 5 pages, and page numbers will be: 1, 2, 3, 4, 5.
        /// </summary>
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