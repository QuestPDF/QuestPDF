namespace QuestPDF.LayoutTests;

public class HyperlinkTests
{
    [Test]
    public void ContentIsDrawn()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Hyperlink("https://example.com/questpdf").Mock("a").SolidBlock(50, 20);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 20)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 20);
                    });
            });
    }

    [Test]
    public void ContentStaysInPlaceInRightToLeftDirection()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                // drawing the link area does not move the content, it still gets the entire space at its original position
                content.ContentFromRightToLeft().Hyperlink("https://example.com/questpdf").Mock("a").SolidBlock(40, 20);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(40, 20)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                    });
            });
    }
}
