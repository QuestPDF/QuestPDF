using System.Diagnostics;
using System.Runtime.CompilerServices;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal sealed class LayoutTest
{
    private string TestIdentifier { get; set; }
    private LayoutTestResult TestResult { get; } = new LayoutTestResult();
  
    public static LayoutTest HavingSpaceOfSize(float width, float height, [CallerMemberName] string testIdentifier = "test")
    {
        var layoutTest = new LayoutTest
        {
            TestIdentifier = testIdentifier,
            
            TestResult =
            {
                PageSize = new Size(width, height)
            }
        };

        return layoutTest;
    }

    public LayoutTest WithContent(Action<IContainer> handler)
    {
        var container = new Container();
        container.Element(handler);

        TestResult.ActualLayout = LayoutTestExecutor.Execute(TestResult.PageSize, container);
        
        return this;
    }

    public void ExpectedDrawResult(Action<ExpectedDocumentLayoutDescriptor> handler)
    {
        var builder = new ExpectedDocumentLayoutDescriptor();
        handler(builder);

        TestResult.ExpectedLayout = builder.DocumentLayout;

        GenerateTestPreview();
        LayoutTestValidator.Validate(TestResult);
    }

    private void GenerateTestPreview()
    {
        if (!Debugger.IsAttached)
        {
            Console.WriteLine("Debugger is not attached. Skipping test preview generation");
            return;
        }
        
        var path = Path.Combine(Path.GetTempPath(), $"{TestIdentifier}.pdf");
        
        if (File.Exists(path))
            File.Delete(path);
        
        var stream = new FileStream(path, FileMode.CreateNew);
        LayoutTestResultVisualization.Visualize(TestResult, stream);
        stream.Dispose();
        
        Console.WriteLine($"Generated test case preview: {path}");
    }
}
