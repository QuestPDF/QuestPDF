using System.Collections.Concurrent;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal static class CanvasCache
    {
        internal static ConcurrentDictionary<string, SKPaint> Paints = new ConcurrentDictionary<string, SKPaint>();
        private static ConcurrentDictionary<string, SKFontMetrics> FontMetrics = new ConcurrentDictionary<string, SKFontMetrics>();
        private static ConcurrentDictionary<string, SKPaint> ColorPaint = new ConcurrentDictionary<string, SKPaint>();

        internal static SKPaint ColorToPaint(this string color)
        {
            return ColorPaint.GetOrAdd(color, Convert);

            static SKPaint Convert(string color)
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(color)
                };
            }
        }
        
        internal static SKPaint ToPaint(this TextStyle style)
        {
            return Paints.GetOrAdd(style.ToString(), key => Convert(style));
            
            static SKPaint Convert(TextStyle style)
            {
                var slant = style.IsItalic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
                
                return new SKPaint
                {
                    Color = SKColor.Parse(style.Color),
                    Typeface = SKTypeface.FromFamilyName(style.FontType, (int)style.FontWeight, (int)SKFontStyleWidth.Normal, slant),
                    TextSize = style.Size,
                    TextEncoding = SKTextEncoding.Utf32
                };
            }
        }

        internal static SKFontMetrics ToFontMetrics(this TextStyle style)
        {
            return FontMetrics.GetOrAdd(style.ToString(), key => style.ToPaint().FontMetrics);
        }
        
        internal static TextMeasurement BreakText(this TextStyle style, string text, float availableWidth)
        {
            var index = (int)style.ToPaint().BreakText(text, availableWidth, out var width);
            
            return new TextMeasurement()
            {
                LineIndex = index,
                FragmentWidth = width
            };
        }
    }
}