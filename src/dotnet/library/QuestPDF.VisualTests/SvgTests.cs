using QuestPDF.Fluent;

namespace QuestPDF.VisualTests;

public class SvgTests
{
    [Test]
    public void TextDominantBaseline()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Svg(Path.Combine("Resources", "textDominantBaseline.svg"));
        });
    }
    
    [Test]
    public void TextSpanWithOffset()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Svg(Path.Combine("Resources", "textSpanOffset.svg"));
        });
    }
}