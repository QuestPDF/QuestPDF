using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class DynamicSvgImage : Element, IStateResettable
{
    private bool IsRendered { get; set; }
    
    public GenerateDynamicSvgDelegate SvgSource { get; set; }

    public void ResetState(bool hardReset = false)
    {
        IsRendered = false;
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.FullRender(Size.Zero);

        if (availableSpace.IsNegative())
            return SpacePlan.Wrap();
        
        return SpacePlan.FullRender(availableSpace);
    }

    internal override void Draw(Size availableSpace)
    {
        var svg = SvgSource?.Invoke(availableSpace);
     
        if (svg == null)
            return;

        using var svgImage = new SkSvgImage(svg, SkResourceProvider.CurrentResourceProvider, FontManager.CurrentFontManager);
        Canvas.DrawSvg(svgImage, availableSpace);
        
        IsRendered = true;
    }
}