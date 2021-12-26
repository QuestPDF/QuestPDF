using System.Collections.Concurrent;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class SkiaCache
    {
        private static ConcurrentDictionary<string, SKPaint> RectangleFillPaints = new ConcurrentDictionary<string, SKPaint>();
        private static ConcurrentDictionary<string, SKPaint> RectangleStrokePaints = new ConcurrentDictionary<string, SKPaint>();

        public static SKPaint GetRectangleFillPaint(string color)
        {
            return RectangleFillPaints.GetOrAdd(color, _ => Convert());

            SKPaint Convert()
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(color),
                    Style = SKPaintStyle.Fill
                };
            }
        }
        
        public static SKPaint GetRectangleStrokePaint(string color, float width)
        {
            var key = $"{color}|{width}";
            return RectangleStrokePaints.GetOrAdd(key, _ => Convert());

            SKPaint Convert()
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(color),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = width
                };
            }
        }
    }
}