using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal sealed class CanvasDrawRectangleOperation : OperationBase
    {
        public Position Position { get; } 
        public Size Size { get; }
        public Color Color { get; }

        public CanvasDrawRectangleOperation(Position position, Size size, Color color)
        {
            Position = position;
            Size = size;
            Color = color;
        }
    }
}