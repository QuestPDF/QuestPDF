namespace QuestPDF.LayoutTests;

public class TranslateTests
{
    [Test]
    public void HorizontalTranslation()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content.Shrink().TranslateX(15).Mock("a").SolidBlock(40, 50);
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
    public void VerticalTranslation()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content.Shrink().TranslateY(25).Mock("a").SolidBlock(30, 40);
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
    public void MultipleItemsWithTranslation()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 120)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().TranslateX(5).TranslateY(10).Mock("a").SolidBlock(40, 20);
                    column.Item().TranslateX(-10).TranslateY(20).Mock("b").SolidBlock(30, 25);
                    column.Item().TranslateX(30).TranslateY(-15).Mock("c").SolidBlock(50, 15);
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