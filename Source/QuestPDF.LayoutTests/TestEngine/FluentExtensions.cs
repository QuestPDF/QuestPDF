using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ExpectedDocumentLayoutDescriptor
{
    public LayoutTestResult.DocumentLayout DocumentLayout { get; } = new(); 
    
    public ExpectedPageLayoutDescriptor Page()
    {
        var page = new LayoutTestResult.PageLayout();
        DocumentLayout.Pages.Add(page);
        return new ExpectedPageLayoutDescriptor(page);
    }
    
    public void ExpectInfiniteLayoutException()
    {
        DocumentLayout.GeneratesInfiniteLayout = true;
    }
}

internal class ExpectedPageLayoutDescriptor
{
    private LayoutTestResult.PageLayout PageLayout { get; }

    public ExpectedPageLayoutDescriptor(LayoutTestResult.PageLayout pageLayout)
    {
        PageLayout = pageLayout;
    }
    
    public ExpectedPageLayoutDescriptor RequiredAreaSize(float width, float height)
    {
        PageLayout.RequiredArea = new Size(width, height);
        return this;
    }
    
    public ExpectedPageLayoutDescriptor Content(Action<ExpectedPageContentDescriptor> content)
    {
        var pageContent = new ExpectedPageContentDescriptor();
        content(pageContent);
        
        PageLayout.Mocks = pageContent.MockPositions;
        return this;
    }
}

internal class ExpectedPageContentDescriptor
{
    public List<LayoutTestResult.MockLayoutPosition> MockPositions { get;} = new();
    
    public ExpectedMockPositionDescriptor Mock(string mockId = MockFluent.DefaultMockId)
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

internal static class MockFluent
{
    public const string DefaultMockId = "$mock";
    
    public static MockDescriptor Mock(this IContainer element, string id = DefaultMockId)
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

internal static class WrapFluent
{
    public static void Wrap(this IContainer element)
    {
        element.Element(new WrapChild());
    } 
}