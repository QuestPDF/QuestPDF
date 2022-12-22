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

        public float TopLeftCorner { get; set; }

        public float TopRightCorner { get; set; }

        public float BottomLeftCorner { get; set; }

        public float BottomRightCorner { get; set; }

        internal override void Draw(Size availableSpace)
        {
            base.Draw(availableSpace);

            #region Draw Corners

            if (TopLeftCorner > 0 && Left > 0 && Top > 0)
                Canvas.DrawCorner(new Position(-Left / 2 + TopLeftCorner, -Top / 2),
                    new Position(-Left / 2, -Top / 2),
                    new Position(-Left / 2, -Top / 2 + TopLeftCorner),
                    new Position(-Left / 2 + Left, -Top / 2 + Top), Color);

            if (BottomLeftCorner > 0 && Left > 0 && Bottom > 0)
                Canvas.DrawCorner(
                    new Position(-Left / 2 + BottomLeftCorner, availableSpace.Height - Bottom / 2 + Bottom),
                    new Position(-Left / 2, availableSpace.Height - Bottom / 2 + Bottom),
                    new Position(-Left / 2, availableSpace.Height - Bottom / 2 + Bottom - BottomLeftCorner),
                    new Position(-Left / 2 + Left, availableSpace.Height - Bottom / 2), Color);

            if (BottomRightCorner > 0 && Bottom > 0 && Right > 0)
                Canvas.DrawCorner(
                    new Position(availableSpace.Width - Right / 2 - BottomRightCorner + Right,
                        availableSpace.Height - Bottom / 2 + Bottom),
                    new Position(availableSpace.Width - Right / 2 + Right, availableSpace.Height - Bottom / 2 + Bottom),
                    new Position(availableSpace.Width - Right / 2 + Right,
                        availableSpace.Height - Bottom / 2 + Bottom - BottomRightCorner),
                    new Position(availableSpace.Width - Right / 2, availableSpace.Height - Bottom / 2), Color);

            if (TopRightCorner > 0 && Right > 0 && Top > 0)
                Canvas.DrawCorner(new Position(availableSpace.Width - Right / 2 - TopRightCorner + Right, -Top / 2),
                    new Position(availableSpace.Width - Right / 2 + Right, -Top / 2),
                    new Position(availableSpace.Width - Right / 2 + Right, -Top / 2 + TopRightCorner),
                    new Position(availableSpace.Width - Right / 2, -Top / 2 + Top), Color);

            #endregion

            Canvas.DrawRectangle(new Position(-Left / 2 + TopLeftCorner, -Top / 2),
                new Size(availableSpace.Width + Left / 2 + Right / 2 - TopLeftCorner - TopRightCorner, Top),
                Color);

            Canvas.DrawRectangle(new Position(-Left / 2, -Top / 2 + TopLeftCorner),
                new Size(Left, availableSpace.Height + Top / 2 + Bottom / 2 - TopLeftCorner - BottomLeftCorner),
                Color);

            Canvas.DrawRectangle(
                new Position(-Left / 2 + BottomLeftCorner, availableSpace.Height - Bottom / 2),
                new Size(availableSpace.Width + Left / 2 + Right / 2 - BottomLeftCorner - BottomRightCorner, Bottom),
                Color);

            Canvas.DrawRectangle(
                new Position(availableSpace.Width - Right / 2, -Top / 2 + TopRightCorner),
                new Size(Right, availableSpace.Height + Top / 2 + Bottom / 2 - TopRightCorner - BottomRightCorner),
                Color);
        }

        public override string ToString()
        {
            return $"Border: Top({Top}) Right({Right}) Bottom({Bottom}) Left({Left}) " +
                   $"Corners({TopLeftCorner},{TopRightCorner},{BottomLeftCorner},{BottomRightCorner}) Color({Color})";
        }
    }
}