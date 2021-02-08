using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class ChildMeasureOperationBase : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }
        public ISpacePlan Output { get; }

        public ChildMeasureOperationBase(string childId, Size input, ISpacePlan output)
        {
            ChildId = childId;
            Input = input;
            Output = output;
        }
    }
}