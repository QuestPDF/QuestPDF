using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        Canvas.DrawSvg(Image.SkSvgImage, availableSpace);
    }
}