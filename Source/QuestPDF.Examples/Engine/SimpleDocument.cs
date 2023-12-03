using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public class SimpleDocument : IDocument
    {
        private Action<IDocumentContainer> Content { get; }
        private int MaxPages { get; }

        public SimpleDocument(Action<IDocumentContainer> content, int maxPages, bool applyCaching, bool applyDebugging)
        {
            Content = content;
            MaxPages = maxPages;

            QuestPDF.Settings.EnableCaching = applyCaching;
            QuestPDF.Settings.EnableDebugging = applyDebugging;
            QuestPDF.Settings.DocumentLayoutExceptionThreshold = MaxPages;
        }

        public void Compose(IDocumentContainer container)
        {
            Content(container);
        }
    }
}