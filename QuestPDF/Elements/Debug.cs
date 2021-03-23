using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Debug : ContainerElement
    {
        private static readonly TextStyle TextStyle = TextStyle.Default.Color("#FF0000").FontType("Consolas").Size(10);
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Child?.Draw(canvas, availableSpace);
            DrawBoundingBox();
            DrawDimensions();
                
            void DrawBoundingBox()
            {
                // TODO: when layer element is done, move this code into fluent API
                
                var container = new Container();

                container
                    .Border(1)
                    .BorderColor("#FF0000")
                    .Background("#33FF0000");

                container.Draw(canvas, availableSpace);
            }

            void DrawDimensions()
            {
                canvas.DrawText($"W: {availableSpace.Width:F1}", new Position(5, 12), TextStyle);
                canvas.DrawText($"H: {availableSpace.Height:F1}", new Position(5, 22), TextStyle);
            }
        }
    }
}