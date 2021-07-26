using QuestPDF.Drawing.SpacePlan;
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

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return new FullRender(Size.Zero);
            
            return Child.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return;
            
            if (Child.Measure(availableSpace) is FullRender)
                IsRendered = true;
            
            Child.Draw(availableSpace);
        }
    }
}