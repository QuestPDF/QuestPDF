using System;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Border : ContainerElement
    {
        public string Color { get; set; } = Colors.Black;

        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }

        internal override void Draw(Size availableSpace)
        {
            base.Draw(availableSpace);

            if (HasEqualWidthOnAllSides())
            {
                Canvas.DrawStrokedRectangle(availableSpace, Color, Top);
            }
            else
            {
                DrawCustomRectangle(availableSpace);
            }
        }

        private void DrawCustomRectangle(Size size)
        {
            Canvas.DrawFilledRectangle(
                new Position(-Left / 2, -Top / 2),
                new Size(size.Width + Left / 2 + Right / 2, Top),
                Color);

            Canvas.DrawFilledRectangle(
                new Position(-Left / 2, -Top / 2),
                new Size(Left, size.Height + Top / 2 + Bottom / 2),
                Color);

            Canvas.DrawFilledRectangle(
                new Position(-Left / 2, size.Height - Bottom / 2),
                new Size(size.Width + Left / 2 + Right / 2, Bottom),
                Color);

            Canvas.DrawFilledRectangle(
                new Position(size.Width - Right / 2, -Top / 2),
                new Size(Right, size.Height + Top / 2 + Bottom / 2),
                Color);
        }
        
        private bool HasEqualWidthOnAllSides()
        {
            return IsClose(Top, Bottom) && 
                   IsClose(Left, Right) &&
                   IsClose(Top, Left);
        }

        private static bool IsClose(float x, float y)
        {
            return Math.Abs(x - y) < Size.Epsilon;
        }
    }
}