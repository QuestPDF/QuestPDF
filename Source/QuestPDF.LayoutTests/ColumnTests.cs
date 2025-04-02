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

    [Test]
    public void SingleItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Mock("a").Size(50, 30);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                    });
            });
    }

    [Test]
    public void ZeroHeightItemDoesNotConsumeSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").Size(50, 30);
                    column.Item().Mock("b").Size(50, 0);
                    column.Item().Mock("c").Size(70, 20);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(70, 30);
                        page.Mock("b").Position(0, 30).Size(70, 0);
                        page.Mock("c").Position(0, 40).Size(70, 20);
                    });
            });
    }

    [Test]
    public void NoSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(0);
                    
                    column.Item().Mock("a").Size(50, 30);
                    column.Item().Mock("b").Size(40, 20);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(0, 30).Size(50, 20);
                    });
            });
    }

    [Test]
    public void PartialRenderItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 80)
            .WithContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(5);
                    
                    column.Item().Mock("a").Size(50, 40);
                    column.Item().Mock("b").Size(60, 50);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 80)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(60, 40);
                        page.Mock("b").Position(0, 45).Size(60, 35);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 15)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(60, 15);
                    });
            });
    }
}