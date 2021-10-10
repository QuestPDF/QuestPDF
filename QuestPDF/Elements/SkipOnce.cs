using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SkipOnce : ContainerElement, IStateResettable
    {
        private bool firstPageWasSkiped;

        public void ResetState()
        {
            firstPageWasSkiped = false;
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (Child == null || !firstPageWasSkiped)
                return new FullRender(Size.Zero);

            return Child.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;

            if (firstPageWasSkiped)
                Child.Draw(availableSpace);

            firstPageWasSkiped = true;
        }
    }
}