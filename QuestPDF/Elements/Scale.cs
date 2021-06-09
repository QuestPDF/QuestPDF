using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Scale : ContainerElement
    {
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));
            
            var measure = base.Measure(targetSpace) as Size;

            if (measure == null)
                return new Wrap();

            var targetSize = new Size(
                Math.Abs(measure.Width * ScaleX), 
                Math.Abs(measure.Height * ScaleY));

            if (measure is PartialRender)
                return new PartialRender(targetSize);
            
            if (measure is FullRender)
                return new FullRender(targetSize);
            
            throw new ArgumentException();
        }
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var skiaCanvas = (canvas as Drawing.Canvas)?.SkiaCanvas;
            
            if (skiaCanvas == null)
                return;
            
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));

            var currentMatrix = skiaCanvas.TotalMatrix;
            
            if (ScaleX < 0)
                skiaCanvas.Translate(availableSpace.Width, 0);
            
            if (ScaleY < 0)
                skiaCanvas.Translate(0, availableSpace.Height);
            
            skiaCanvas.Scale(ScaleX, ScaleY);
            Child?.Draw(canvas, targetSpace);
            skiaCanvas.SetMatrix(currentMatrix);
        }
    }
}