using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public class SimpleDocument : IDocument
    {
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
                RasterDpi = PageSizes.PointsPerInch * 2,
                Size = Size,
                DocumentLayoutExceptionThreshold = 10
            };
        }

        public void Compose(IContainer container)
        {
            container.Background("#FFF").Element(Container.Child);
        }
    }
}