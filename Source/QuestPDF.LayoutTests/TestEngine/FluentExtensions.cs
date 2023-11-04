using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ExpectedDocumentLayoutDescriptor
{
    public List<LayoutTestResult.PageLayoutSnapshot> PageLayouts { get; } = new(); 
    
    public ExpectedPageLayoutDescriptor Page()
    {
        var page = new LayoutTestResult.PageLayoutSnapshot();
        PageLayouts.Add(page);
        return new ExpectedPageLayoutDescriptor(page);
    }
}

internal class ExpectedPageLayoutDescriptor
{
    private LayoutTestResult.PageLayoutSnapshot PageLayout { get; }

    public ExpectedPageLayoutDescriptor(LayoutTestResult.PageLayoutSnapshot pageLayout)
    {
        PageLayout = pageLayout;
    }

    public ExpectedPageLayoutDescriptor TakenAreaSize(float width, float height)
    {
        PageLayout.RequiredArea = new Size(width, height);
        return this;
    }
    
    public ExpectedPageLayoutDescriptor Content(Action<ExpectedPageContentDescriptor> content)
    {
        var pageContent = new ExpectedPageContentDescriptor();
        content(pageContent);
        
        PageLayout.MockPositions = pageContent.MockPositions;
        return this;
    }
}

internal class ExpectedPageContentDescriptor
{
    public List<LayoutTestResult.MockLayoutPosition> MockPositions { get;} = new();
    
    public ExpectedMockPositionDescriptor Mock(string mockId)
    {
        var child = new LayoutTestResult.MockLayoutPosition { MockId = mockId };
        MockPositions.Add(child);
        return new ExpectedMockPositionDescriptor(child);
    }
}

internal class ExpectedMockPositionDescriptor
{
    private LayoutTestResult.MockLayoutPosition MockLayoutPosition { get; }

    public ExpectedMockPositionDescriptor(LayoutTestResult.MockLayoutPosition mockLayoutPosition)
    {
        MockLayoutPosition = mockLayoutPosition;
    }

    public ExpectedMockPositionDescriptor Position(float x, float y)
    {
        MockLayoutPosition.Position = new Position(x, y);
        return this;
    }
    
    public ExpectedMockPositionDescriptor Size(float width, float height)
    {
        MockLayoutPosition.Size = new Size(width, height);
        return this;
    }
}

internal static class ElementExtensions
{
    public static MockDescriptor Mock(this IContainer element, string id)
    {
        var mock = new ElementMock
        {
            MockId = id
        };
        
        element.Element(mock);
        return new MockDescriptor(mock);
    } 
}

internal class MockDescriptor
{
    private ElementMock Mock { get; }

    public MockDescriptor(ElementMock mock)
    {
        Mock = mock;
    }

    public MockDescriptor Size(float width, float height)
    {
        Mock.TotalWidth = width;
        Mock.TotalHeight = height;

        return this;
    }
}