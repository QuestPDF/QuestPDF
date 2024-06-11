using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ShowOnce : ContainerElement, IStateResettable
    {
        private bool IsRendered { get; set; }

        public void ResetState(bool hardReset)
        {
            if (hardReset)
                IsRendered = false;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return SpacePlan.Empty();
            
            return base.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (IsRendered)
                return;
            
            if (base.Measure(availableSpace).Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                IsRendered = true;
            
            base.Draw(availableSpace);
        }
    }
}