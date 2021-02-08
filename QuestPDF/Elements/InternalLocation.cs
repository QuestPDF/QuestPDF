using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InternalLocation : ContainerElement
    {
        public string LocationName { get; set; }
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            canvas.DrawLocation(LocationName);
            Child?.Draw(canvas, availableSpace);
        }
    }
}