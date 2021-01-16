using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ShowOnce : ContainerElement
    {
        private bool IsRendered { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return new FullRender(Size.Zero);
            
            return Child.Measure(availableSpace);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            if (Child == null || IsRendered)
                return;
            
            if (Child.Measure(availableSpace) is FullRender)
                IsRendered = true;
            
            Child.Draw(canvas, availableSpace);
        }
    }
}