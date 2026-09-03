using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    /// <summary>
    /// Prefers to render the content on a single page: content that would be split is moved to the next page,
    /// and split only where no page can hold it entirely.
    /// </summary>
    /// <remarks>
    /// The move is requested only when the offered height is flowing, because only then can the next page offer more.
    /// A final height is the most any page will ever offer, so the content is rendered as it is. This is also why
    /// no state is needed: an element offered the entire height of a page is never asked to move again.
    /// </remarks>
    internal sealed class PreventPageBreak : ContainerElement
    {
        private bool WasSplit { get; set; }
        
        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (ShouldMoveToTheNextPage(availableSpace, measurement))
                return SpacePlan.PartialRender(Size.Zero);
            
            if (availableSpace.IsHeightFinal && measurement.Type is SpacePlanType.PartialRender)
                WasSplit = true;
            
            return measurement;
        }
        
        internal override void Draw(LayoutSpace availableSpace)
        {
            var measurement = base.Measure(availableSpace);
            
            if (ShouldMoveToTheNextPage(availableSpace, measurement))
                return;
            
            base.Draw(availableSpace);
        }

        private static bool ShouldMoveToTheNextPage(LayoutSpace availableSpace, SpacePlan measurement)
        {
            if (measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                return false;
            
            return availableSpace.HeightMode == LayoutAxisMode.Flowing;
        }

        internal override string? GetCompanionHint()
        {
            return WasSplit ? "split across pages: no page can hold the content entirely" : null;
        }
    }
}
