using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class Image : Element, ICacheable
    {
        public Infrastructure.Image? DocumentImage { get; set; }

        ~Image()
        {
            if (DocumentImage is { IsDocumentScoped: true })
                DocumentImage?.Dispose();
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (DocumentImage == null)
                return;

            var image = DocumentImage.GetVersionOfSize(availableSpace);
            Canvas.DrawImage(image, Position.Zero, availableSpace);
        }
    }
}