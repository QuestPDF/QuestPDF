using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer.LayoutInspection;

namespace QuestPDF.Drawing.Proxy;

internal sealed class LayoutDebuggingProxy : ElementProxy, IStateResettable
{
    public SpacePlanType? CurrentSpacePlanType { get; set; }
    public List<InspectionDrawOperation> DrawOperations { get; set; } = new();

    public LayoutDebuggingProxy(Element child)
    {
        Child = child;
    }

    public void ResetState()
    {
        CurrentSpacePlanType = null;
        DrawOperations.Clear();
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        var spacePlan = Child.Measure(availableSpace);
        CurrentSpacePlanType = spacePlan.Type;
        return spacePlan;
    }
    
    internal override void Draw(Size availableSpace)
    {
        if (Canvas is SkiaCanvasBase canvas)
        {
            var matrix = canvas.Canvas.TotalMatrix;

            var inspectionItem = new InspectionDrawOperation
            {
                PageNumber = PageContext.CurrentPage,
                Position = new Position(matrix.TransX, matrix.TransY),
                Size = availableSpace
            };

            DrawOperations.Add(inspectionItem);
        }
            
        base.Draw(availableSpace);
    }
}