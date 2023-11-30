using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class MultiColumn : ContainerElement
{
    internal override SpacePlan Measure(Size availableSpace)
    {
        var columnChild = Child as Column;
        
        Child.InjectDependencies(PageContext, new FreeCanvas());
        var originalState = columnChild.State;
        
        var leftSize = columnChild.Measure(availableSpace);
        columnChild.Draw(availableSpace);
        var rightSize = columnChild.Measure(availableSpace);

        Child.InjectDependencies(PageContext, Canvas);
        columnChild.State = originalState;

        if (leftSize.Type == SpacePlanType.FullRender || rightSize.Type == SpacePlanType.FullRender)
            return SpacePlan.FullRender(availableSpace);
        
        return SpacePlan.PartialRender(availableSpace);
    }
        
    internal override void Draw(Size availableSpace)
    {
        var childSpace = new Size(availableSpace.Width / 2, availableSpace.Height);
        var rightOffset = new Position(availableSpace.Width / 2, 0);
        
        Child.Draw(childSpace);
        Canvas.Translate(rightOffset);
        Child.Draw(childSpace);
        Canvas.Translate(rightOffset.Reverse());
    }
}