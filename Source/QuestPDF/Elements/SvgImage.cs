using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SvgImage : Element, IStateful
{
    public bool IsRendered { get; set; }
    public Infrastructure.SvgImage Image { get; set; }
    
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
        Canvas.DrawSvg(Image.SkSvgImage, availableSpace);
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