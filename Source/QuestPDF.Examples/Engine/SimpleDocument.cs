using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public class SimpleDocument : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;
        
        private Action<IDocumentContainer> Content { get; }

        public SimpleDocument(Action<IDocumentContainer> content, bool applyCaching, bool applyDebugging)
        {
            Content = content;

            QuestPDF.Settings.EnableCaching = applyCaching;
            QuestPDF.Settings.EnableDebugging = applyDebugging;
        }

        public void Compose(IDocumentContainer container)
        {
            Content(container);
        }
    }
}