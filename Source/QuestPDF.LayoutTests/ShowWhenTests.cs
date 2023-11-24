namespace QuestPDF.LayoutTests;

public class ShowWhenTests
{
    [Test]
    public void Scenario()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().ShowWhen(c => c.PageNumber % 2 == 0).Mock("before").Size(80, 20);
                    decoration.Content().Mock("content").Size(70, 400);
                    decoration.After().ShowWhen(c => c.PageNumber % 3 == 0).Mock("after").Size(90, 30);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(70, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(80, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(80, 20);
                        page.Mock("content").Position(0, 20).Size(80, 80);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(90, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(90, 20);
                        page.Mock("content").Position(0, 20).Size(90, 50);
                        page.Mock("after").Position(0, 70).Size(90, 30);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(80, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(60, 120);
                        page.Mock("content").Position(0, 0).Size(60, 120);
                        page.Mock("after").Position(0, 0).Size(60, 120);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(60, 120);
                        page.Mock("content").Position(0, 0).Size(60, 120);
                        page.Mock("after").Position(0, 0).Size(60, 120);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(90, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(60, 120);
                        page.Mock("content").Position(0, 0).Size(60, 120);
                        page.Mock("after").Position(0, 0).Size(60, 120);
                    });
            });
    }
}