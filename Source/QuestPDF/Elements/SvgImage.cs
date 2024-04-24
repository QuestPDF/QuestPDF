using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using static QuestPDF.Skia.SkSvgImageSize.Unit;

namespace QuestPDF.Elements;

internal class SvgImage : Element
{
    public Infrastructure.SvgImage Image { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        return availableSpace.IsNegative() 
            ? SpacePlan.Wrap() 
            : SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        var widthScale = CalculateSpaceScale(availableSpace.Width, Image.SkSvgImage.Size.Width, Image.SkSvgImage.Size.WidthUnit);
        var heightScale = CalculateSpaceScale(availableSpace.Height, Image.SkSvgImage.Size.Height, Image.SkSvgImage.Size.HeightUnit);
        
        Canvas.Save();
        Canvas.Scale(widthScale,  heightScale);
        Canvas.DrawSvg(Image.SkSvgImage, availableSpace);
        Canvas.Restore();
        
        float CalculateSpaceScale(float availableSize, float imageSize, SkSvgImageSize.Unit unit)
        {
            if (unit is Pixels)
                return 1;
            
            if (unit == Percentage)
                return 100f / imageSize;

            if (unit is Centimeters or Millimeters or Inches or Points or Picas)
                return availableSize / ConvertToPoints(imageSize, unit);

            if (unit is Number)
                return availableSize / imageSize;

            throw new NotSupportedException();
        }
    
        float ConvertToPoints(float value, SkSvgImageSize.Unit unit)
        {
            const float InchToCentimetre = 2.54f;
            const float InchToPoints = 72;
            
            // in CSS dpi is set to 96, but Skia uses more traditional 90
            const float PointToPixel = 90f / 72;
        
            var points =  unit switch
            {
                Centimeters => value / InchToCentimetre * InchToPoints,
                Millimeters => value / 10 / InchToCentimetre * InchToPoints,
                Inches => value * InchToPoints,
                Points => value,
                Picas => value * 12,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            // different naming schema: SVG pixel = PDF point
            return points * PointToPixel;
        }
    }
}