using System;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Rotate : ContainerElement
    {
        public int TurnCount { get; set; }
        public int NormalizedTurnCount => (TurnCount % 4 + 4) % 4;
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (NormalizedTurnCount == 0 || NormalizedTurnCount == 2)
                return base.Measure(availableSpace);
            
            availableSpace = new Size(availableSpace.Height, availableSpace.Width);
            var childSpace = base.Measure(availableSpace) as Size;

            if (childSpace == null)
                return new Wrap();

            var targetSpace = new Size(childSpace.Height, childSpace.Width);

            if (childSpace is FullRender)
                return new FullRender(targetSpace);
            
            if (childSpace is PartialRender)
                return new PartialRender(targetSpace);

            throw new ArgumentException();
        }
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var skiaCanvas = (canvas as Drawing.Canvas)?.SkiaCanvas;
            
            if (skiaCanvas == null)
                return;

            var currentMatrix = skiaCanvas.TotalMatrix;

            if (NormalizedTurnCount % 4 == 1 || NormalizedTurnCount % 4 == 2)
                skiaCanvas.Translate(availableSpace.Width, 0);
            
            if (NormalizedTurnCount % 4 == 2  || NormalizedTurnCount % 4 == 3)
                skiaCanvas.Translate(0, availableSpace.Height);
            
            skiaCanvas.RotateRadians(TurnCount * (float) Math.PI / 2f);
            Child?.Draw(canvas, availableSpace);
            skiaCanvas.SetMatrix(currentMatrix);
        }
    }
}