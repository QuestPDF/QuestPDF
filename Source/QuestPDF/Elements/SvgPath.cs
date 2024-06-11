using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class SvgPath : Element, IStateResettable
{
    private bool IsRendered { get; set; }
    
    public string Path { get; set; } = string.Empty;
    public Color FillColor { get; set; } = Colors.Black;
    
    public void ResetState(bool hardReset = false)
    {
        IsRendered = false;
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();

        if (availableSpace.IsNegative())
            return SpacePlan.Wrap();
        
        return SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        if (IsRendered)
            return;
        
        Canvas.DrawSvgPath(Path, FillColor);
        IsRendered = true;
    }
}