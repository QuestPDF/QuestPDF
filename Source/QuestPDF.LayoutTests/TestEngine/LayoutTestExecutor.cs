using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class LayoutTestExecutor
{
    public static ICollection<LayoutTestResult.PageLayoutSnapshot> Execute(Size pageSize, Container container)
    {
        var pageSizes = new List<Size>();
        GenerateDocument();
        return CollectMockInformation();

        void GenerateDocument()
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
        
            while(true)
            {
                var spacePlan = container.Measure(pageSize);
                pageSizes.Add(spacePlan);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
                    canvas.EndDocument();
                    throw new LayoutTestException("Provided layout generates infinite document");
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
                    throw new LayoutTestException("Exception occured during layout execution", exception);
                }

                canvas.EndPage();

                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;
            }
        }

        ICollection<LayoutTestResult.PageLayoutSnapshot> CollectMockInformation()
        {
            // mock cannot contain another mock, flat structure
            var mocks = container.ExtractElementsOfType<ElementMock>().Select(x => x.Value); 

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
    }
}