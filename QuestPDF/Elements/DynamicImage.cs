using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class DynamicImage : Element
    {
        public Func<Size, byte[]>? Source { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.Width < 0 || availableSpace.Height < 0)
                return new Wrap();
            
            return new FullRender(availableSpace.Width, availableSpace.Height);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var imageElement = new Image()
            {
                Data = Source?.Invoke(availableSpace)
            };
            
            imageElement.Draw(canvas, availableSpace);
        }
    }
}