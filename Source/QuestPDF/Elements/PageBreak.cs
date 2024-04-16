using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class PageBreak : Element, IStateful
    {
        public bool IsRendered { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();
            
            if (IsRendered)
                return SpacePlan.FullRender(0, 0);

            return SpacePlan.PartialRender(Size.Zero);
        }

        internal override void Draw(Size availableSpace)
        {
            IsRendered = true;
        }
        
        #region IStateful
    
        object IStateful.CloneState()
        {
            return IsRendered;
        }

        void IStateful.SetState(object state)
        {
            IsRendered = (bool) state;
        }

        void IStateful.ResetState(bool hardReset)
        {
            IsRendered = false;
        }
    
        #endregion
    }
}