using System.Collections;
using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer.Inspection;

namespace QuestPDF.Drawing.Proxy
{
    internal class InspectionProxy : ElementProxy
    {
        
        public Dictionary<int, InspectionStateItem> Statistics { get; set; } = new();

        public InspectionProxy(Element child)
        {
            Child = child;
        }

        internal override void Draw(Size availableSpace)
        {
            if (Canvas is SkiaCanvasBase canvas)
            {
                var matrix = canvas.Canvas.TotalMatrix;

                var inspectionItem = new InspectionStateItem
                {
                    Element = Child,
                    Position = new Position(matrix.TransX, matrix.TransY),
                    Size = availableSpace
                };

                Statistics[PageContext.CurrentPage] = inspectionItem;
            }
            
            base.Draw(availableSpace);
        }
    }
}