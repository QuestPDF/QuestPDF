using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class SvgPath : Element, IStateful
{
    public string Path { get; set; } = string.Empty;
    public Color FillColor { get; set; } = Colors.Black;

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
        
        Canvas.DrawSvgPath(Path, FillColor);
        IsRendered = true;
    }
    
    #region IStateful
    
    private bool IsRendered { get; set; }
    
    public void ResetState(bool hardReset = false) => IsRendered = false;
    public object GetState() => IsRendered;
    public void SetState(object state) => IsRendered = (bool) state;
    
    #endregion
}