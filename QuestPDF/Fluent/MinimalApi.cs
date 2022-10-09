using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class Document : IDocument
    {
        private Action<IDocumentContainer> ContentSource { get; }
        private DocumentMetadata Metadata { get; set; } = DocumentMetadata.Default;

        private Document(Action<IDocumentContainer> contentSource)
        {
            ContentSource = contentSource;
        }
        
        public static Document Create(Action<IDocumentContainer> handler)
        {
            return new Document(handler);
        }

        public static IMergedDocument Merge(IEnumerable<IDocument> documents)
        {
            return new MergedDocument(documents);
        }

        public static IMergedDocument Merge(params IDocument[] documents)
        {
            return new MergedDocument(documents);
        }

        public Document WithMetadata(DocumentMetadata metadata)
        {
            Metadata = metadata ?? Metadata;
            return this;
        }
        
        #region IDocument

        public DocumentMetadata GetMetadata() => Metadata;
        public void Compose(IDocumentContainer container) => ContentSource(container);

        #endregion
    }
}