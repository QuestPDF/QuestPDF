using System;
using System.Collections.Generic;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.Proxy;

internal class SnapshotRecorder : ElementProxy, IDisposable
{
    SnapshotRecorderCanvas RecorderCanvas { get; } = new();
    SkPictureRecorder PictureRecorder { get; } = new();
    Dictionary<(int pageNumber, float availableWidth, float availableHeight), SpacePlan> MeasureCache { get; } = new();
    Dictionary<int, SkPicture> DrawCache { get; } = new();

    ~SnapshotRecorder()
    {
        Dispose();
    }

    public void Dispose()
    {
        PictureRecorder.Dispose();
        
        foreach (var cacheValue in DrawCache.Values)
            cacheValue.Dispose();
        
        GC.SuppressFinalize(this);
    }
    
    public SnapshotRecorder(Element child)
    {
        Child = child;
    }
    
    private void Initialize()
    {
        if (Child.Canvas is SnapshotRecorderCanvas)
            return;
        
        Child.VisitChildren(x => x.Canvas = RecorderCanvas);
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        Initialize();

        var cacheItem = (PageContext.CurrentPage, availableSpace.Width, availableSpace.Height);
        
        if (MeasureCache.TryGetValue(cacheItem, out var measurement))
            return measurement;
        
        var result = base.Measure(availableSpace);
        MeasureCache[cacheItem] = result;
        return result;
    }
        
    internal override void Draw(Size availableSpace)
    {
        // element may overflow the available space
        // capture as much as possible around the origin point
        var cachePictureSize = Size.Max;
        var cachePictureOffset = new Position(cachePictureSize.Width / 2, cachePictureSize.Height / 2);
        
        if (DrawCache.TryGetValue(PageContext.CurrentPage, out var snapshot))
        {
            Canvas.Translate(cachePictureOffset.Reverse());
            Canvas.DrawPicture(snapshot);
            Canvas.Translate(cachePictureOffset);
            
            snapshot.Dispose();
            return;
        }
        
        using var canvas = PictureRecorder.BeginRecording(Size.Max.Width, Size.Max.Height);
        RecorderCanvas.Canvas = canvas;

        RecorderCanvas.Translate(cachePictureOffset);
        base.Draw(availableSpace);
        
        DrawCache[PageContext.CurrentPage] = PictureRecorder.EndRecording();
        RecorderCanvas.Canvas = null;
    }
}
