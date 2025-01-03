using System;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples;

public class RoundedCornersExample
{
    [Test]
    public void ItemTypes()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    var roundedRectangle = new RoundedRectangleParameters
                    {
                        TopLeftRadius = 5,
                        TopRightRadius = 10,
                        BottomRightRadius = 15,
                        BottomLeftRadius = 20,
                        FillColor = Colors.Blue.Lighten3
                    };

                    page.Content()
                        .Padding(10)
                        .Shrink()
                        .Layers(layers =>
                        {
                            layers.Layer().Svg(roundedRectangle.GenerateSVG);
                            layers.PrimaryLayer().Padding(10).Text(Placeholders.Sentence());
                        });
                });
            })
            .GeneratePdfAndShow();
    }
}

public class RoundedRectangleParameters
{
    public double TopLeftRadius { get; set; }
    public double TopRightRadius { get; set; }
    public double BottomLeftRadius { get; set; }
    public double BottomRightRadius { get; set; }
    public string FillColor { get; set; }
    
    public string GenerateSVG(Size size)
    {
        var maxAllowedRadiusX = size.Width / 2.0;
        var maxAllowedRadiusY = size.Height / 2.0;
        
        TopLeftRadius = Math.Min(TopLeftRadius, Math.Min(maxAllowedRadiusX, maxAllowedRadiusY));
        TopRightRadius = Math.Min(TopRightRadius, Math.Min(maxAllowedRadiusX, maxAllowedRadiusY));
        BottomRightRadius = Math.Min(BottomRightRadius, Math.Min(maxAllowedRadiusX, maxAllowedRadiusY));
        BottomLeftRadius = Math.Min(BottomLeftRadius, Math.Min(maxAllowedRadiusX, maxAllowedRadiusY));
        
        var pathData = string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "M {0},0 " +
            "H {1} " +
            "A {2},{2} 0 0 1 {3},{4} " +
            "V {5} " +
            "A {6},{6} 0 0 1 {7},{8} " +
            "H {9} " +
            "A {10},{10} 0 0 1 {11},{12} " +
            "V {13} " +
            "A {14},{14} 0 0 1 {15},0 Z",
            TopLeftRadius,
            size.Width - TopRightRadius,
            TopRightRadius, size.Width, TopRightRadius,
            size.Height - BottomRightRadius,
            BottomRightRadius, size.Width - BottomRightRadius, size.Height,
            BottomLeftRadius,
            BottomLeftRadius, 0, size.Height - BottomLeftRadius,
            TopLeftRadius,
            TopLeftRadius, TopLeftRadius);
        
        return string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "<svg size.Width=\"{0}\" size.Height=\"{1}\" xmlns=\"http://www.w3.org/2000/svg\" " +
            "viewBox=\"0 0 {0} {1}\">" +
            "<path d=\"{2}\" fill=\"{3}\" />" +
            "</svg>",
            size.Width, size.Height,
            pathData,
            FillColor);
    }
}

