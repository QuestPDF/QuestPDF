using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasDrawTextOperation : OperationBase
    {
        public string Text { get; }
        public Position Position { get; }
        public TextStyle Style { get; }

        public CanvasDrawTextOperation(string text, Position position, TextStyle style)
        {
            Text = text;
            Position = position;
            Style = style;
        }
    }
}