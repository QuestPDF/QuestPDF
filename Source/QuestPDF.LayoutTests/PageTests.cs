namespace QuestPDF.LayoutTests;

public class PageTests
{
    [Test]
    public void GenerationWorks_WhenPageSizeIsZero_AndUsesSizePerPdfSpecification()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.ContinuousSize(100);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 3);
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
