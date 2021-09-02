using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Scale : ContainerElement
    {
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));
            
            var measure = base.Measure(targetSpace);

            if (measure.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            var targetSize = new Size(
                Math.Abs(measure.Width * ScaleX), 
                Math.Abs(measure.Height * ScaleY));

            if (measure.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSize);
            
            if (measure.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(targetSize);
            
            throw new ArgumentException();
        }
        
        internal override void Draw(Size availableSpace)
        {
            var skiaCanvas = (Canvas as Drawing.SkiaCanvasBase)?.Canvas;
            
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
            base.Draw(targetSpace);
            skiaCanvas.SetMatrix(currentMatrix);
        }
    }
}