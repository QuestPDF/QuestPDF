using System.Collections.Generic;
using System.Linq;
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

    private List<(Size Size, SkSvgImage? Image)> Cache { get; } = new(1);
    
    ~DynamicSvgImage()
    {
        foreach (var cacheItem in Cache)
            cacheItem.Image?.Dispose();
    }
    
    public void ResetState(bool hardReset = false)
    {
        IsRendered = false;
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();

        if (availableSpace.IsNegative())
            return SpacePlan.Wrap();
        
        return SpacePlan.FullRender(availableSpace);
    }

    internal override void Draw(Size availableSpace)
    {
        var targetImage = Cache.FirstOrDefault(x => Size.Equal(x.Size, availableSpace)).Image;
            
        if (targetImage == null)
        {
            targetImage = GetImage(availableSpace);
            Cache.Add((availableSpace, targetImage));
        }
        
        if (targetImage != null)
            Canvas.DrawSvg(targetImage, availableSpace);
            
        IsRendered = true;
    }
    
    private SkSvgImage? GetImage(Size availableSpace)
    {
        var svg = SvgSource?.Invoke(availableSpace);
     
        if (svg == null)
            return null;

        return new SkSvgImage(svg, SkResourceProvider.CurrentResourceProvider, FontManager.CurrentFontManager);
    }
}