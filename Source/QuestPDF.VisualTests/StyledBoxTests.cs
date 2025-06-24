using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests;

public class StyledBoxTests
{
    #region Background

    private static readonly IEnumerable<Color> BackgroundColorValues = [ Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium ];
    
    [Test, TestCaseSource(nameof(BackgroundColorValues))]
    public void BackgroundColor(Color color)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .Background(color);
        });
    }

    private static readonly IEnumerable<Color[]> BackgroundGradientColorsValues =
    [
        [Colors.Red.Medium, Colors.Green.Darken2],
        [Colors.Red.Medium, Colors.Yellow.Medium, Colors.Green.Medium],
        [Colors.Blue.Medium, Colors.LightBlue.Medium, Colors.Cyan.Medium, Colors.Teal.Medium, Colors.Green.Medium]
    ];
    
    [Test, TestCaseSource(nameof(BackgroundGradientColorsValues))]
    public void BackgroundGradientColors(Color[] gradientColors)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .BackgroundLinearGradient(0, gradientColors);
        });
    }
    
    [Test]
    public void BackgroundGradientAngle(
        [Values(0, 30, 45, 60, 90)] float angle)
    {
        var gradient = new[]
        {
            Colors.Black,
            Colors.White
        };

        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .BackgroundLinearGradient(angle, gradient);
        });
    }
    
    [Test]
    public void BackgroundUniformRoundedCorners(
        [Values(0, 5, 10, 25, 50, 100)] float radius)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .Background(Colors.Grey.Medium)
                .BorderRadius(radius);
        });
    }
    
    [TestCase(0, 10, 20, 40)]
    [TestCase(10, 20, 40, 0)]
    [TestCase(20, 40, 0, 10)]
    [TestCase(40, 0, 10, 20)]
    public void BackgroundRoundedCorners(float topLeft, float topRight, float bottomRight, float bottomLeft)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .Background(Colors.Grey.Medium)
                .BorderRadiusTopLeft(topLeft)
                .BorderRadiusTopRight(topRight)
                .BorderRadiusBottomRight(bottomRight)
                .BorderRadiusBottomLeft(bottomLeft);
        });
    }
    
    #endregion
    
    #region Border
    
    [Test]
    public void BorderThicknessUniform(
        [Values(1, 2, 4, 8)] float thickness)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(100)
                .Border(thickness)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten2);
        });
    }
    
    [Test]
    public void BorderThickness(
        [Values(0, 2)] float left, 
        [Values(0, 4)] float top, 
        [Values(0, 6)] float right, 
        [Values(0, 8)] float bottom)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(100)
                .BorderLeft(left)
                .BorderTop(top)
                .BorderRight(right)
                .BorderBottom(bottom)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten2);
        });
    }
    
    [Test]
    public void BorderRoundedCornersUniform(
        [Values(0, 5, 10, 25, 50, 100)] float radius)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(200)
                .Height(100)
                .Border(2)
                .BorderRadius(radius)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten2);
        });
    }
    
    [Test]
    public void BorderRoundedCornersWithVariousCornerRadius(
        [Values(0, 5)] float left,
        [Values(0, 5)] float top,
        [Values(0, 5)] float right,
        [Values(0, 5)] float bottom,
        [Values(5, 15)] float roundedRadius)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(50)
                .BorderLeft(left)
                .BorderTop(top)
                .BorderRight(right)
                .BorderBottom(bottom)
                .BorderRadius(roundedRadius)
                .Background(Colors.Blue.Lighten3);
        });
    }
    
    public static readonly IEnumerable<Color> BorderColorCases = [ Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium ];
    
    [Test, TestCaseSource(nameof(BorderColorCases))]
    public void BorderColor(Color color)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(200)
                .Height(100)
                .Border(5)
                .BorderColor(color)
                .Background(Colors.Grey.Lighten2);
        });
    }

    private static readonly IEnumerable<Color[]> BorderGradientColorsValues =
    [
        [Colors.Red.Medium, Colors.Green.Darken2],
        [Colors.Red.Medium, Colors.Yellow.Medium, Colors.Green.Medium],
        [Colors.Blue.Medium, Colors.LightBlue.Medium, Colors.Cyan.Medium, Colors.Teal.Medium, Colors.Green.Medium]
    ];
    
    [Test, TestCaseSource(nameof(BorderGradientColorsValues))]
    public void BorderGradientColors(Color[] gradientColors)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(200)
                .Height(100)
                .Border(10)
                .BorderRadius(5)
                .BorderLinearGradient(0, gradientColors)
                .Background(Colors.Grey.Lighten2);
        });
    }
    
    [Test]
    public void BorderGradientAngle(
        [Values(0, 30, 45, 60, 90)] float angle)
    {
        var gradient = new[]
        {
            Colors.Black,
            Colors.White
        };

        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .Border(10)
                .BorderRadius(5)
                .BorderLinearGradient(angle, gradient);
        });
    }
    
    private void BorderAlignmentTest(Func<IContainer, IContainer> configuration)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .Height(100)
                .Border(10)
                .BorderRadius(10)
                .BorderColor(Colors.Black.WithAlpha(0.5f))
                .Apply(configuration)
                .Background(Colors.Blue.Lighten3);
        });
    }
    
    [Test]
    public void BorderAlignmentInside()
    {
        BorderAlignmentTest(x => x.BorderAlignmentInside());
    }
    
    [Test]
    public void BorderAlignmentMiddle()
    {
        BorderAlignmentTest(x => x.BorderAlignmentMiddle());
    }
    
    [Test]
    public void BorderAlignmentOutside()
    {
        BorderAlignmentTest(x => x.BorderAlignmentOutside());
    }
    
    #endregion
    
    #region Shadow
    
    private static readonly IEnumerable<Color> ShadowColorValues = [ Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium ];
    
    [Test, TestCaseSource(nameof(ShadowColorValues))]
    public void ShadowColor(Color shadowColor)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(200)
                .Height(100)
                .Background(Colors.White)
                .Shadow(new BoxShadowStyle
                {
                    Color = shadowColor,
                    Blur = 8
                });
        });
    }
    
    [Test]
    public void ShadowBlur(
        [Values(2, 4, 8, 16)] float blur)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(50)
                .Background(Colors.White)
                .Shadow(new BoxShadowStyle
                {
                    Color = Colors.Grey.Darken2,
                    Blur = blur,
                });
        });
    }
    
    [Test]
    public void ShadowWithoutBlur(
        [Values] bool applyRoundedCorners, 
        [Values] bool applySpread, 
        [Values] bool applyOffset)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(50)
                .BorderRadius(applyRoundedCorners ? 20f : 0f)
                .Background(Colors.LightBlue.Medium)
                .Shadow(new BoxShadowStyle
                {
                    Color = Colors.Grey.Darken2,
                    OffsetX = applyOffset ? 4f : 0f,
                    OffsetY = applyOffset ? 4f : 0f,
                    Spread = applySpread ? 8f : 0f,
                });
        });
    }
    
    [Test]
    public void ShadowSpread(
        [Values(-4, 0, 4, 8, 12)] float spread)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(100)
                .Background(Colors.White)
                .Shadow(new BoxShadowStyle
                {
                    Color = Colors.Grey.Darken2,
                    Blur = 4,
                    Spread = spread
                });
        });
    }
    
    [Test]
    public void ShadowOffsetX(
        [Values(-8, -4, 0, 4, 8)] float offsetX)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(100)
                .Background(Colors.White)
                .Shadow(new BoxShadowStyle
                {
                    Color = Colors.Grey.Darken2,
                    Blur = 8,
                    OffsetX = offsetX
                });
        });
    }
    
    [Test]
    public void ShadowOffsetY(
        [Values(-8, -4, 0, 4, 8)] float offsetY)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(100)
                .Height(100)
                .Background(Colors.White)
                .Shadow(new BoxShadowStyle
                {
                    Color = Colors.Grey.Darken2,
                    Blur = 8,
                    OffsetY = offsetY
                });
        });
    }
    
    #endregion
    
    #region Clipping
    
    [Test]
    public void ClipImage()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(200)
                .BorderRadius(25)
                .Shadow(new BoxShadowStyle
                {
                    Color = Colors.Grey.Darken2,
                    Blur = 8
                })
                .Image("Resources/gradient.png");
        });
    }
    
    [Test]
    public void ClipText()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(300)
                .Border(2, Colors.Black)
                .BorderRadius(75)
                .Text(Placeholders.LoremIpsum());
        });
    }
    
    [Test]
    public void ClipContent()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(400)
                .Border(2, Colors.Black)
                .BorderRadius(150)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var i in Enumerable.Range(1, 10))
                            columns.RelativeColumn();
                    });

                    foreach (var i in Enumerable.Range(0, 100))
                    {
                        table.Cell()
                            .Border(1)
                            .Padding(5)
                            .AlignCenter()
                            .Text(i.ToString());
                    }
                });
        });
    }
    
    #endregion
}