namespace QuestPDF.LayoutTests;

public class StopPagingTests
{
    [Test]
    public void ChildReturnsWrap()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink()
                    .Mock("a")
                    .StopPaging() // <- 
                    .Mock("b")
                    .SolidBlock(200, 200);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 0)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(0, 0);
                    });
            });
    }
    
    [Test]
    public void ChildReturnsPartialRender()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink()
                    .Mock("a")
                    .StopPaging() // <-
                    .Mock("b")
                    .ContinuousBlock(50, 150);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 100);
                        page.Mock("b").Position(0, 0).Size(50, 100);
                    });
                
                // remaining item space is ignored
            });
    }
    
    [Test]
    public void ChildReturnsFullRender()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink()
                    .Mock("a")
                    .StopPaging() // <-
                    .Mock("b")
                    .SolidBlock(50, 50);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 50);
                        page.Mock("b").Position(0, 0).Size(50, 50);
                    });
            });
    }
    
    [Test]
    public void ChildReturnsEmpty()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink()
                    .Mock("a")
                    .StopPaging() // <-
                    .Mock("b");
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 0)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(0, 0);
                        page.Mock("b").Position(0, 0).Size(0, 0);
                    });
            });
    }
}