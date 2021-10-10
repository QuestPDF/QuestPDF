using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SkipOnce : ContainerElement, IStateResettable
    {
        private bool FirstPageWasSkipped { get; set; }

        public void ResetState()
        {
            FirstPageWasSkipped = false;
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (Child == null || !FirstPageWasSkipped)
                return new FullRender(Size.Zero);

            return Child.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;

            if (FirstPageWasSkipped)
                Child.Draw(availableSpace);

            FirstPageWasSkipped = true;
        }
    }
}