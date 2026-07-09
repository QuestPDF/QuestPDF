using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal sealed class SvgImage : Element, IStateful, IDisposable
{
    public Infrastructure.SvgImage Image { get; set; }
    
    ~SvgImage()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Image != null && !Image.IsShared)
            Image?.Dispose();
        
        GC.SuppressFinalize(this);
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();

        if (availableSpace.IsNegative())
            return SpacePlan.Wrap("The available space is negative.");
        
        return SpacePlan.FullRender(Size.Zero);
    }

    internal override void Draw(Size availableSpace)
    {
        if (IsRendered)
            return;
        
        var (widthScale, heightScale) = Image.SkSvgImage.CalculateSpaceScale(availableSpace);

        Canvas.Save();
        Canvas.Scale(widthScale,  heightScale);
        Canvas.DrawSvg(Image.SkSvgImage, availableSpace);
        Canvas.Restore();
        
        IsRendered = true;
    }
    
    #region IStateful
    
    private bool IsRendered { get; set; }
    
    public void ResetState(bool hardReset = false) => IsRendered = false;
    public object GetState() => IsRendered;
    public void SetState(object state) => IsRendered = (bool) state;
    
    #endregion
}