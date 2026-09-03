using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    /// <summary>
    /// Keeps the content on a single page: content that would be split is moved to the next page instead.
    /// </summary>
    /// <remarks>
    /// The move is requested only when it can change the answer, that is when the offered height is flowing.
    /// A final height is the most any page will ever offer, so no page can hold the content entirely,
    /// and rendering it split is the nearest reading of the configuration that the space allows.
    /// A query asks for the natural size, which is likewise described as it is.
    /// </remarks>
    internal sealed class ShowEntire : ContainerElement
    {
        private bool WasSplit { get; set; }
        
        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            var childMeasurement = base.Measure(availableSpace);
            
            if (childMeasurement.Type is SpacePlanType.Wrap)
                return SpacePlan.Wrap("Child element does not fit (even partially) on the page.");

            if (childMeasurement.Type is not SpacePlanType.PartialRender)
                return childMeasurement;
            
            if (availableSpace.HeightMode == LayoutAxisMode.Flowing)
                return SpacePlan.Wrap("Child element fits only partially on the page.");

            if (availableSpace.IsHeightFinal)
                WasSplit = true;
            
            return childMeasurement;
        }

        internal override string? GetCompanionHint()
        {
            return WasSplit ? "split across pages: no page can hold the content entirely" : null;
        }
    }
}
