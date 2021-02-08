using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasTranslateOperationBase : OperationBase
    {
        public Position Position { get; }

        public CanvasTranslateOperationBase(Position position)
        {
            Position = position;
        }
    }
}