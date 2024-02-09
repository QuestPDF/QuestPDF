using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Background : ContainerElement
    {
        public Color Color { get; set; } = Colors.Black;
        
        internal override void Draw(Size availableSpace)
        {
            Canvas.DrawFilledRectangle(Position.Zero, availableSpace, Color);
            base.Draw(availableSpace);
        }
    }
}