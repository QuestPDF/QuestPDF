using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasDrawRectangleOperation : OperationBase
    {
        public Position Position { get; } 
        public Size Size { get; }
        public string Color { get; }

        public CanvasDrawRectangleOperation(Position position, Size size, string color)
        {
            Position = position;
            Size = size;
            Color = color;
        }
    }
}