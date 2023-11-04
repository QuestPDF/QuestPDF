using System.Collections;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.LayoutTests.TestEngine;

internal class LayoutBuilderDescriptor
{
    public void Compose(Action<IContainer> container)
    {
        
    }
}

internal class DocumentLayoutBuilder
{
    public List<LayoutTestResult.PageLayoutSnapshot> Commands { get; } = new(); 
    
    public PageLayoutDescriptor Page()
    {
        var page = new LayoutTestResult.PageLayoutSnapshot();
        Commands.Add(page);
        return new PageLayoutDescriptor(page);
    }
}

internal class PageLayoutDescriptor
{
    private LayoutTestResult.PageLayoutSnapshot Command { get; }

    public PageLayoutDescriptor(LayoutTestResult.PageLayoutSnapshot command)
    {
        Command = command;
    }

    public PageLayoutDescriptor TakenAreaSize(float width, float height)
    {
        Command.RequiredArea = new Size(width, height);
        return this;
    }
    
    public PageLayoutDescriptor Content(Action<PageLayoutBuilder> content)
    {
        var pageContent = new PageLayoutBuilder();
        content(pageContent);
        Command.MockPositions = pageContent.Commands;
        return this;
    }
}

internal class PageLayoutBuilder
{
    public List<LayoutTestResult.MockLayoutPosition> Commands { get;} = new();
    
    public ChildLayoutDescriptor Child(string mockId)
    {
        var child = new LayoutTestResult.MockLayoutPosition { MockId = mockId };
        Commands.Add(child);
        return new ChildLayoutDescriptor(child);
    }
}

internal class ChildLayoutDescriptor
{
    private LayoutTestResult.MockLayoutPosition Command { get; }

    public ChildLayoutDescriptor(LayoutTestResult.MockLayoutPosition command)
    {
        Command = command;
    }

    public ChildLayoutDescriptor Position(float x, float y)
    {
        Command.Position = new Position(x, y);
        return this;
    }
    
    public ChildLayoutDescriptor Size(float width, float height)
    {
        Command.Size = new Size(width, height);
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

internal class LayoutTest
{
    private LayoutTestResult TestResult { get; } = new LayoutTestResult();
  
    public static LayoutTest HavingSpaceOfSize(float width, float height)
    {
        var result = new LayoutTest();
        result.TestResult.PageSize = new Size(width, height);
        return result;
    }

    public LayoutTest WithContent(Action<IContainer> handler)
    {
        // compose content
        var container = new Container();
        container.Element(handler);

        TestResult.ActualLayout = LayoutTestExecutor.Execute(TestResult.PageSize, container);
        
        return this;
    }
    
    public void ExpectWrap()
    {
        
    }
    
    public LayoutTest ExpectedDrawResult(Action<DocumentLayoutBuilder> handler)
    {
        var builder = new DocumentLayoutBuilder();
        handler(builder);

        TestResult.ExpectedLayout = builder.Commands;
        return this;
    }

    public void CompareVisually()
    {
        var path = "output.pdf";
        
        if (File.Exists(path))
            File.Delete(path);
        
        var stream = new FileStream(path, FileMode.CreateNew);
        LayoutTestResultVisualization.Visualize(TestResult, stream);
        stream.Dispose();
        
        GenerateExtensions.OpenFileUsingDefaultProgram(path);
    }

    public void Validate()
    {
        LayoutTestValidator.Validate(TestResult);
    }
}
