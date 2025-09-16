using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Skia;

namespace QuestPDF.LayoutTests.TestEngine;

internal class SolidBlock : Element, IStateful
{
    public float TotalWidth { get; set; } 
    public float TotalHeight { get; set; }

    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();
        
        if (TotalWidth > availableSpace.Width + Size.Epsilon)
            return SpacePlan.Wrap("The content requires more horizontal space than available.");

        if (TotalHeight > availableSpace.Height + Size.Epsilon)
            return SpacePlan.Wrap("The content requires more vertical space than available.");

        return SpacePlan.FullRender(TotalWidth, TotalHeight);
    }

    internal override void Draw(Size availableSpace)
    {
        using var paint = new SkPaint();
        paint.SetSolidColor(Colors.Grey.Medium);
        Canvas.DrawRectangle(Position.Zero, availableSpace, paint);
        
        IsRendered = true;
    }
    
    #region IStateful
        
    private bool IsRendered { get; set; }

    public void ResetState(bool hardReset = false)
    {
        if (hardReset)
            IsRendered = false;
    }
        
    public object GetState() => IsRendered;
    public void SetState(object state) => IsRendered = (bool) state;
    
    #endregion
}