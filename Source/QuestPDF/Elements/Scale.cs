using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Scale : ContainerElement, ICacheable
    {
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));
            
            var measure = base.Measure(targetSpace);

            if (measure.Type is SpacePlanType.NoContent or SpacePlanType.Wrap)
                return measure;

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
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));

            var translate = new Position(
                ScaleX < 0 ? availableSpace.Width : 0,
                ScaleY < 0 ? availableSpace.Height : 0);
            
            Canvas.Translate(translate);
            Canvas.Scale(ScaleX, ScaleY);
            
            Child?.Draw(targetSpace);
             
            Canvas.Scale(1/ScaleX, 1/ScaleY);
            Canvas.Translate(translate.Reverse());
        }
    }
}