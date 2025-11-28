using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    internal enum LineType
    {
        Vertical,
        Horizontal
    }

    internal sealed class Line : Element, IStateful
    {
        public LineType Type { get; set; } = LineType.Vertical;
        public Color Color { get; set; } = Colors.Black;
        public float Thickness { get; set; } = 1;
        public float[] DashPattern { get; set; } = [];
        public Color[] GradientColors { get; set; } = [];
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return SpacePlan.Empty();
            
            // TODO: this code is defensive, taking into account conditions below, it is not needed
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap("The available space is negative.");

            if (Type == LineType.Vertical)
            {
                if (Thickness.IsGreaterThan(availableSpace.Width))
                    return SpacePlan.Wrap("The line thickness is greater than the available horizontal space.");

                return SpacePlan.FullRender(Thickness, 0);
            }
            
            if (Type == LineType.Horizontal)
            {
                if (Thickness.IsGreaterThan(availableSpace.Height))
                    return SpacePlan.Wrap("The line thickness is greater than the available vertical space.");

                return SpacePlan.FullRender(0, Thickness);
            }

            // Stryker disable once: unreachable code
            throw new NotSupportedException();
        }

        internal override void Draw(Size availableSpace)
        {
            if (IsRendered)
                return;
            
            var start = Position.Zero;
            
            var end = Type == LineType.Vertical
                ? new Position(0, availableSpace.Height)
                : new Position(availableSpace.Width, 0);

            using var paint = new SkPaint();
            paint.SetStroke(Thickness);
            paint.SetSolidColor(Color);
            
            if (GradientColors.Length > 0)
                paint.SetLinearGradient(start, end, GradientColors);

            if (DashPattern.Length > 0)
                paint.SetDashedPathEffect(DashPattern);

            var offset = Type == LineType.Vertical
                ? new Position(Thickness / 2, 0)
                : new Position(0, Thickness / 2);
            
            Canvas.Translate(offset);
            Canvas.DrawLine(start, end, paint);
            Canvas.Translate(offset.Reverse());
            
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
            return $"{Type} {Thickness:0.#}";
        }
    }
}