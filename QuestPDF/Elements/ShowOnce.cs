using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ShowOnce : ContainerElement, IStateResettable
    {
        private bool IsRendered { get; set; }

        public void ResetState()
        {
            IsRendered = false;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return SpacePlan.FullRender(0, 0);
            
            return base.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return;
            
            if (base.Measure(availableSpace).Type == SpacePlanType.FullRender)
                IsRendered = true;
            
            base.Draw(availableSpace);
        }
    }
}