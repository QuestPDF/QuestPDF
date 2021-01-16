using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Debug : ContainerElement
    {
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var textStyle = new TextStyle
            {
                Color = "#FF0000",
                FontType = "Consolas",
                Size = 10
            };
            
            Child?.Draw(canvas, availableSpace);
            
            canvas.DrawRectangle(Position.Zero, availableSpace, "#FF0000");
            canvas.DrawRectangle(Position.Zero, availableSpace, "#FF00FF");
            
            canvas.DrawText($"W: {availableSpace.Width}", new Position(5, 12), textStyle);
            canvas.DrawText($"H: {availableSpace.Height}", new Position(5, 22), textStyle);
        }
    }
}