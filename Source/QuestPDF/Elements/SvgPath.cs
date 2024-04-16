using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SvgPath : Element, IContent, IStateResettable
{
    public bool IsRendered { get; set; }
    
    public string Path { get; set; } = string.Empty;
    public Color FillColor { get; set; } = Colors.Black;
    
    public void ResetState()
    {
        IsRendered = false;
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (availableSpace.IsNegative())
            return SpacePlan.Wrap();
        
        if (IsRendered)
            return SpacePlan.Empty();
        
        return SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        Canvas.DrawSvgPath(Path, FillColor);
        IsRendered = true;
    }
}