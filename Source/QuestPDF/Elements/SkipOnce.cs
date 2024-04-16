using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class SkipOnce : ContainerElement, IStateResettable
    {
        private bool FirstPageWasSkipped { get; set; }

        public void ResetState()
        {
            FirstPageWasSkipped = false;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!FirstPageWasSkipped)
                return SpacePlan.Empty();

            return Child.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (FirstPageWasSkipped)
                Child.Draw(availableSpace);

            FirstPageWasSkipped = true;
        }
    }
}