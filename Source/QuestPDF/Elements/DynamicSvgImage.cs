using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class DynamicSvgImage : Element
{
    public GenerateDynamicSvgDelegate SvgSource { get; set; }

    internal override SpacePlan Measure(Size availableSpace)
    {
        return availableSpace.IsNegative() 
            ? SpacePlan.Wrap() 
            : SpacePlan.FullRender(availableSpace);
    }

    internal override void Draw(Size availableSpace)
    {
        var svg = SvgSource?.Invoke(availableSpace);
     
        if (svg == null)
            return;

        using var svgImage = new SkSvgImage(svg, SkResourceProvider.CurrentResourceProvider, FontManager.CurrentFontManager);
        Canvas.DrawSvg(svgImage, availableSpace);
    }
}