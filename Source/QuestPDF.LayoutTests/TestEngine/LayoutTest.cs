using System.Runtime.CompilerServices;
using QuestPDF.Elements;

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

        try
        {
            LayoutTestValidator.Validate(TestResult);
        }
        catch
        {
            if (Settings.LayoutTestVisualizationStrategy != LayoutTestVisualizationStrategy.Never)
                GenerateTestPreview();
                
            throw;
        }
        finally
        {
            if (Settings.LayoutTestVisualizationStrategy == LayoutTestVisualizationStrategy.Always)
                GenerateTestPreview();
        }
    }

    private void GenerateTestPreview()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{TestIdentifier}.pdf");
        
        if (File.Exists(path))
            File.Delete(path);
        
        var stream = new FileStream(path, FileMode.CreateNew);
        LayoutTestResultVisualization.Visualize(TestResult, stream);
        stream.Dispose();
        
        Helpers.Helpers.OpenFileUsingDefaultProgram(path);
    }
}
