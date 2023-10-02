using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal sealed class CanvasTranslateOperation : OperationBase
    {
        public Position Position { get; }

        public CanvasTranslateOperation(Position position)
        {
            Position = position;
        }
    }
}