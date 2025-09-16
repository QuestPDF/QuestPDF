namespace QuestPDF.LayoutTests;

[TestFixture]
public class PaddingTests
{
    [Test]
    public void PaddingModifiesPositioningAndMinimumSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .PaddingLeft(5)
                    .PaddingTop(10)
                    .PaddingRight(15)
                    .PaddingBottom(20)
                    .Mock("a")
                    .SolidBlock(20, 30);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(40, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(5, 10).Size(20, 30);
                    });
            });
    }
    
    [Test]
    public void NegativePaddingIsAllowed()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .Padding(-10)
                    .Mock("a")
                    .SolidBlock(50, 70);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(30, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(-10, -10).Size(50, 70);
                    });
            });
    }
    
    [Test]
    public void PaddingSupportsPaging()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .Padding(15)
                    .Mock("a")
                    .ContinuousBlock(50, 90);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(80, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(15, 15).Size(50, 70);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(80, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(15, 15).Size(50, 20);
                    });
            });
    }
    
    [Test]
    public void MultipleItemsWithAppliedPadding()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 150)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().PaddingVertical(5).Mock("a").SolidBlock(15, 25);
                    column.Item().PaddingHorizontal(10).Mock("b").SolidBlock(20, 30);
                    column.Item().Padding(15).Mock("c").SolidBlock(25, 35);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(55, 130)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 5).Size(55, 25);
                        page.Mock("b").Position(10, 35).Size(35, 30);
                        page.Mock("c").Position(15, 80).Size(25, 35);
                    });
            });
    }
    
    [Test]
    public void PaddingProducesAvailableSpaceOfNegativeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 150)
            .ForContent(content =>
            {
                content.Shrink().Padding(60).SolidBlock(20, 25);
            })
            .ExpectLayoutException("The available space is negative.");
    }
    
    [Test]
    public void PaddingWithEmptyChild()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 150)
            .ForContent(content =>
            {
                content.Shrink().Padding(30);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 60);
            });
    }
    
    [Test]
    public void PaddingOnEmptyElementProducesAvailableSpaceOfNegativeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 150)
            .ForContent(content =>
            {
                content.Shrink().Padding(60);
            })
            .ExpectLayoutException("The available space is negative.");
    }
    
    [Test]
    public void PaddingOnEmptyElementProducesAvailableSpaceOfNegativeSize2()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 150)
            .ForContent(content =>
            {
                content.Shrink().Padding(60).Column(column => { });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 0);
            });
    }
    
    [Test]
    public void NegativePaddingProducesMeasurementOfNegativeSize()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 150)
            .ForContent(content =>
            {
                content.Shrink().Padding(-15).Mock("a").SolidBlock(20, 40);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 10)
                    .Content(page =>
                    {
                        page.Mock("a").Position(-15, -15).Size(30, 40);
                    });
            });
    }
}