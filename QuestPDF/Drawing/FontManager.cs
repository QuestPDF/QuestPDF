using System;
using System.Collections.Concurrent;
using System.IO;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    public static class FontManager
    {
        private static ConcurrentDictionary<string, FontStyleSet> StyleSets = new ConcurrentDictionary<string, FontStyleSet>();
        private static ConcurrentDictionary<string, SKFontMetrics> FontMetrics = new ConcurrentDictionary<string, SKFontMetrics>();
        private static ConcurrentDictionary<string, SKPaint> Paints = new ConcurrentDictionary<string, SKPaint>();
        private static ConcurrentDictionary<string, SKPaint> ColorPaint = new ConcurrentDictionary<string, SKPaint>();

        private static void RegisterFontType(string fontName, SKTypeface typeface)
        {
            FontStyleSet set = StyleSets.GetOrAdd(fontName, _ => new FontStyleSet());
            set.Add(typeface);
        }

        public static void RegisterFontType(string fontName, Stream stream)
        {
            SKTypeface typeface = SKTypeface.FromStream(stream);
            RegisterFontType(fontName, typeface);
        }

        public static void RegisterFontType(Stream stream)
        {
            SKTypeface typeface = SKTypeface.FromStream(stream);
            RegisterFontType(typeface.FamilyName, typeface);
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
                SKFontStyleWeight weight = (SKFontStyleWeight)(style.FontWeight ?? FontWeight.Normal);
                SKFontStyleWidth width = SKFontStyleWidth.Normal;
                SKFontStyleSlant slant = (style.IsItalic ?? false) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

                SKFontStyle skFontStyle = new SKFontStyle(weight, width, slant);

                FontStyleSet set;
                if (StyleSets.TryGetValue(style.FontType, out set))
                {
                    return set.Match(skFontStyle);
                }
                else
                {
                    return SKTypeface.FromFamilyName(style.FontType, skFontStyle)
                        ?? throw new ArgumentException($"The typeface {style.FontType} could not be found.");
                }
            }
        }

        internal static SKFontMetrics ToFontMetrics(this TextStyle style)
        {
            return FontMetrics.GetOrAdd(style.Key, key => style.ToPaint().FontMetrics);
        }
    }
}