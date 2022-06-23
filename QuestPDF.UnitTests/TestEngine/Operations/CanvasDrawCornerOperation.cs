using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasDrawCornerOperation : OperationBase
    {
        public Position First { get; }
        public Position Center { get; }
        public Position Last { get; }
        public Position CornerCenter { get; }
        public string Color { get; }

        public CanvasDrawCornerOperation(Position first, Position center, Position last, Position cornerCenter, string color)
        {
            First = first;
            Center = center;
            Last = last;
            CornerCenter = cornerCenter;
            Color = color;
        }
    }
}