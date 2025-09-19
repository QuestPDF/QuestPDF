namespace QuestPDF.LayoutTests;

public class LineTests
{
    [Test]
    public void VerticalLineRequiresSpaceEqualToItsThickness()
    {
        LayoutTest
            .HavingSpaceOfSize(5, 100)
            .ForContent(content =>
            {
                content.Shrink().LineVertical(10);
            })
            .ExpectLayoutException("The line thickness is greater than the available horizontal space.");
    }
    
    [Test]
    public void HorizontalLineRequiresSpaceEqualToItsThickness()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 5)
            .ForContent(content =>
            {
                content.Shrink().LineHorizontal(10);
            })
            .ExpectLayoutException("The line thickness is greater than the available vertical space.");
    }
    
    #region Stateful
    
    [Test]
    public void CheckRenderingState()
    {
        LayoutTest
            .HavingSpaceOfSize(240, 100)
            .ForContent(content =>
            {
                content.ShrinkVertical().Mock("a").LineHorizontal(2);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 2)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(240, 2).State(true);
                    });
            });
    }
    
    #endregion
}