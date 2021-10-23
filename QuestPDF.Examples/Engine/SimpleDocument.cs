using System;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public class SimpleDocument : IDocument
    {
        public const int ImageScalingFactor = 2;
        
        private Action<IDocumentContainer> Content { get; }
        private int MaxPages { get; }

        public SimpleDocument(Action<IDocumentContainer> content, int maxPages)
        {
            Content = content;
            MaxPages = maxPages;
        }
        
        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata()
            {
                RasterDpi = PageSizes.PointsPerInch * ImageScalingFactor,
                DocumentLayoutExceptionThreshold = MaxPages
            };
        }
        
        public void Compose(IDocumentContainer container)
        {
            Content(container);
        }
    }
}