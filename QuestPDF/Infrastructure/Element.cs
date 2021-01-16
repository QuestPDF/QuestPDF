using QuestPDF.Drawing.SpacePlan;

namespace QuestPDF.Infrastructure
{
    internal abstract class Element : IElement
    {
        internal abstract ISpacePlan Measure(Size availableSpace);
        internal abstract void Draw(ICanvas canvas, Size availableSpace);
    }
}