using System.Collections;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class MultiColumn : ContainerElement
{
    internal override SpacePlan Measure(Size availableSpace)
    {
        Child.InjectCanvas(new FreeCanvas());
        var originalState = GetState();
    
        var leftSize = Child.Measure(availableSpace);
        Child.Draw(availableSpace);
        var rightSize = Child.Measure(availableSpace);
    
        Child.InjectCanvas(Canvas);
        SetState(originalState);
    
        if (leftSize.Type == SpacePlanType.FullRender || rightSize.Type == SpacePlanType.FullRender)
            return SpacePlan.FullRender(availableSpace);
    
        return SpacePlan.PartialRender(availableSpace);

        ICollection<(Element element, object state)> GetState()
        {
            var result = new List<(Element element, object state)>();
            
            Child.VisitChildren(x =>
            {
                if (x is IStateful stateful)
                    result.Add((x, stateful.CloneState()));
            });
            
            return result;
        }

        void SetState(ICollection<(Element element, object state)> state)
        {
            foreach (var (element, elementState) in state)
            {
                (element as IStateful).SetState(elementState);
            }
        }
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