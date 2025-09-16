using System.Diagnostics;
using QuestPDF.Helpers;
using QuestPDF.Skia;

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
        
        DrawingRecorder?.Record(new ElementDrawingEvent
        {
            ObserverId = ObserverId,
            PageNumber = PageContext.CurrentPage,
            Position = new Position(matrix.TranslateX, matrix.TranslateY),
            Size = ObserverId == "$document" ? Child.Measure(availableSpace) : availableSpace
        });
        
        base.Draw(availableSpace);
    }
}