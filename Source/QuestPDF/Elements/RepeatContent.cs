using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class RepeatContent : ContainerElement
    {
        internal override void Draw(Size availableSpace)
        {
            var childMeasurement = Child?.Measure(availableSpace);
            base.Draw(availableSpace);
            
            if (childMeasurement?.Type == SpacePlanType.FullRender)
            {
                Child.VisitChildren(x => (x as IStateful)?.ResetState(false));
            }
        }
    }
}