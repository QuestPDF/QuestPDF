using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using IContainer = System.ComponentModel.IContainer;

namespace QuestPDF.Drawing
{
    internal class ElementProxy : ContainerElement
    {
        
    }
    
    internal class CacheProxy : ElementProxy
    {
        public Size? AvailableSpace { get; set; }
        public ISpacePlan? MeasurementResult { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (MeasurementResult != null &&
                AvailableSpace != null &&
                IsClose(AvailableSpace.Width, availableSpace.Width) &&
                IsClose(AvailableSpace.Height, availableSpace.Height))
            {
                return MeasurementResult;
            }

            AvailableSpace = availableSpace;
            MeasurementResult = Child?.Measure(availableSpace) ?? new FullRender(Size.Zero);

            return MeasurementResult;
        }

        internal override void Draw(Size availableSpace)
        {
            AvailableSpace = null;
            MeasurementResult = null;
            
            base.Draw(availableSpace);
        }

        private bool IsClose(float x, float y)
        {
            return Math.Abs(x - y) < Size.Epsilon;
        }
    }

    internal class DebugProxy : ElementProxy
    {
        
    }
}