using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Background : ContainerElement
    {
        public string Color { get; set; } = "#00000000";
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            canvas.DrawRectangle(Position.Zero, availableSpace, Color);
            Child?.Draw(canvas, availableSpace);
        }
    }
}