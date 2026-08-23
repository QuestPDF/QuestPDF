using System.Collections.Concurrent;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing;

internal static class SkPaintCache
{
    private static readonly ConcurrentDictionary<uint, SkPaint> SolidColorPaints = new();
    private static readonly ConcurrentDictionary<(uint Color, float Thickness), SkPaint> StrokePaints = new();

    public static SkPaint GetSolidColor(Color color)
    {
        return SolidColorPaints.GetOrAdd(color.Hex, static color =>
        {
            var paint = new SkPaint();
            
            paint.SetSolidColor(color);
            paint.MarkAsShared();
            
            return paint;
        });
    }

    public static SkPaint GetStroke(Color color, float thickness)
    {
        return StrokePaints.GetOrAdd((color.Hex, thickness), static key =>
        {
            var paint = new SkPaint();
            
            paint.SetStroke(key.Thickness);
            paint.SetSolidColor(key.Color);
            paint.MarkAsShared();
            
            return paint;
        });
    }
}
