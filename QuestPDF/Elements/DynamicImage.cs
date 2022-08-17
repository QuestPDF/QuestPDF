using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class DynamicImage : Element
    {
        public Func<Size, byte[]>? Source { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            var imageData = Source?.Invoke(availableSpace);
            
            if (imageData == null)
                return;

            var imageElement = new Image
            {
                InternalImage = SKImage.FromEncodedData(imageData)
            };
            
            imageElement.Initialize(PageContext, Canvas);
            imageElement.Draw(availableSpace);
        }
    }
}