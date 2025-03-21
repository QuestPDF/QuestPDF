using System;
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

    internal sealed class Line : Element, ILine, IStateful
    {
        public LineType Type { get; set; } = LineType.Vertical;
        public Color Color { get; set; } = Colors.Black;
        public float Thickness { get; set; } = 1;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return SpacePlan.Empty();
            
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap("The available space is negative.");

            if (Type == LineType.Vertical)
            {
                if (availableSpace.Width + Size.Epsilon < Thickness)
                    return SpacePlan.Wrap("The line thickness is greater than the available horizontal space.");

                return SpacePlan.FullRender(Thickness, 0);
            }
            
            if (Type == LineType.Horizontal)
            {
                if (availableSpace.Height + Size.Epsilon < Thickness)
                    return SpacePlan.Wrap("The line thickness is greater than the available vertical space.");

                return SpacePlan.FullRender(0, Thickness);
            }

            throw new NotSupportedException();
        }

        internal override void Draw(Size availableSpace)
        {
            if (IsRendered)
                return;
            
            if (Type == LineType.Vertical)
            {
                Canvas.DrawFilledRectangle(Position.Zero, new Size(Thickness, availableSpace.Height), Color);
            }
            else if (Type == LineType.Horizontal)
            {
                Canvas.DrawFilledRectangle(Position.Zero, new Size(availableSpace.Width, Thickness), Color);
            }
            
            IsRendered = true;
        }
        
        #region IStateful
        
        private bool IsRendered { get; set; }
    
        public void ResetState(bool hardReset = false) => IsRendered = false;
        public object GetState() => IsRendered;
        public void SetState(object state) => IsRendered = (bool) state;
    
        #endregion
        
        internal override string? GetCompanionHint()
        {
            return $"{Type} {Thickness:F1} {Color.ToString()}";
        }
    }
}