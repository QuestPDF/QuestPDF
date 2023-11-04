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
    public List<PageDrawingCommand> Commands { get; } = new(); 
    
    public PageLayoutDescriptor Page()
    {
        var page = new PageDrawingCommand();
        Commands.Add(page);
        return new PageLayoutDescriptor(page);
    }
}

internal class PageLayoutDescriptor
{
    private PageDrawingCommand Command { get; }

    public PageLayoutDescriptor(PageDrawingCommand command)
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
        Command.Children = pageContent.Commands;
        return this;
    }
}

internal class PageLayoutBuilder
{
    public List<ChildDrawingCommand> Commands { get;} = new();
    
    public ChildLayoutDescriptor Child(string childId)
    {
        var child = new ChildDrawingCommand { ChildId = childId };
        Commands.Add(child);
        return new ChildLayoutDescriptor(child);
    }
}

internal class ChildLayoutDescriptor
{
    private ChildDrawingCommand Command { get; }

    public ChildLayoutDescriptor(ChildDrawingCommand command)
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

internal class ExpectedLayoutChildPosition
{
    public string ElementId { get; set; }
    public int PageNumber { get; set; }
    public int DepthIndex { get; set; }
    public Position Position { get; set; }
    public Size Size { get; set; }
}

public static class ElementExtensions
{
    public static void Mock(this IContainer element, string id, float width, float height)
    {
        var mock = new MockChild
        {
            Id = id,
            TotalWidth = width,
            TotalHeight = height
        };
        
        element.Element(mock);
    } 
}

internal class LayoutTest
{
    private const string DocumentColor = Colors.Grey.Lighten1;
    private const string PageColor = Colors.Grey.Lighten3;
    private const string TargetColor = Colors.White;
    
    public Size PageSize { get; set; }
    public ICollection<PageDrawingCommand> ActualCommands { get; set; }
    public ICollection<PageDrawingCommand> ExpectedCommands { get; set; }
    
    public static LayoutTest HavingSpaceOfSize(float width, float height)
    {
        return new LayoutTest
        {
            PageSize = new Size(width, height)
        };
    }

    public LayoutTest WithContent(Action<IContainer> handler)
    {
        // compose content
        var container = new Container();
        container.Element(handler);

        ActualCommands = GenerateResult(PageSize, container);
        
        return this;
    }

    private static ICollection<PageDrawingCommand> GenerateResult(Size pageSize, Container container)
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
        var mocks = container.ExtractElementsOfType<MockChild>().Select(x => x.Value); // mock cannot contain another mock, flat structure

        return mocks
            .SelectMany(x => x.DrawingCommands)
            .GroupBy(x => x.PageNumber)
            .Select(x => new PageDrawingCommand
            {
                RequiredArea = pageSizes[x.Key - 1],
                Children = x
                    .Select(y => new ChildDrawingCommand
                    {
                        ChildId = y.ChildId,
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

        ExpectedCommands = builder.Commands;
        return this;
    }

    public void CompareVisually()
    {
        VisualizeExpectedResult(PageSize, ActualCommands, ExpectedCommands);
    }

    public void Validate()
    {
        if (ActualCommands.Count != ExpectedCommands.Count)
            throw new Exception($"Generated {ActualCommands.Count} but expected {ExpectedCommands.Count} pages.");

        var numberOfPages = ActualCommands.Count;
        
        foreach (var i in Enumerable.Range(0, numberOfPages))
        {
            try
            {
                var actual = ActualCommands.ElementAt(i);
                var expected = ExpectedCommands.ElementAt(i);

                if (Math.Abs(actual.RequiredArea.Width - expected.RequiredArea.Width) > Size.Epsilon)
                    throw new Exception($"Taken area width is equal to {actual.RequiredArea.Width} but expected {expected.RequiredArea.Width}");
                
                if (Math.Abs(actual.RequiredArea.Height - expected.RequiredArea.Height) > Size.Epsilon)
                    throw new Exception($"Taken area height is equal to {actual.RequiredArea.Height} but expected {expected.RequiredArea.Height}");
                
                if (actual.Children.Count != expected.Children.Count)
                    throw new Exception($"Visible {actual.Children.Count} but expected {expected.Children.Count}");

                foreach (var child in expected.Children)
                {
                    var matchingActualElements = actual
                        .Children
                        .Where(x => x.ChildId == child.ChildId)
                        .Where(x => Position.Equal(x.Position, child.Position))
                        .Where(x => Size.Equal(x.Size, child.Size))
                        .Count();

                    if (matchingActualElements == 0)
                        throw new Exception($"Cannot find actual drawing command for child {child.ChildId} on position {child.Position} and size {child.Size}");
                    
                    if (matchingActualElements > 1)
                        throw new Exception($"Found multiple drawing commands for child {child.ChildId} on position {child.Position} and size {child.Size}");
                }
                
                // todo: add z-depth testing
                var actualOverlaps = GetOverlappingItems(actual.Children);
                var expectedOverlaps = GetOverlappingItems(expected.Children);
                
                foreach (var overlap in expectedOverlaps)
                {
                    var matchingActualElements = actualOverlaps.Count(x => x.Item1 == overlap.Item1 && x.Item2 == overlap.Item2);

                    if (matchingActualElements != 1)
                        throw new Exception($"Element {overlap.Item1} should be visible underneath element {overlap.Item2}");
                }
                
                IEnumerable<(string, string)> GetOverlappingItems(ICollection<ChildDrawingCommand> items)
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

                            yield return (beforeChild.ChildId, afterChild.ChildId);
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
        
    private static void VisualizeExpectedResult(Size pageSize, ICollection<PageDrawingCommand> left, ICollection<PageDrawingCommand> right)
    {
        var path = "test.pdf";
        
        if (File.Exists(path))
            File.Delete(path);
        
        // default colors
        var defaultColors = new string[]
        {
            Colors.Red.Medium,
            Colors.Green.Medium,
            Colors.Blue.Medium,
            Colors.Pink.Medium,
            Colors.Orange.Medium,
            Colors.Lime.Medium,
            Colors.Cyan.Medium,
            Colors.Indigo.Medium
        };
        
        // determine children colors
        var children = Enumerable
            .Concat(left, right)
            .SelectMany(x => x.Children)
            .Select(x => x.ChildId)
            .Distinct()
            .ToList();

        var colors = Enumerable
            .Range(0, children.Count)
            .ToDictionary(i => children[i], i => defaultColors[i]);

        // create new pdf document output
        var matrixHeight = Math.Max(left.Count, right.Count);
        
        const int pagePadding = 25;
        var imageInfo = new SKImageInfo((int)pageSize.Width * 2 + pagePadding * 4, (int)(pageSize.Height * matrixHeight + pagePadding * (matrixHeight + 2)));

        using var pdf = SKDocument.CreatePdf(path);
        using var canvas = pdf.BeginPage(imageInfo.Width, imageInfo.Height);
        
        // page background
        canvas.Clear(SKColor.Parse(DocumentColor));
        
        // chain titles
        
        // available area
        using var titlePaint = TextStyle.LibraryDefault.FontSize(16).Bold().ToPaint().Clone();
        titlePaint.TextAlign = SKTextAlign.Center;

        canvas.Save();
        
        canvas.Translate(pagePadding + pageSize.Width / 2f, pagePadding + titlePaint.TextSize / 2);
        canvas.DrawText("RESULT", 0, 0, titlePaint);
        
        canvas.Translate(pagePadding * 2 + pageSize.Width, 0);
        canvas.DrawText("EXPECTED", 0, 0, titlePaint);
        
        canvas.Restore();

        // side visualization
        canvas.Save();
        
        canvas.Translate(pagePadding, pagePadding * 2);
        DrawSide(left);
        
        canvas.Translate(pageSize.Width + pagePadding * 2, 0);
        DrawSide(right);
        
        canvas.Restore();
        
        
        // draw page numbers
        canvas.Save();
        
        canvas.Translate(pagePadding * 2 + pageSize.Width, pagePadding * 2 + titlePaint.TextSize);
        
        foreach (var i in Enumerable.Range(0, matrixHeight))
        {
            canvas.DrawText((i + 1).ToString(), 0, 0, titlePaint);
            canvas.Translate(0, pagePadding + pageSize.Height);
        }
        
        canvas.Restore();

        pdf.EndPage();
        pdf.Close();
        
        void DrawSide(ICollection<PageDrawingCommand> commands)
        {
            canvas.Save();
            
            foreach (var pageDrawingCommand in commands)
            {
                DrawPage(pageDrawingCommand);
                canvas.Translate(0, pageSize.Height + pagePadding);
            }
            
            canvas.Restore();
        }

        void DrawPage(PageDrawingCommand command)
        {
            // available area
            using var availableAreaPaint = new SKPaint
            {
                Color = SKColor.Parse(PageColor)
            };
            
            canvas.DrawRect(0, 0, pageSize.Width, pageSize.Height, availableAreaPaint);
            
            // taken area
            using var takenAreaPaint = new SKPaint
            {
                Color = SKColor.Parse(TargetColor)
            };
            
            canvas.DrawRect(0, 0, command.RequiredArea.Width, command.RequiredArea.Height, takenAreaPaint);
        
            // draw children
            foreach (var child in command.Children)
            {
                canvas.Save();

                var color = colors[child.ChildId];
            
                using var childBorderPaint = new SKPaint
                {
                    Color = SKColor.Parse(color),
                    IsStroke = true,
                    StrokeWidth = 2
                };
            
                using var childAreaPaint = new SKPaint
                {
                    Color = SKColor.Parse(color).WithAlpha(128)
                };
            
                canvas.Translate(child.Position.X, child.Position.Y);
                canvas.DrawRect(0, 0, child.Size.Width, child.Size.Height, childAreaPaint);
                canvas.DrawRect(0, 0, child.Size.Width, child.Size.Height, childBorderPaint);
            
                canvas.Restore();
            }
        }
        
        // save
        GenerateExtensions.OpenFileUsingDefaultProgram(path);
    }
}
