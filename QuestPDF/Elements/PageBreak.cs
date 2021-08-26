using QuestPDF.Drawing.SpacePlan;
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

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return new FullRender(Size.Zero);

            return new PartialRender(Size.Zero);
        }

        internal override void Draw(Size availableSpace)
        {
            IsRendered = true;
        }
    }
}