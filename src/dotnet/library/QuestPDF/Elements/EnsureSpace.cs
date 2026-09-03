using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    /// <summary>
    /// Moves the content to the next page when less than the configured height of it would be rendered here.
    /// </summary>
    /// <remarks>
    /// The move is requested only when the offered height is flowing, because only then can the next page offer more.
    /// A final height is the most any page will ever offer, so the content is rendered as it is. This is also why
    /// no state is needed: an element offered the entire height of a page is never asked to move again.
    /// </remarks>
    internal sealed class EnsureSpace : ContainerElement
    {
        public const float DefaultMinHeight = 150;
        public float MinHeight { get; set; } = DefaultMinHeight;

        private bool WasRenderedBelowTheMinimum { get; set; }

        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (ShouldMoveToTheNextPage(availableSpace, measurement))
                return SpacePlan.PartialRender(Size.Zero);
            
            if (availableSpace.IsHeightFinal && measurement.Type is SpacePlanType.PartialRender && measurement.Height < MinHeight)
                WasRenderedBelowTheMinimum = true;
            
            return measurement;
        }
        
        internal override void Draw(LayoutSpace availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (ShouldMoveToTheNextPage(availableSpace, measurement))
                return;
            
            base.Draw(availableSpace);
        }

        private bool ShouldMoveToTheNextPage(LayoutSpace availableSpace, SpacePlan measurement)
        {
            if (measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                return false;
            
            if (measurement.Type is SpacePlanType.PartialRender && MinHeight <= measurement.Height)
                return false;
            
            return availableSpace.HeightMode == LayoutAxisMode.Flowing;
        }

        internal override string? GetCompanionHint()
        {
            var hint = $"at least {MinHeight}";
            
            return WasRenderedBelowTheMinimum
                ? $"{hint}, not satisfiable: no page offers more"
                : hint;
        }
    }
}
