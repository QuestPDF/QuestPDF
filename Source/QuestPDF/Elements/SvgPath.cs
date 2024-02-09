using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class SvgPath : Element
{
    public string Path { get; set; } = string.Empty;
    public Color FillColor { get; set; } = Colors.Black;
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        return availableSpace.IsNegative() 
            ? SpacePlan.Wrap() 
            : SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        Canvas.DrawSvgPath(Path, FillColor);
    }
}