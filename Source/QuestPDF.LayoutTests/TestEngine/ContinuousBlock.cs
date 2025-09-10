using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Skia;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ContinuousBlock : Element
{
    public float TotalWidth { get; set; }
    public float TotalHeight { get; set; }
    
    private float HeightOffset { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (TotalWidth > availableSpace.Width)
            return SpacePlan.Wrap("The content requires more horizontal space than available.");

        if (availableSpace.Height < Size.Epsilon)
            return SpacePlan.Wrap("The content requires more vertical space than available.");

        var remainingHeight = TotalHeight - HeightOffset;

        if (remainingHeight < Size.Epsilon)
            return SpacePlan.FullRender(Size.Zero);
        
        if (remainingHeight > availableSpace.Height)
            return SpacePlan.PartialRender(TotalWidth, availableSpace.Height);
        
        return SpacePlan.FullRender(TotalWidth, remainingHeight);
    }

    internal override void Draw(Size availableSpace)
    {
        var height = Math.Min(TotalHeight - HeightOffset, availableSpace.Height);
        var size = new Size(TotalWidth, height);
        
        HeightOffset += height;

        using var paint = new SkPaint();
        paint.SetSolidColor(Colors.Grey.Medium);
        Canvas.DrawRectangle(Position.Zero, size, paint);

        if (HeightOffset > TotalHeight - Size.Epsilon)
            HeightOffset = 0;
    }
}