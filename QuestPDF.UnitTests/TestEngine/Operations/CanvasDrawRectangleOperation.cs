using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasDrawRectangleOperationBase : OperationBase
    {
        public Position Position { get; } 
        public Size Size { get; }
        public string Color { get; }

        public CanvasDrawRectangleOperationBase(Position position, Size size, string color)
        {
            Position = position;
            Size = size;
            Color = color;
        }
    }
}