using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Background : ContainerElement
    {
        public string Color { get; set; } = Colors.Black;
        
        internal override void Draw(Size availableSpace)
        {
            Canvas.DrawRectangle(Position.Zero, availableSpace, Color);
            Child?.Draw(availableSpace);
        }
    }
}