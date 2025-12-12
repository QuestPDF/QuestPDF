using System.Diagnostics;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;

namespace QuestPDF.LayoutTests.TestEngine;



internal class ElementObserver : ContainerElement
{
    public string? ObserverId { get; set; }
    public DrawingRecorder? DrawingRecorder { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        Debug.Assert(ObserverId != null);
        Debug.Assert(DrawingRecorder != null);
        
        var matrix = Canvas.GetCurrentMatrix();

        var drawingEvent = new ElementDrawingEvent
        {
            ObserverId = ObserverId,
            PageNumber = PageContext.CurrentPage,
            Position = new Position(matrix.TranslateX, matrix.TranslateY),
            Size = ObserverId == "$document" ? Child.Measure(availableSpace) : availableSpace
        };
        
        if (!IsDiscardDrawingCanvas())
            DrawingRecorder?.Record(drawingEvent);
        
        var matrixBeforeDraw = Canvas.GetCurrentMatrix().ToMatrix4x4();
        base.Draw(availableSpace);
        var matrixAfterDraw = Canvas.GetCurrentMatrix().ToMatrix4x4();
        
        if (matrixAfterDraw != matrixBeforeDraw)
            throw new InvalidOperationException("Canvas state was not restored after drawing operation.");

        drawingEvent.StateAfterDrawing = (GetRealChild() as IStateful)?.GetState();
    }

    private Element GetRealChild()
    {
        var result = Child;

        while (true)
        {
            if (result is ElementProxy proxy)
            {
                result = proxy.Child;
                continue;
            }

            if (result is DebugPointer debugPointer)
            {
                result = debugPointer.Child;
                continue;
            }
            
            break;
        }
        
        return result;
    }

    private bool IsDiscardDrawingCanvas()
    {
        var canvasUnderTest = Canvas;

        while (canvasUnderTest is ProxyDrawingCanvas proxy)
            canvasUnderTest = proxy.Target;

        return canvasUnderTest is DiscardDrawingCanvas;
    }
}