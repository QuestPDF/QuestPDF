using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.VisualTests;

public class LineTests
{
    [Test]
    public void Line_Thickness_Horizontal()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                foreach (var thickness in new[] { 1, 2, 4, 8 })
                {
                    column.Item()
                        .Width(100)
                        .LineHorizontal(thickness);
                }
            });
        });
    }
    
    [Test]
    public void Line_Thickness_Vertical()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Row(row =>
            {
                row.Spacing(20);

                foreach (var thickness in new[] { 1, 2, 4, 8 })
                {
                    row.AutoItem()
                        .Height(100)
                        .LineVertical(thickness);
                }
            });
        });
    }
    
    [Test]
    public void Line_Colors()
    {
        var colors = new[]
        {
            Colors.Red.Medium,
            Colors.Green.Medium,
            Colors.Blue.Medium,
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);
                
                foreach (var color in colors)
                {
                    column.Item()
                        .Width(100)
                        .LineHorizontal(4)
                        .LineColor(color);
                }
            });
        });
    }
    
    [Test]
    public void Line_Gradient()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                column.Item()
                    .Width(100)
                    .LineHorizontal(4)
                    .LineGradient([Colors.Red.Medium, Colors.Orange.Medium]);
                
                column.Item()
                    .Width(100)
                    .LineHorizontal(4)
                    .LineGradient([Colors.Orange.Medium, Colors.Yellow.Medium, Colors.Lime.Medium]);
                
                column.Item()
                    .Width(100)
                    .LineHorizontal(4)
                    .LineGradient([Colors.Blue.Lighten2, Colors.LightBlue.Lighten1, Colors.Cyan.Medium, Colors.Teal.Darken1, Colors.Green.Darken2]);
            });
        });
    }
    
    [Test]
    public void Line_DashPattern()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                column.Item()
                    .Width(200)
                    .LineHorizontal(4)
                    .LineDashPattern([4f, 4f]);
                
                column.Item()
                    .Width(200)
                    .LineHorizontal(4)
                    .LineDashPattern([12f, 12f]);
                
                column.Item()
                    .Width(200)
                    .LineHorizontal(4)
                    .LineDashPattern([4f, 4f, 12f, 4f]);
            });
        });
    }
    
    [Test]
    public void Line_Complex()
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
}