namespace QuestPDF.LayoutTests;

public class ShowIfTests
{
    [Test]
    public void Scenario()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().ShowIf(c => c.PageNumber % 2 == 0).Mock("before").Size(80, 20);
                    decoration.Content().Mock("content").ContinuousBlock(70, 460);
                    decoration.After().ShowIf(c => c.PageNumber % 3 == 0).Mock("after").Size(90, 30);
                });
            })
            .ExpectDrawResult(document =>
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
                        page.Mock("content").Position(0, 0).Size(90, 70);
                        page.Mock("after").Position(0, 70).Size(90, 30);
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
                    .RequiredAreaSize(70, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(70, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(90, 80)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(90, 20);
                        page.Mock("content").Position(0, 20).Size(90, 30);
                        page.Mock("after").Position(0, 50).Size(90, 30);
                    });
            });
    }

    // ShowIf(false) and ShowIf(predicate) hide content in the same way, the tests below check both

    [TestCase(false)]
    [TestCase(true)]
    public void ElementsBeforeShowIfKeepTheirSize(bool usePredicate)
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Mock("a").SolidBlock(50, 20);

                    // only the content after ShowIf is hidden, the height set before it stays
                    column.Item().Mock("box").Height(30).Hide(usePredicate).SolidBlock(50, 20);

                    column.Item().Mock("b").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 90)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 20);
                        page.Mock("box").Position(0, 30).Size(50, 30);
                        page.Mock("b").Position(0, 70).Size(50, 20);
                    });
            });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ShowIfPlacedFirstHidesAllElementsAfterIt(bool usePredicate)
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Mock("a").SolidBlock(50, 20);

                    // the hidden item takes no height, so it takes no spacing either
                    column.Item().Hide(usePredicate).Mock("box").Height(30).SolidBlock(50, 20);

                    column.Item().Mock("b").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 20);
                        page.Mock("b").Position(0, 30).Size(50, 20);
                    });
            });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ContainerReceivesContentFromVisibleBranch(bool condition)
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                // the same container is used in both branches of an if/else pair, the hidden branch must not take its place
                var container = content.Shrink();
                container.ShowIf(condition).Mock("a").SolidBlock(50, 20);
                container.ShowIf(!condition).Mock("b").SolidBlock(50, 30);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, condition ? 20 : 30)
                    .Content(page =>
                    {
                        if (condition)
                            page.Mock("a").Position(0, 0).Size(50, 20);
                        else
                            page.Mock("b").Position(0, 0).Size(50, 30);
                    });
            });
    }
}