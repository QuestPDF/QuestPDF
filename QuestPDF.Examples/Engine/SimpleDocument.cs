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
        
        private IContainer Container { get; }
        private Size Size { get; }

        public SimpleDocument(IContainer container, Size size)
        {
            Container = container;
            Size = size;
        }
        
        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata()
            {
                RasterDpi = PageSizes.PointsPerInch * ImageScalingFactor,
                DocumentLayoutExceptionThreshold = 10
            };
        }
        
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(new PageSize(Size.Width, Size.Height));
                page.Content().Container().Element(Container as Container);
            });
        }
    }
}