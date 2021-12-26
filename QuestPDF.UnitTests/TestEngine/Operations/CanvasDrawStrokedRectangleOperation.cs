using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasDrawStrokedRectangleOperation : OperationBase
    {
        public Size Size { get; }
        public string Color { get; }
        public float Width { get; set; }
        
        public CanvasDrawStrokedRectangleOperation(Size size, string color, float width)
        {
            Size = size;
            Color = color;
            Width = width;
        }
    }
}