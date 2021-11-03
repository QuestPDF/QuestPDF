using System;
using System.Collections.Concurrent;
using System.IO;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    public static class FontManager
    {
        private static ConcurrentDictionary<string, SKTypeface> Typefaces = new ConcurrentDictionary<string, SKTypeface>();
        private static ConcurrentDictionary<string, SKFontMetrics> FontMetrics = new ConcurrentDictionary<string, SKFontMetrics>();
        private static ConcurrentDictionary<string, SKPaint> Paints = new ConcurrentDictionary<string, SKPaint>();
        private static ConcurrentDictionary<string, SKPaint> ColorPaint = new ConcurrentDictionary<string, SKPaint>();

        public static void RegisterFontType(string fontName, Stream stream)
        {
            Typefaces.TryAdd(fontName, SKTypeface.FromStream(stream));
        }
        
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
            return Paints.GetOrAdd(style.Key, key => Convert(style));
            
            static SKPaint Convert(TextStyle style)
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(style.Color),
                    Typeface = GetTypeface(style),
                    TextSize = (style.Size ?? 12),
                    TextEncoding = SKTextEncoding.Utf32
                };
            }

            static SKTypeface GetTypeface(TextStyle style)
            {
                if (Typefaces.TryGetValue(style.FontType, out var result))
                    return result;
                
                var slant = (style.IsItalic ?? false) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
                
                return SKTypeface.FromFamilyName(style.FontType, (int)(style.FontWeight ?? FontWeight.Normal), (int)SKFontStyleWidth.Normal, slant) 
                       ?? throw new ArgumentException($"The typeface {style.FontType} could not be found.");
            }
        }

        internal static SKFontMetrics ToFontMetrics(this TextStyle style)
        {
            return FontMetrics.GetOrAdd(style.Key, key => style.ToPaint().FontMetrics);
        }
    }
}