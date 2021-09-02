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
            
            Canvas.DrawRectangle(
                new Position(-Left/2, -Top/2), 
                new Size(availableSpace.Width + Left/2 + Right/2, Top), 
                Color);
            
            Canvas.DrawRectangle(
                new Position(-Left/2, -Top/2), 
                new Size(Left, availableSpace.Height + Top/2 + Bottom/2), 
                Color);
            
            Canvas.DrawRectangle(
                new Position(-Left/2, availableSpace.Height-Bottom/2), 
                new Size(availableSpace.Width + Left/2 + Right/2, Bottom), 
                Color);
            
            Canvas.DrawRectangle(
                new Position(availableSpace.Width-Right/2, -Top/2), 
                new Size(Right, availableSpace.Height + Top/2 + Bottom/2), 
                Color);
        }
    }
}