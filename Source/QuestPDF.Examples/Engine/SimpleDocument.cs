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
        private bool ApplyCaching { get; }
        private bool ApplyDebugging { get; }

        public SimpleDocument(Action<IDocumentContainer> content, int maxPages, bool applyCaching, bool applyDebugging)
        {
            Content = content;
            MaxPages = maxPages;
            ApplyCaching = applyCaching;
            ApplyDebugging = applyDebugging;
        }
        
        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata()
            {
                RasterDpi = PageSizes.PointsPerInch * ImageScalingFactor,
                DocumentLayoutExceptionThreshold = MaxPages,
                ApplyCaching = ApplyCaching,
                ApplyDebugging = ApplyDebugging
            };
        }
        
        public void Compose(IDocumentContainer container)
        {
            Content(container);
        }
    }
}