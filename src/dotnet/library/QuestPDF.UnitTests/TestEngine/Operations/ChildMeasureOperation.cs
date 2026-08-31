using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal sealed class ChildMeasureOperation : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }
        public LayoutSpace? Space { get; }
        public SpacePlan Output { get; }

        public ChildMeasureOperation(string childId, Size input, LayoutSpace? space, SpacePlan output)
        {
            ChildId = childId;
            Input = input;
            Space = space;
            Output = output;
        }
    }
}