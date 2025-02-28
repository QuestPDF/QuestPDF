using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class PreventPageBreak : ContainerElement, IStateful
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (IsFirstPageRendered)
                return measurement;

            if (measurement.Type != SpacePlanType.PartialRender)
                return measurement;

            return SpacePlan.PartialRender(Size.Zero);
        }
        
        internal override void Draw(Size availableSpace)
        {
            if (IsFirstPageRendered)
            {
                base.Draw(availableSpace);
                return;
            }

            var measurement = base.Measure(availableSpace);
            
            if (measurement.Type == SpacePlanType.FullRender)
                base.Draw(availableSpace);
        
            IsFirstPageRendered = true;
        }
        
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