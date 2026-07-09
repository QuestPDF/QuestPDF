using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ExpectedDocumentLayoutDescriptor(DrawingRecorder DrawingRecorder)
{
    private int CurrentPage { get; set; } = 1;
    
    public ExpectedPageLayoutDescriptor Page()
    {
        return new ExpectedPageLayoutDescriptor(DrawingRecorder, CurrentPage++);
    }
}

internal class ExpectedPageLayoutDescriptor(DrawingRecorder DrawingRecorder, int CurrentPageNumber)
{
    public ExpectedPageLayoutDescriptor RequiredAreaSize(float width, float height)
    {
        DrawingRecorder.Record(new ElementDrawingEvent
        {
            ObserverId = "$document",
            PageNumber = CurrentPageNumber,
            Size = new Size(width, height)
        });
        
        return this;
    }
    
    public void Content(Action<ExpectedPageContentDescriptor> content)
    {
        var pageContent = new ExpectedPageContentDescriptor(DrawingRecorder, CurrentPageNumber);
        content(pageContent);
    }
}

internal class ExpectedPageContentDescriptor(DrawingRecorder drawingRecorder, int CurrentPageNumber)
{
    public ExpectedMockPositionDescriptor Mock(string mockId)
    {
        var elementDrawingEvent = new ElementDrawingEvent
        {
            ObserverId = mockId,
            PageNumber = CurrentPageNumber,
        };
        
        drawingRecorder.Record(elementDrawingEvent);
        return new ExpectedMockPositionDescriptor(elementDrawingEvent);
    }
}

internal class ExpectedMockPositionDescriptor(ElementDrawingEvent drawingEvent)
{
    public ExpectedMockPositionDescriptor Position(float x, float y)
    {
        drawingEvent.Position = new Position(x, y);
        return this;
    }
    
    public ExpectedMockPositionDescriptor Size(float width, float height)
    {
        drawingEvent.Size = new Size(width, height);
        return this;
    }
    
    public ExpectedMockPositionDescriptor State(object state)
    {
        drawingEvent.StateAfterDrawing = state;
        return this;
    }
}

internal static class FluentExtensions
{
    public const string DefaultMockId = "$mock";
    
    public static IContainer Mock(this IContainer element, string id)
    {
        return element.Element(new ElementObserver
        {
            ObserverId = id
        });
    } 

    public static IContainer ElementObserverSetter(this IContainer element, DrawingRecorder recorder)
    {
        return element.Element(new ElementObserverSetter
        {
            Recorder = recorder
        });
    } 
    
    public static IContainer Size(this IContainer element, float width, float height)
    {
        return element.Width(width).Height(height);
    }     
    
    public static void ContinuousBlock(this IContainer element, float width = 1f, float height = 1f)
    {
        element.Element(new ContinuousBlock
        {
            TotalWidth = width, 
            TotalHeight = height
        });
    } 
    
    public static void SolidBlock(this IContainer element, float width = 1f, float height = 1f)
    {
        element.Element(new SolidBlock
        {
            TotalWidth = width, 
            TotalHeight = height
        });
    } 
}