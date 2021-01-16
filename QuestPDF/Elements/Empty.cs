using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Empty : Element
    {
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(Size.Zero);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            
        }
    }
}