using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class PageBreak : Element
    {
        private bool IsRendered { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return new FullRender(Size.Zero);
            
            return new PartialRender(availableSpace.Width, 1);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            IsRendered = true;
        }
    }
}