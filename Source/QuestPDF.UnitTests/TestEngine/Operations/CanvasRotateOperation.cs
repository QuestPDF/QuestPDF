namespace QuestPDF.UnitTests.TestEngine.Operations
{
    public class CanvasRotateOperation : OperationBase
    {
        public float Angle { get; }

        public CanvasRotateOperation(float angle)
        {
            Angle = angle;
        }
    }
}