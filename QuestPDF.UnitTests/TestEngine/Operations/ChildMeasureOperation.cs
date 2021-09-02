using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class ChildMeasureOperationBase : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }
        public SpacePlan Output { get; }

        public ChildMeasureOperationBase(string childId, Size input, SpacePlan output)
        {
            ChildId = childId;
            Input = input;
            Output = output;
        }
    }
}