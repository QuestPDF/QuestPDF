using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class SvgImage : Element
{
    public SkSvgImage Svg { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        return availableSpace.IsNegative() 
            ? SpacePlan.Wrap() 
            : SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        Canvas.DrawSvg(Svg, availableSpace);
    }
}