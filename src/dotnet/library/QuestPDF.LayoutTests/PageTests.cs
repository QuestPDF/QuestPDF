using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests;

public class PageTests
{
    // TODO: adjust and refactor layout tests for the Page element:
    // there are two types of Empty content:
    // 1) SpacePlan.Empty, e.g. when Column has nothing more to render
    // 2) SpacePlan.FullRender(0, 0), used by default in practically all element slots (most common when the IContainer chain is left open)
    // these tests should properly communicate which type of "empty" is used
    
    [Test]
    public void WhenContentSizeIsEmpty_AppliesMinimalPageSizePerPdfSpecification()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(0, 0));
                page.MaxSize(new PageSize(100, 100));
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(3, 3);
            });
    }
    
    [Test]
    public void WhenContentSizeIsEmpty_AppliesMinimalPageSizePerPdfSpecification2()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(0, 0));
                page.MaxSize(new PageSize(100, 100));
                
                page.Content().Column(_ => { });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(3, 3);
            });
    }
    
    [Test]
    public void WhenContentIsEmpty_AppliesMinimumFlexiblePageSize()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(200, 300));
                page.MaxSize(new PageSize(400, 500));
                page.Content().Column(_ => { });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300);
            });
    }
    
    [Test]
    public void GenerationWorks_WhenPageSizeIsEmpty_AndUsesSizePerPdfSpecification()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.Size(200, 300);
                page.Content().Column(_ => { });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300);
            });
    }
    
    [Test]
    public void GenerationWorks_WhenContentIsEmpty()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.Size(200, 300);
                page.Content().Mock("content");
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(200, 300);
                    });
            });
    }
    
    [Test]
    public void Header_WhenContentIsEmpty()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.Size(200, 300);

                page.Header()
                    .Mock("header")
                    .Height(50)
                    .SolidBlock();
                
                page.Content().Mock("content");
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300)
                    .Content(page =>
                    {
                        page.Mock("header").Position(0, 0).Size(200, 50);
                        page.Mock("content").Position(0, 50).Size(200, 250);
                    });
            });
    }
    
    [Test]
    public void Footer_WhenContentIsEmpty()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.Size(200, 300);
                
                page.Content().Mock("content");
                
                page.Footer()
                    .Mock("footer")
                    .Height(50)
                    .SolidBlock();
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(200, 250);
                        page.Mock("footer").Position(0, 250).Size(200, 50);
                    });
            });
    }
    
    [Test]
    public void Footer_WhenContentIsEmpty2()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.Size(200, 300);
                
                page.Content().Mock("content").Column(_ => { });
                
                page.Footer()
                    .Height(50)
                    .ExtendHorizontal()
                    .Mock("footer")
                    .SolidBlock();
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(200, 250);
                        page.Mock("footer").Position(0, 250).Size(200, 50);
                    });
            });
    }
}
