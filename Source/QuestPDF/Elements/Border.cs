using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Border : ContainerElement
    {
        public Color Color { get; set; } = Colors.Black;

        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }

        internal override void Draw(Size availableSpace)
        {
            base.Draw(availableSpace);
            
            Canvas.DrawFilledRectangle(
                new Position(-Left/2, -Top/2), 
                new Size(availableSpace.Width + Left/2 + Right/2, Top), 
                Color);
            
            Canvas.DrawFilledRectangle(
                new Position(-Left/2, -Top/2), 
                new Size(Left, availableSpace.Height + Top/2 + Bottom/2), 
                Color);
            
            Canvas.DrawFilledRectangle(
                new Position(-Left/2, availableSpace.Height-Bottom/2), 
                new Size(availableSpace.Width + Left/2 + Right/2, Bottom), 
                Color);
            
            Canvas.DrawFilledRectangle(
                new Position(availableSpace.Width-Right/2, -Top/2), 
                new Size(Right, availableSpace.Height + Top/2 + Bottom/2), 
                Color);
        }

        internal override string? GetCompanionHint()
        {
            return $"C={Color}   {FormatSides()}";

            string FormatSides()
            {
                if (Top == Bottom && Right == Left && Top == Right)
                    return $"A={Top:F1}";
                
                if (Top == Bottom && Right == Left)
                    return $"V={Top:F1}   H={Left:F1}";
                
                return $"T={Top:F1}   R={Right:F1}   B={Bottom:F1}   L={Left:F1}";
            }
        }
    }
}