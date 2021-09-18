using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class ChildMeasureOperation : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }
        public ISpacePlan Output { get; }

        public ChildMeasureOperation(string childId, Size input, ISpacePlan output)
        {
            ChildId = childId;
            Input = input;
            Output = output;
        }
    }
}