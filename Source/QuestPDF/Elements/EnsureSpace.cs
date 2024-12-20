using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class EnsureSpace : ContainerElement, IStateful
    {
        public float? MinHeight { get; set; } = null;

        internal override SpacePlan Measure(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (IsFirstPageRendered)
                return measurement;

            if (measurement.Type != SpacePlanType.PartialRender)
                return measurement;

            if (MinHeight == null || MinHeight <= availableSpace.Height)
                return measurement;
            
            return SpacePlan.Wrap("The available vertical space is smaller than requested in the constraint.");
        }
        
        internal override void Draw(Size availableSpace)
        {
            base.Draw(availableSpace);
            IsFirstPageRendered = true;
        }

        internal override string? GetCompanionHint() => MinHeight.HasValue ? $"at least {MinHeight.Value}" : null;
        
        #region IStateful
        
        private bool IsFirstPageRendered { get; set; }

        public void ResetState(bool hardReset = false)
        {
            if (hardReset)
                IsFirstPageRendered = false;
        }
        
        public object GetState() => IsFirstPageRendered;
        public void SetState(object state) => IsFirstPageRendered = (bool) state;
    
        #endregion
    }
}