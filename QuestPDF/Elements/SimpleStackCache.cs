using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SimpleStackWithCache : SimpleStack
    {
        private ISpacePlan? Cache { get; set; }
        private Size? CacheForSize { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (Cache != null && CacheForSize != null && CacheForSize.Equals(availableSpace))
                return Cache;

            CacheForSize = availableSpace;
            Cache = base.Measure(availableSpace);
            return Cache;
        }
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            base.Draw(canvas, availableSpace);
            Cache = null;
            CacheForSize = null;
        }
    }
}