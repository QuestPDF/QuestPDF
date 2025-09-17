namespace QuestPDF.LayoutTests;

public class RotateTests
{
    [Test]
    public void SimpleRotation()
    {
        const float angle = 60;
        const float armLength = 100;
        
        var expectedX = armLength * MathF.Cos(float.DegreesToRadians(angle));
        var expectedY = armLength * MathF.Sin(float.DegreesToRadians(angle));
        
        Assert.That(expectedX, Is.EqualTo(50).Within(0.1f));
        Assert.That(expectedY, Is.EqualTo(86.6).Within(0.1f));
        
        LayoutTest
            .HavingSpaceOfSize(500, 500)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .Rotate(angle)
                    .TranslateX(armLength)
                    .Mock("a")
                    .SolidBlock(100, 100);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(expectedX, expectedY).Size(100, 100);
                    });
            });
    }
}