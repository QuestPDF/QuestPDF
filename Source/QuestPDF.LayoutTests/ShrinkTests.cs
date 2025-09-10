namespace QuestPDF.LayoutTests;

public class ShrinkTests
{
    [Test]
    public void Both()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .Mock().ContinuousBlock(60, 200);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 120)
                    .Content(page =>
                    {
                        page.Mock().Position(0, 0).Size(60, 120);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 80)
                    .Content(page =>
                    {
                        page.Mock().Position(0, 0).Size(60, 80);
                    });
            });
    }
    
    [Test]
    public void Vertical()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content
                    .ShrinkVertical()
                    .Mock().ContinuousBlock(60, 200);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 120)
                    .Content(page =>
                    {
                        page.Mock().Position(0, 0).Size(100, 120);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 80)
                    .Content(page =>
                    {
                        page.Mock().Position(0, 0).Size(100, 80);
                    });
            });
    }
    
    [Test]
    public void Horizontal()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content
                    .ShrinkHorizontal()
                    .Mock().ContinuousBlock(60, 200);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 120)
                    .Content(page =>
                    {
                        page.Mock().Position(0, 0).Size(60, 120);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 80)
                    .Content(page =>
                    {
                        page.Mock().Position(0, 0).Size(60, 120);
                    });
            });
    }
    
    [Test]
    public void ContentFromRightToLeft()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content
                    .ContentFromRightToLeft()
                    .Shrink()
                    .Mock().ContinuousBlock(60, 200);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 120)
                    .Content(page =>
                    {
                        page.Mock().Position(40, 0).Size(60, 120);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 80)
                    .Content(page =>
                    {
                        page.Mock().Position(40, 0).Size(60, 80);
                    });
            });
    }
}