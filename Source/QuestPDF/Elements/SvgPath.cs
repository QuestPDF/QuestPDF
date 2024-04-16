using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SvgPath : Element, IStateful
{
    public bool IsRendered { get; set; }
    
    public string Path { get; set; } = string.Empty;
    public Color FillColor { get; set; } = Colors.Black;
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (availableSpace.IsNegative())
            return SpacePlan.Wrap();
        
        if (IsRendered)
            return SpacePlan.None();
        
        return SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        Canvas.DrawSvgPath(Path, FillColor);
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