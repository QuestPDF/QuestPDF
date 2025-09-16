using System.Runtime.CompilerServices;
using System.Text;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests.TestEngine;

internal class LayoutTest
{
    private string TestIdentifier { get; set; }
    private Size AvailableSpace { get; set; }
    private DrawingRecorder ActualDrawingRecorder { get; } = new();
    private DrawingRecorder ExpectedDrawingRecorder { get; } = new();
    private IContainer? Content { get; set; }
  
    public static LayoutTest HavingSpaceOfSize(float width, float height, [CallerMemberName] string testIdentifier = "test")
    {
        var layoutTest = new LayoutTest
        {
            TestIdentifier = testIdentifier,
            AvailableSpace = new Size(width, height)
        };

        return layoutTest;
    }

    public LayoutTest ForContent(Action<IContainer> handler)
    {
        if (Content != null)
            throw new InvalidOperationException("Content has already been defined.");
        
        Content = new Container();
        
        Content
            .Width(AvailableSpace.Width)
            .Height(AvailableSpace.Height)
            .ElementObserverSetter(ActualDrawingRecorder)
            .Mock("$document")
            .Element(handler);
        
        return this;
    }

    public void ExpectDrawResult(Action<ExpectedDocumentLayoutDescriptor> handler)
    {
        if (!ActualDrawingRecorder.GetDrawingEvents().Any())
            PerformTest();
        
        var builder = new ExpectedDocumentLayoutDescriptor(ExpectedDrawingRecorder);
        handler(builder);

        var actualDrawingEvents = ActualDrawingRecorder.GetDrawingEvents();
        var expectedDrawingEvents = ExpectedDrawingRecorder.GetDrawingEvents();

        if (CheckIfIdentical(actualDrawingEvents, expectedDrawingEvents))
        {
            Assert.Pass();
        }
        else
        {
            DrawLog(actualDrawingEvents, expectedDrawingEvents);
            Assert.Fail($"The drawing operations do not match the expected result. See the log above for details. Test identifier: '{TestIdentifier}'.");
        }

        static bool CheckIfIdentical(IReadOnlyCollection<ElementDrawingEvent> actual, IReadOnlyCollection<ElementDrawingEvent> expected)
        {
            if (actual.Count != expected.Count)
                return false;

            return actual.Zip(expected, Compare).All(x => x);
        }

        static bool Compare(ElementDrawingEvent? actual, ElementDrawingEvent? expected)
        {
            if (actual == null && expected == null)
                return true;
            
            if (actual == null || expected == null)
                return false;
            
            return actual.ObserverId == expected.ObserverId &&
                   actual.PageNumber == expected.PageNumber &&
                   Position.Equal(actual.Position, expected.Position) &&
                   Size.Equal(actual.Size, expected.Size);
        }

        static void DrawLog(IReadOnlyCollection<ElementDrawingEvent> actualEvents, IReadOnlyCollection<ElementDrawingEvent> expectedEvents)
        {
            var identicalLines = actualEvents.Zip(expectedEvents, Compare).TakeWhile(x => x).Count();

            if (identicalLines > 0)
            {
                TestContext.Out.WriteLine("IDENTICAL");
                TestContext.Out.WriteLine(DrawHeader());
                
                foreach (var actualEvent in actualEvents.Take(identicalLines))
                    TestContext.Out.WriteLine($"🟩\t{GetEventAsText(actualEvent)}");
            }

            if (expectedEvents.Count > identicalLines)
            {
                TestContext.Out.WriteLine();
                TestContext.Out.WriteLine("EXPECTED");
                TestContext.Out.WriteLine(DrawHeader());
                
                foreach (var expectedEvent in expectedEvents.Skip(identicalLines))
                    TestContext.Out.WriteLine($"🟧\t{GetEventAsText(expectedEvent)}");   
            }

            if (actualEvents.Count > identicalLines)
            {
                TestContext.Out.WriteLine();
                TestContext.Out.WriteLine("ACTUAL");
                TestContext.Out.WriteLine(DrawHeader());
                
                foreach (var actualEvent in actualEvents.Skip(identicalLines))
                    TestContext.Out.WriteLine($"🟥\t{GetEventAsText(actualEvent)}");    
            }
        }

        static string DrawHeader()
        {
            var mock = "Mock".PadRight(12);
            var page = "Page".PadRight(6);
            var x = "X".PadRight(8);
            var y = "Y".PadRight(8);
            var width = "W".PadRight(10);
            var height = "H";
            
            return $"\t{mock} {page} {x} {y} {width} {height}";      
        }
        
        static string GetEventAsText(ElementDrawingEvent drawingEvent)
        {
            var observerId = drawingEvent.ObserverId.PadRight(12);
            var pageNumber = $"{drawingEvent.PageNumber}".PadRight(6);
            
            var positionX = $"{drawingEvent.Position.X}".PadRight(8);
            var positionY = $"{drawingEvent.Position.Y}".PadRight(8);
            
            var sizeWidth = $"{drawingEvent.Size.Width}".PadRight(10);
            var sizeHeight = $"{drawingEvent.Size.Height}";
            
            return $"{observerId} {pageNumber} {positionX} {positionY} {sizeWidth} {sizeHeight}";       
        }
    }

    public void ExpectLayoutException(string? reason = null)
    {
        try
        {
            QuestPDF.Settings.EnableDebugging = true;
            PerformTest();
        }
        catch (DocumentLayoutException e)
        {
            Assert.That(e.Message.Contains(reason));
            Assert.Pass($"The expected exception was thrown: {e.Message}");
        }
        catch
        {
            Assert.Fail("Un expected exception was thrown.");
        }
    }

    private void PerformTest()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(Size.Infinity, Size.Infinity));
                    page.Content().Element(Content);
                });
            })
            .GenerateAndDiscard();
    }

    public LayoutTest VisualizeOutput()
    {
        if (Content == null)
            throw new InvalidOperationException("Content has not been defined.");
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(Size.Infinity, Size.Infinity));
                    page.Content().Element(Content);
                });
            })
            .GeneratePdfAndShow();
        
        return this;
    }
}