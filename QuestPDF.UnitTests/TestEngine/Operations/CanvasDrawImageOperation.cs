using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasDrawImageOperationBase : OperationBase
    {
        public Position Position { get; }
        public Size Size { get; }

        public CanvasDrawImageOperationBase(Position position, Size size)
        {
            Position = position;
            Size = size;
        }
    }
}