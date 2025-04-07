using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Skia;

namespace QuestPDF.Drawing;

internal class DocumentPageSnapshot : IDisposable
{
    public List<LayerSnapshot> Layers { get; init; }
    
    ~DocumentPageSnapshot()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        foreach (var layer in Layers)
            layer.Picture.Dispose();

        Layers.Clear();
        GC.SuppressFinalize(this);
    }
    
    public class LayerSnapshot
    {
        public int ZIndex { get; init; }
        public SkPicture Picture { get; init; }
    }

    public void DrawOnSkCanvas(SkCanvas canvas)
    {
        foreach (var layerSnapshot in Layers.OrderBy(x => x.ZIndex))
            canvas.DrawPicture(layerSnapshot.Picture);
    }
}