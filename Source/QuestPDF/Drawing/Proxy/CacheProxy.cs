using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy
{
    internal sealed class CacheProxy : ElementProxy
    {
        public Size? AvailableSpace { get; set; }
        public SpacePlan? MeasurementResult { get; set; }

        public CacheProxy(Element child)
        {
            Child = child;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (MeasurementResult != null &&
                AvailableSpace != null &&
                IsClose(AvailableSpace.Value.Width, availableSpace.Width) &&
                IsClose(AvailableSpace.Value.Height, availableSpace.Height))
            {
                return MeasurementResult.Value;
            }

            AvailableSpace = availableSpace;
            MeasurementResult = base.Measure(availableSpace);

            return MeasurementResult.Value;
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
}