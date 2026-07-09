using System.Collections.ObjectModel;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ElementDrawingEvent
{
    public string ObserverId { get; set; }
    public int PageNumber { get; set; }
    public Position Position { get; set; }
    public Size Size { get; set; }
    public object? StateAfterDrawing { get; set; }
}

internal class DrawingRecorder
{
    private List<ElementDrawingEvent> DrawingEvents { get; } = [];
    
    public void Record(ElementDrawingEvent elementDrawingEvent)
    {
        DrawingEvents.Add(elementDrawingEvent);
    }
    
    public IReadOnlyCollection<ElementDrawingEvent> GetDrawingEvents()
    {
        return new ReadOnlyCollection<ElementDrawingEvent>(DrawingEvents);
    }
}