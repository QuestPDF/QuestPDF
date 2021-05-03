using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class DynamicImage : Element
    {
        public Func<Size, byte[]>? Source { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(availableSpace.Width, availableSpace.Height);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var imageData = Source?.Invoke(availableSpace);
            
            if (imageData == null)
                return;

            var imageElement = new Image
            {
                InternalImage = SKImage.FromEncodedData(imageData)
            };
            
            imageElement.Draw(canvas, availableSpace);
        }
    }
}