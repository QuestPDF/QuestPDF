using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class DynamicImage : Element, IVisual
    {
        public bool IsRendered { get; set; }
        public bool RepeatContent { get; set; }
        
        public Func<Size, byte[]>? Source { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();
            
            if (IsRendered && !RepeatContent)
                return SpacePlan.FullRender(Size.Zero);
            
            return SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            IsRendered = true;
            
            if (availableSpace.Width < Size.Epsilon || availableSpace.Height < Size.Epsilon)
                return;
            
            var imageData = Source?.Invoke(availableSpace);
            
            if (imageData == null)
                return;

            using var image = SKImage.FromEncodedData(imageData);
            Canvas.DrawImage(image, Position.Zero, availableSpace);
        }
    }
}