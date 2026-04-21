using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class SkipLast : ContainerElement
    {
        internal DecorationLastPageState? PageState { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (PageState == null)
                throw new DocumentComposeException("The SkipLast element can only be used within the Before or After slot of a Decoration element.");

            if (PageState.IsLastPage)
                return SpacePlan.Empty();

            return base.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (PageState?.IsLastPage == true)
                return;

            base.Draw(availableSpace);
        }
    }
}
