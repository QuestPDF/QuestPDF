namespace QuestPDF.LayoutTests;

public class OffsetTests
{
    [Test]
    public void HorizontalOffset()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content.Shrink().OffsetX(15).Mock("a").SolidBlock(40, 50);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(40, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(15, 0).Size(40, 50);
                    });
            });
    }

    [Test]
    public void VerticalOffset()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content.Shrink().OffsetY(25).Mock("a").SolidBlock(30, 40);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(30, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 25).Size(30, 40);
                    });
            });
    }

    [Test]
    public void MultipleItemsWithOffset()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().OffsetX(5).OffsetY(10).Mock("a").SolidBlock(40, 20);
                    column.Item().OffsetX(-10).OffsetY(20).Mock("b").SolidBlock(30, 25);
                    column.Item().OffsetX(30).OffsetY(-15).Mock("c").SolidBlock(50, 15);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(5, 10).Size(50, 20);
                        page.Mock("b").Position(-10, 40).Size(50, 25);
                        page.Mock("c").Position(30, 30).Size(50, 15);
                    });
            });
    }
}
