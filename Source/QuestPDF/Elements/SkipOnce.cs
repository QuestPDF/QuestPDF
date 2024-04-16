using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class SkipOnce : ContainerElement, IStateful
    {
        private bool IsFirstPageWasSkipped { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!IsFirstPageWasSkipped)
                return SpacePlan.None();

            return Child.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (IsFirstPageWasSkipped)
                Child.Draw(availableSpace);

            IsFirstPageWasSkipped = true;
        }
        
        #region IStateful
    
        object IStateful.CloneState()
        {
            return IsFirstPageWasSkipped;
        }

        void IStateful.SetState(object state)
        {
            IsFirstPageWasSkipped = (bool) state;
        }

        void IStateful.ResetState(bool hardReset)
        {
            if (hardReset)
                IsFirstPageWasSkipped = false;
        }
    
        #endregion
    }
}