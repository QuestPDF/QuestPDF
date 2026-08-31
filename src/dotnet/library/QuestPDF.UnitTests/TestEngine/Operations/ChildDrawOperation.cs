using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal sealed class ChildDrawOperation : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }
        public LayoutSpace? Space { get; }

        public ChildDrawOperation(string childId, Size input, LayoutSpace? space)
        {
            ChildId = childId;
            Input = input;
            Space = space;
        }
    }
}