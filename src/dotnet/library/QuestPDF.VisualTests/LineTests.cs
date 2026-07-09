using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests;

public class LineTests
{
    [Test]
    public void ThicknessHorizontal(
        [Values(1, 2, 4, 8)] float thickness)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(100)
                .Column(column =>
                {
                    column.Item().Height(50).Background(Colors.Blue.Lighten4);
                    column.Item().LineHorizontal(thickness);
                    column.Item().Height(50).Background(Colors.Green.Lighten4);
                });
        });
    }
    
    [Test]
    public void ThicknessVertical(
        [Values(1, 2, 4, 8)] float thickness)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Height(100)
                .Row(row =>
                {
                    row.AutoItem().Width(50).Background(Colors.Blue.Lighten4);
                    row.AutoItem().LineVertical(thickness);
                    row.AutoItem().Width(50).Background(Colors.Green.Lighten4);
                });
        });
    }
    
    private static readonly IEnumerable<Color> ColorValues = [Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium];

    [Test]
    public void Color(
        [ValueSource(nameof(ColorValues))] Color color)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(100)
                .LineHorizontal(4)
                .LineColor(color);
        });
    }

    private static readonly IEnumerable<Color[]> GradientValues =
    [
        [Colors.Red.Medium, Colors.Green.Medium],
        [Colors.Red.Medium, Colors.Yellow.Medium, Colors.Green.Medium],
        [Colors.Blue.Medium, Colors.LightBlue.Medium, Colors.Cyan.Medium, Colors.Teal.Medium, Colors.Green.Medium]
    ];
    
    [Test]
    public void Gradient(
        [ValueSource(nameof(GradientValues))] Color[] colors)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(400)
                .LineHorizontal(16)
                .LineGradient(colors);
        });
    }

    private static readonly IEnumerable<float[]> DashPatternCases =
    [
        [1, 1],
        [1, 2],
        [2, 1],
        [2, 2],
        [4, 4],
        [8, 8],
        [4, 4, 12, 4],
        [4, 4, 8, 8, 12, 12],
    ];
    
    [Test, TestCaseSource(nameof(DashPatternCases))]
    public void DashPattern(float[] dashPattern)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(400)
                .LineHorizontal(4)
                .LineDashPattern(dashPattern);
        });
    }
    
    [Test]
    public void Complex()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(300)
                .LineHorizontal(8)
                .LineDashPattern([4, 4, 8, 8, 12, 12])
                .LineGradient([Colors.Red.Medium, Colors.Orange.Medium, Colors.Yellow.Medium]);
        });
    }
    
    #region IStateful
    
    [Test]
    public void LineShouldRenderOnlyOnce()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Height(400)
                .Width(400)
                .Row(row =>
                {
                    row.RelativeItem().LineHorizontal(10);
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Height(300).Background(Colors.Blue.Lighten1);
                        column.Item().Height(200).Background(Colors.Blue.Lighten3);
                    });
                });
        });
    }
    
    [Test]
    public void LineShouldRerenderWhenCombinedWithRepeat()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Height(400)
                .Width(400)
                .Row(row =>
                {
                    row.RelativeItem().Repeat().LineHorizontal(10);
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Height(300).Background(Colors.Blue.Lighten1);
                        column.Item().Height(200).Background(Colors.Blue.Lighten3);
                    });
                });
        });
    }
    
    #endregion
}