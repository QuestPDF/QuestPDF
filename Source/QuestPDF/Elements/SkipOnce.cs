using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class SkipOnce : ContainerElement, IStateful
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!FirstPageWasSkipped)
                return SpacePlan.Empty();

            return base.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (FirstPageWasSkipped)
                Child.Draw(availableSpace);

            FirstPageWasSkipped = true;
        }
        
        #region IStateful
        
        private bool FirstPageWasSkipped { get; set; }
    
        public void ResetState(bool hardReset = false)
        {
            if (hardReset)
                FirstPageWasSkipped = false;
        }

        public object GetState() => FirstPageWasSkipped;
        public void SetState(object state) => FirstPageWasSkipped = (bool) state;
        
        #endregion
    }
}