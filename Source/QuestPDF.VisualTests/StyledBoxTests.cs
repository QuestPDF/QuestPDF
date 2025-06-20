using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests;

public class StyledBoxTests
{
    #region Background
    
    [Test]
    public void StyledBox_Background_Color()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                column.Item()
                    .Width(200)
                    .Height(50)
                    .Background(Colors.Red.Medium);
                
                column.Item()
                    .Width(200)
                    .Height(100)
                    .Background(Colors.Green.Medium);
                
                column.Item()
                    .Width(200)
                    .Height(150)
                    .Background(Colors.Blue.Medium);
            });
        });
    }
    
    [Test]
    public void StyledBox_Background_GradientColors()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                column.Item()
                    .Width(200)
                    .Height(100)
                    .BackgroundLinearGradient(0, [Colors.Red.Medium, Colors.Blue.Medium]);
                
                column.Item()
                    .Width(200)
                    .Height(100)
                    .BackgroundLinearGradient(0, [Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium]);
                
                column.Item()
                    .Width(200)
                    .Height(100)
                    .BackgroundLinearGradient(0, [Colors.Red.Medium, Colors.Yellow.Darken1, Colors.Green.Medium, Colors.Cyan.Medium, Colors.Blue.Medium]);
            });
        });
    }
    
    [Test]
    public void StyledBox_Background_GradientAngle()
    {
        var gradient = new[]
        {
            Colors.Black,
            Colors.White
        };
        
        var angles = new[] { 0, 30, 45, 60, 90 };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                foreach (var angle in angles)
                {
                    column.Item()
                        .Width(200)
                        .Height(100)
                        .BackgroundLinearGradient(angle, gradient);
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Background_RoundedCorners()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                column
                    .Item()
                    .Width(200)
                    .Height(150)
                    .Background(Colors.Grey.Medium)
                    .BorderRadius(10);
                
                column
                    .Item()
                    .Width(200)
                    .Height(150)
                    .Background(Colors.Grey.Medium)
                    .BorderRadius(20);
                
                column
                    .Item()
                    .Width(200)
                    .Height(150)
                    .Background(Colors.Grey.Medium)
                    .BorderRadiusTopLeft(0)
                    .BorderRadiusTopRight(10)
                    .BorderRadiusBottomRight(20)
                    .BorderRadiusBottomLeft(40);

                column
                    .Item()
                    .Width(200)
                    .Height(150)
                    .Background(Colors.Grey.Medium)
                    .BorderRadius(100);
            });
        });
    }
    
    #endregion
    
    #region Border
    
    [Test]
    public void StyledBox_Border_Thickness_Consistent()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var thickness in new[] { 1, 2, 4, 8 })
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Border(thickness)
                        .BorderColor(Colors.Black)
                        .Background(Colors.Grey.Lighten2);
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_Thickness_Various()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);

                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .BorderLeft(1)
                    .BorderTop(2)
                    .BorderRight(4)
                    .BorderBottom(8)
                    .Background(Colors.Grey.Lighten2);

                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .BorderLeft(4)
                    .BorderTop(8)
                    .Background(Colors.Grey.Lighten2);
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .BorderLeft(4)
                    .BorderRight(8)
                    .Background(Colors.Grey.Lighten2);
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_RoundedCorners_Simple()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var borderRadius in new[] { 0, 5, 10, 20, 50 })
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Background(Colors.Blue.Lighten3)
                        .Border(2)
                        .BorderRadius(borderRadius); 
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_RoundedCorners_Complex1()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);

                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .BorderLeft(8)
                    .BorderTop(8)
                    .BorderRadius(8)
                    .Background(Colors.Blue.Lighten3);
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .BorderLeft(8)
                    .BorderRight(8)
                    .BorderRadius(16)
                    .Background(Colors.Blue.Lighten3);
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_RoundedCorners_Complex2()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40); 

                foreach (var borderRadius in new[] { 5, 10, 15, 20, 30, 40 })
                {
                    row.AutoItem()
                        .Width(100)
                        .Height(100)
                        .Background(Colors.Blue.Lighten3)
                        .BorderLeft(5)
                        .BorderTop(10)
                        .BorderRight(15)
                        .BorderBottom(20)
                        .BorderRadius(borderRadius); 
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_Color()
    {
        var colors = new[]
        { 
            Colors.Red.Darken3,
            Colors.Green.Darken3,
            Colors.Blue.Darken3
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var color in colors)
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Border(5)
                        .BorderColor(color)
                        .Background(Colors.Grey.Lighten2);
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_GradientColors()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Column(column =>
            {
                column.Spacing(20);

                column.Item()
                    .Width(200)
                    .Height(100)
                    .Border(10)
                    .BorderRadius(5)
                    .BorderLinearGradient(0, [Colors.Red.Medium, Colors.Blue.Medium]);
                
                column.Item()
                    .Width(200)
                    .Height(100)
                    .Border(10)
                    .BorderRadius(5)
                    .BorderLinearGradient(0, [Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium]);
                
                column.Item()
                    .Width(200)
                    .Height(100)
                    .Border(10)
                    .BorderRadius(5)
                    .BorderLinearGradient(0, [Colors.Red.Medium, Colors.Yellow.Darken1, Colors.Green.Medium, Colors.Cyan.Medium, Colors.Blue.Medium]);
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_GradientAngle()
    {
        var gradient = new[]
        {
            Colors.Black,
            Colors.White
        };
        
        var angles = new[] { 0, 30, 45, 60, 90 };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Column(column =>
            {
                column.Spacing(20);

                foreach (var angle in angles)
                {
                    column.Item()
                        .Width(200)
                        .Height(100)
                        .Border(10)
                        .BorderRadius(5)
                        .BorderLinearGradient(angle, gradient);
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Border_Alignment()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container.Row(row =>
            {
                row.Spacing(20);

                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .Border(5)
                    .BorderAlignmentInside()
                    .BorderRadius(10)
                    .Background(Colors.Blue.Lighten3);
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .Border(5)
                    .BorderAlignmentMiddle()
                    .BorderRadius(10)
                    .Background(Colors.Blue.Lighten3);
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .Border(5)
                    .BorderAlignmentOutside()
                    .BorderRadius(10)
                    .Background(Colors.Blue.Lighten3);
            });
        });
    }
    
    #endregion
    
    #region Shadow
    
    [Test]
    public void StyledBox_Shadow_Color()
    {
        var colors = new[]
        {
            Colors.Red.Darken3,
            Colors.Green.Darken3,
            Colors.Blue.Darken3
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var color in colors)
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Background(Colors.White)
                        .Shadow(new BoxShadowStyle
                        {
                            Color = color,
                            Blur = 8
                        });
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Shadow_Blur()
    {
        var blurs = new[]
        {
            2, 
            4,
            8,
            16
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var blur in blurs)
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Background(Colors.White)
                        .Shadow(new BoxShadowStyle
                        {
                            Color = Colors.Grey.Darken2,
                            Blur = blur,
                        });
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Shadow_NoBlur()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .Background(Colors.LightBlue.Medium)
                    .Shadow(new BoxShadowStyle
                    {
                        Color = Colors.Grey.Darken2,
                        OffsetX = 4,
                        OffsetY = 4
                    });
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .Background(Colors.LightBlue.Medium)
                    .Shadow(new BoxShadowStyle
                    {
                        Color = Colors.Grey.Darken2,
                        OffsetX = 0,
                        OffsetY = 0,
                        Spread = 4
                    });
                
                row.AutoItem()
                    .Width(50)
                    .Height(50)
                    .BorderRadius(10)
                    .Background(Colors.LightBlue.Medium)
                    .Shadow(new BoxShadowStyle
                    {
                        Color = Colors.Grey.Darken2,
                        OffsetX = 4,
                        OffsetY = 4
                    });
            });
        });
    }
    
    [Test]
    public void StyledBox_Shadow_Spread()
    {
        var spreads = new[]
        {
            -4,
            0,
            4,
            8,
            12
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var spread in spreads)
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Background(Colors.White)
                        .Shadow(new BoxShadowStyle
                        {
                            Color = Colors.Grey.Darken2,
                            Blur = 4,
                            Spread = spread
                        });
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Shadow_OffsetX()
    {
        var offsets = new[]
        {
            -8,
            -4,
            0,
            4,
            8
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var offset in offsets)
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Background(Colors.White)
                        .Shadow(new BoxShadowStyle
                        {
                            Color = Colors.Grey.Darken2,
                            Blur = 8,
                            OffsetX = offset
                        });
                }
            });
        });
    }
    
    [Test]
    public void StyledBox_Shadow_OffsetY()
    {
        var offsets = new[]
        {
            -8,
            -4,
            0,
            4,
            8
        };
        
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container.Row(row =>
            {
                row.Spacing(40);
                
                foreach (var offset in offsets)
                {
                    row.AutoItem()
                        .Width(50)
                        .Height(50)
                        .Background(Colors.White)
                        .Shadow(new BoxShadowStyle
                        {
                            Color = Colors.Grey.Darken2,
                            Blur = 8,
                            OffsetY = offset
                        });
                }
            });
        });
    }
    
    #endregion
    
    #region Clipping
    
    [Test]
    public void StyledBox_Clip_Image()
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
    public void StyledBox_Clip_Text()
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
    
    #endregion
}