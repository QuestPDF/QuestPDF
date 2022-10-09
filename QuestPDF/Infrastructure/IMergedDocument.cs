using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    public interface IMergedDocument : IDocument
    {
        /// <summary>
        /// Each document should be treated as a 'single' one in terms of page numbering.
        /// Page numbers will be reset after each document is generated.
        /// E.g. two documents with a page count of 2 and 3 will result in the following page numbers: 1,2,1,2,3.
        /// </summary>
        IMergedDocument SeperatePageNumbers();

        /// <summary>
        /// All documents should be treated as a 'single' document in terms of page numbering.
        /// Page numbers will continue to increase throughout the document generation.
        /// E.g. two documents with a page count of 2 and 3 will result in the following page numbers: 1,2,3,4,5.
        /// </summary>
        IMergedDocument ContinuousPageNumbers();

        IMergedDocument WithMetadata(DocumentMetadata metadata);
    }

    internal enum MergedDocumentPageNumberHandling
    {
        Seperate,
        Continuous,
    }

    internal sealed class MergedDocument : IMergedDocument, IDocument
    {
        public IReadOnlyList<IDocument> Documents { get; }

        public MergedDocumentPageNumberHandling PageNumberHandling { get; private set; } = MergedDocumentPageNumberHandling.Seperate;
        public DocumentMetadata Metadata { get; private set; } = DocumentMetadata.Default;

        public MergedDocument(IEnumerable<IDocument> documents)
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

        public IMergedDocument SeperatePageNumbers()
        {
            PageNumberHandling = MergedDocumentPageNumberHandling.Seperate;
            return this;
        }

        public IMergedDocument ContinuousPageNumbers()
        {
            PageNumberHandling = MergedDocumentPageNumberHandling.Continuous;
            return this;
        }

        public IMergedDocument WithMetadata(DocumentMetadata metadata)
        {
            Metadata = metadata ?? Metadata;
            return this;
        }
    }
}
