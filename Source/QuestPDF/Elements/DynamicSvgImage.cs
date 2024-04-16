using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class DynamicSvgImage : Element, IStateful
{
    public bool IsRendered { get; set; }
    public GenerateDynamicSvgDelegate SvgSource { get; set; }

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
        var svg = SvgSource?.Invoke(availableSpace);
     
        if (svg == null)
            return;

        using var svgImage = new SkSvgImage(svg, FontManager.CurrentFontManager);
        Canvas.DrawSvg(svgImage, availableSpace);
        
        IsRendered = true;
    }
    
    #region IStateful
    
    object IStateful.CloneState()
    {
        return IsRendered;
    }

    void IStateful.SetState(object state)
    {
        IsRendered = (bool) state;
    }

    void IStateful.ResetState(bool hardReset)
    {
        IsRendered = false;
    }
    
    #endregion
}