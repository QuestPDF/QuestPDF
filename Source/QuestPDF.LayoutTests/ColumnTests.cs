namespace QuestPDF.LayoutTests;

public class ColumnTests
{
    [Test]
    public void Typical()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 140)
            .WithContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").Size(50, 30);
                    column.Item().Mock("b").Size(40, 20);
                    column.Item().Mock("c").Size(70, 40);
                    column.Item().Mock("d").Size(60, 60);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 140)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(70, 30);
                        page.Mock("b").Position(0, 40).Size(70, 20);
                        page.Mock("c").Position(0, 70).Size(70, 40);
                        page.Mock("d").Position(0, 120).Size(70, 20);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 40)
                    .Content(page =>
                    {
                        page.Mock("d").Position(0, 0).Size(60, 40);
                    });
            });
    }
}