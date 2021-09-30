namespace QuestPDF.UnitTests.TestEngine.Operations
{
    public class CanvasScaleOperation : OperationBase
    {
        public float ScaleX { get; }
        public float ScaleY { get; }

        public CanvasScaleOperation(float scaleX, float scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
        }
    }
}