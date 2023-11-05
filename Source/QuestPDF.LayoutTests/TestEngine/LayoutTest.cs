using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal sealed class LayoutTest
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

    public LayoutTest ExpectedDrawResult(Action<ExpectedDocumentLayoutDescriptor> handler)
    {
        var builder = new ExpectedDocumentLayoutDescriptor();
        handler(builder);

        TestResult.ExpectedLayout = builder.DocumentLayout;
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
