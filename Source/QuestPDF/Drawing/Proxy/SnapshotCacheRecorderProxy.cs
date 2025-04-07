using System;
using System.Collections.Generic;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.Proxy;

internal sealed class SnapshotCacheRecorderProxy : ElementProxy, IDisposable
{
    private ProxyDrawingCanvas RecorderCanvas { get; } = new();
    private Dictionary<(int pageNumber, float availableWidth, float availableHeight), SpacePlan> MeasureCache { get; } = new();
    private Dictionary<int, DocumentPageSnapshot> DrawCache { get; } = new();

    ~SnapshotCacheRecorderProxy()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }

    public void Dispose()
    {
        RecorderCanvas?.Dispose();
        
        foreach (var cacheValue in DrawCache.Values)
            cacheValue.Dispose();
        
        GC.SuppressFinalize(this);
    }
    
    public SnapshotCacheRecorderProxy(Element child)
    {
        Child = child;
    }
    
    private void Initialize()
    {
        if (Child.Canvas == RecorderCanvas)
            return;
        
        Child.VisitChildren(x => x.Canvas = RecorderCanvas);
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        Initialize();

        var cacheItem = (PageContext.CurrentPage, availableSpace.Width, availableSpace.Height);
        
        if (MeasureCache.TryGetValue(cacheItem, out var measurement))
            return measurement;

        RecorderCanvas.Target = new FreeDrawingCanvas();
        var result = base.Measure(availableSpace);
        RecorderCanvas.Target = null;
        
        MeasureCache[cacheItem] = result;
        return result;
    }
        
    internal override void Draw(Size availableSpace)
    {
        if (DrawCache.TryGetValue(PageContext.CurrentPage, out var snapshot))
        {
            Canvas.DrawSnapshot(snapshot);
            
            snapshot.Dispose();
            DrawCache.Remove(PageContext.CurrentPage);
            return;
        }
        
        using var skiaCanvas = new SkiaDrawingCanvas(Size.Max.Width, Size.Max.Height);
        RecorderCanvas.Target = skiaCanvas;
        RecorderCanvas.SetZIndex(0);
        RecorderCanvas.SetMatrix(Canvas.GetCurrentMatrix());
        
        base.Draw(availableSpace);
        
        DrawCache[PageContext.CurrentPage] = skiaCanvas.GetSnapshot();
        RecorderCanvas.Target = null;
    }
}
