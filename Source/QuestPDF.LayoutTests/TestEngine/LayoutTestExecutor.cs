using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal static class LayoutTestExecutor
{
    public static LayoutTestResult.DocumentLayout Execute(Size pageSize, Container container)
    {
        var (pageSizes, generatesInfiniteLayout) = GenerateDocument();

        return new LayoutTestResult.DocumentLayout
        {
            Pages = CollectMockInformation(pageSizes),
            GeneratesInfiniteLayout = generatesInfiniteLayout
        };

        (List<Size> pageSizes, bool generatesInfiniteLayout) GenerateDocument()
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
                pageContext.IncrementPageNumber();
              
                var spacePlan = container.Measure(pageSize);
                pageSizes.Add(spacePlan);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
                    pageContext.DecrementPageNumber();
                    canvas.EndDocument();
                    return (pageSizes, true);
                }

                try
                {
                    canvas.BeginPage(pageSize);
                    container.Draw(pageSize);
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

            return (pageSizes, false);
        }

        ICollection<LayoutTestResult.PageLayout> CollectMockInformation(ICollection<Size> pageSizes)
        {
            // mock cannot contain another mock, flat structure
            var mocks = container.ExtractElementsOfType<ElementMock>().Select(x => x.Value); 

            return mocks
                .SelectMany(x => x.DrawingCommands)
                .GroupBy(x => x.PageNumber)
                .OrderBy(x => x.Key)
                .Select(x => new LayoutTestResult.PageLayout
                {
                    RequiredArea = pageSizes.ElementAt(x.Key - 1),
                    Mocks = x
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