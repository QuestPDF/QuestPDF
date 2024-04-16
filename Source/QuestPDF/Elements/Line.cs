using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    public interface ILine
    {
        
    }
    
    internal enum LineType
    {
        Vertical,
        Horizontal
    }

    internal sealed class Line : Element, ILine, IStateful, ICacheable
    {
        public bool IsRendered { get; set; }
        
        public LineType Type { get; set; } = LineType.Vertical;
        public Color Color { get; set; } = Colors.Black;
        public float Size { get; set; } = 1;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();

            if (IsRendered)
                return SpacePlan.None();
            
            return Type switch
            {
                LineType.Vertical when availableSpace.Width + Infrastructure.Size.Epsilon >= Size => SpacePlan.FullRender(Size, 0),
                LineType.Horizontal when availableSpace.Height + Infrastructure.Size.Epsilon >= Size => SpacePlan.FullRender(0, Size),
                _ => SpacePlan.Wrap()
            };
        }

        internal override void Draw(Size availableSpace)
        {
            if (Type == LineType.Vertical)
            {
                Canvas.DrawFilledRectangle(new Position(-Size/2, 0), new Size(Size, availableSpace.Height), Color);
            }
            else if (Type == LineType.Horizontal)
            {
                Canvas.DrawFilledRectangle(new Position(0, -Size/2), new Size(availableSpace.Width, Size), Color);
            }
            
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