using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class MultiColumn : ContainerElement
{
    internal override SpacePlan Measure(Size availableSpace)
    {
        var columnChild = Child as Column;
        
        Child.InjectDependencies(PageContext, new FreeCanvas()); // TODO: optimize to pass only canvas
        var originalState = columnChild.State; // TODO: traverse the Child's tree and extract state to the ICollection(ElementReference, State)
        
        var leftSize = columnChild.Measure(availableSpace);
        columnChild.Draw(availableSpace);
        var rightSize = columnChild.Measure(availableSpace);

        Child.InjectDependencies(PageContext, Canvas); // TODO: optimize to pass only canvas
        columnChild.State = originalState; // TODO: traverse the Child's tree to apply original state

        // TODO: MultiColumn takes entire horizontal space, binary search for the minimal height
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