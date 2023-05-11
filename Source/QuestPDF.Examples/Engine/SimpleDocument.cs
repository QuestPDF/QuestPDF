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

        public SimpleDocument(Action<IDocumentContainer> content, int maxPages, bool applyCaching, bool applyDebugging)
        {
            Content = content;
            MaxPages = maxPages;

            QuestPDF.Settings.EnableCaching = applyCaching;
            QuestPDF.Settings.EnableDebugging = applyDebugging;
            QuestPDF.Settings.DocumentLayoutExceptionThreshold = MaxPages;
        }
        
        public DocumentSettings GetSettings()
        {
            return new DocumentSettings()
            {
                ImageRasterDpi = PageSizes.PointsPerInch * ImageScalingFactor
            };
        }
        
        public void Compose(IDocumentContainer container)
        {
            Content(container);
        }
    }
}