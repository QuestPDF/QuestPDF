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

        TestResult.GeneratedLayout = GenerateResult(TestResult.PageSize, container);
        
        return this;
    }

    private static ICollection<LayoutTestResult.PageLayoutSnapshot> GenerateResult(Size pageSize, Container container)
    {
        // inject dependencies
        var pageContext = new PageContext();
        pageContext.ResetPageNumber();

        var canvas = new PreviewerCanvas();
        
        container.InjectDependencies(pageContext, canvas);
        
        // distribute global state
        container.ApplyInheritedAndGlobalTexStyle(TextStyle.Default);
        container.ApplyContentDirection(ContentDirection.LeftToRight);
        container.ApplyDefaultImageConfiguration(DocumentSettings.Default.ImageRasterDpi, DocumentSettings.Default.ImageCompressionQuality, true);
        
        // render
        container.VisitChildren(x => (x as IStateResettable)?.ResetState());
        
        canvas.BeginDocument();

        var pageSizes = new List<Size>();
        
        while(true)
        {
            var spacePlan = container.Measure(pageSize);
            pageSizes.Add(spacePlan);
            
            if (spacePlan.Type == SpacePlanType.Wrap)
            {
                throw new Exception();
            }

            try
            {
                canvas.BeginPage(pageSize);
                container.Draw(pageSize);
                
                pageContext.IncrementPageNumber();
            }
            catch (Exception exception)
            {
                canvas.EndDocument();
                throw new Exception();
            }

            canvas.EndPage();

            if (spacePlan.Type == SpacePlanType.FullRender)
                break;
        }
        
        // extract results
        var mocks = container.ExtractElementsOfType<ElementMock>().Select(x => x.Value); // mock cannot contain another mock, flat structure

        return mocks
            .SelectMany(x => x.DrawingCommands)
            .GroupBy(x => x.PageNumber)
            .Select(x => new LayoutTestResult.PageLayoutSnapshot
            {
                RequiredArea = pageSizes[x.Key - 1],
                MockPositions = x
                    .Select(y => new LayoutTestResult.MockLayoutPosition
                    {
                        MockId = y.MockId,
                        Size = y.Size,
                        Position = y.Position
                    })
                    .ToList()
            })
            .ToList();
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
        if (TestResult.GeneratedLayout.Count != TestResult.ExpectedLayout.Count)
            throw new Exception($"Generated {TestResult.GeneratedLayout.Count} but expected {TestResult.ExpectedLayout.Count} pages.");

        var numberOfPages = TestResult.GeneratedLayout.Count;
        
        foreach (var i in Enumerable.Range(0, numberOfPages))
        {
            try
            {
                var actual = TestResult.GeneratedLayout.ElementAt(i);
                var expected = TestResult.ExpectedLayout.ElementAt(i);

                if (Math.Abs(actual.RequiredArea.Width - expected.RequiredArea.Width) > Size.Epsilon)
                    throw new Exception($"Taken area width is equal to {actual.RequiredArea.Width} but expected {expected.RequiredArea.Width}");
                
                if (Math.Abs(actual.RequiredArea.Height - expected.RequiredArea.Height) > Size.Epsilon)
                    throw new Exception($"Taken area height is equal to {actual.RequiredArea.Height} but expected {expected.RequiredArea.Height}");
                
                if (actual.MockPositions.Count != expected.MockPositions.Count)
                    throw new Exception($"Visible {actual.MockPositions.Count} but expected {expected.MockPositions.Count}");

                foreach (var child in expected.MockPositions)
                {
                    var matchingActualElements = actual
                        .MockPositions
                        .Where(x => x.MockId == child.MockId)
                        .Where(x => Position.Equal(x.Position, child.Position))
                        .Where(x => Size.Equal(x.Size, child.Size))
                        .Count();

                    if (matchingActualElements == 0)
                        throw new Exception($"Cannot find actual drawing command for child {child.MockId} on position {child.Position} and size {child.Size}");
                    
                    if (matchingActualElements > 1)
                        throw new Exception($"Found multiple drawing commands for child {child.MockId} on position {child.Position} and size {child.Size}");
                }
                
                // todo: add z-depth testing
                var actualOverlaps = GetOverlappingItems(actual.MockPositions);
                var expectedOverlaps = GetOverlappingItems(expected.MockPositions);
                
                foreach (var overlap in expectedOverlaps)
                {
                    var matchingActualElements = actualOverlaps.Count(x => x.Item1 == overlap.Item1 && x.Item2 == overlap.Item2);

                    if (matchingActualElements != 1)
                        throw new Exception($"Element {overlap.Item1} should be visible underneath element {overlap.Item2}");
                }
                
                IEnumerable<(string, string)> GetOverlappingItems(ICollection<LayoutTestResult.MockLayoutPosition> items)
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        for (var j = i; j < items.Count; j++)
                        {
                            var beforeChild = items.ElementAt(i);
                            var afterChild = items.ElementAt(j);

                            var beforeBoundingBox = BoundingBox.From(beforeChild.Position, beforeChild.Size);
                            var afterBoundingBox = BoundingBox.From(afterChild.Position, afterChild.Size);

                            var intersection = BoundingBoxExtensions.Intersection(beforeBoundingBox, afterBoundingBox);
                            
                            if (intersection == null)
                                continue;

                            yield return (beforeChild.MockId, afterChild.MockId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error on page {i + 1}: {e.Message}");
            }
        }
    }
}
