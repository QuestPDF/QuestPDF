using System.Diagnostics;

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
        
        DrawingRecorder?.Record(drawingEvent);
        
        var matrixBeforeDraw = Canvas.GetCurrentMatrix().ToMatrix4x4();
        base.Draw(availableSpace);
        var matrixAfterDraw = Canvas.GetCurrentMatrix().ToMatrix4x4();
        
        if (matrixAfterDraw != matrixBeforeDraw)
            throw new InvalidOperationException("Canvas state was not restored after drawing operation.");

        drawingEvent.StateAfterDrawing = (Child as IStateful)?.GetState();
    }
}