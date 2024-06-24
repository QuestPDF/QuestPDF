using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using static QuestPDF.Skia.SkSvgImageSize.Unit;

namespace QuestPDF.Elements;

internal class SvgImage : Element, IStateResettable
{
    private bool IsRendered { get; set; }
    
    public Infrastructure.SvgImage Image { get; set; }
    
    public void ResetState(bool hardReset = false)
    {
        IsRendered = false;
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();

        if (availableSpace.IsNegative())
            return SpacePlan.Wrap("The available space is negative.");
        
        return SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        if (IsRendered)
            return;
        
        var widthScale = CalculateSpaceScale(availableSpace.Width, Image.SkSvgImage.Size.Width, Image.SkSvgImage.Size.WidthUnit);
        var heightScale = CalculateSpaceScale(availableSpace.Height, Image.SkSvgImage.Size.Height, Image.SkSvgImage.Size.HeightUnit);
        
        Canvas.Save();
        Canvas.Scale(widthScale,  heightScale);
        Canvas.DrawSvg(Image.SkSvgImage, availableSpace);
        Canvas.Restore();
        
        IsRendered = true;
        
        float CalculateSpaceScale(float availableSize, float imageSize, SkSvgImageSize.Unit unit)
        {
            if (unit == Percentage)
                return 100f / imageSize;

            if (unit is Centimeters or Millimeters or Inches or Points or Picas)
                return availableSize / ConvertToPoints(imageSize, unit);

            return availableSize / imageSize;
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