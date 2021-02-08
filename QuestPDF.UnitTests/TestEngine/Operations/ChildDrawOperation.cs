using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    public class ChildDrawOperationBase : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }

        public ChildDrawOperationBase(string childId, Size input)
        {
            ChildId = childId;
            Input = input;
        }
    }
}