using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class PageBreak : Element, IStateResettable
    {
        private bool IsRendered { get; set; }
        
        public void ResetState()
        {
            IsRendered = false;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.Width < 0f || availableSpace.Height < 0f)
                return SpacePlan.Wrap();
            
            if (IsRendered)
                return SpacePlan.FullRender(0, 0);

            return SpacePlan.PartialRender(Size.Zero);
        }

        internal override void Draw(Size availableSpace)
        {
            IsRendered = true;
        }
    }
}