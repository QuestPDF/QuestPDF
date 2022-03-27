using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    public static class FontManager
    {
        private static ConcurrentDictionary<string, FontStyleSet> StyleSets = new();
        private static ConcurrentDictionary<object, SKFontMetrics> FontMetrics = new();
        private static ConcurrentDictionary<object, SKPaint> Paints = new();
        private static ConcurrentDictionary<string, SKPaint> ColorPaint = new();

        private static void RegisterFontType(SKData fontData, string? customName = null)
        {
            foreach (var index in Enumerable.Range(0, 256))
            {
                var typeface = SKTypeface.FromData(fontData, index);
                
                if (typeface == null)
                    break;
                
                var typefaceName = customName ?? typeface.FamilyName;

                var fontStyleSet = StyleSets.GetOrAdd(typefaceName, _ => new FontStyleSet());
                fontStyleSet.Add(typeface);
            }
        }

        [Obsolete("Since version 2022.3, the FontManager class offers better font type matching support. Please use the RegisterFont(Stream stream) method.")]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            using var fontData = SKData.Create(stream);
            RegisterFontType(fontData);
            RegisterFontType(fontData, customName: fontName);
        }

        public static void RegisterFont(Stream stream)
        {
            using var fontData = SKData.Create(stream);
            RegisterFontType(fontData);
        }

        internal static SKPaint ColorToPaint(this string color)
        {
            return ColorPaint.GetOrAdd(color, Convert);

            static SKPaint Convert(string color)
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(color),
                    IsAntialias = true
                };
            }
        }

        internal static SKPaint ToPaint(this TextStyle style)
        {
            return Paints.GetOrAdd(style.PaintKey, key => Convert(style));

            static SKPaint Convert(TextStyle style)
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(style.Color),
                    Typeface = GetTypeface(style),
                    TextSize = style.Size ?? 12,
                    IsAntialias = true,
                };
            }

            static SKTypeface GetTypeface(TextStyle style)
            {
                var weight = (SKFontStyleWeight)(style.FontWeight ?? FontWeight.Normal);
                var slant = (style.IsItalic ?? false) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

                var fontStyle = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);

                if (StyleSets.TryGetValue(style.FontFamily, out var fontStyleSet))
                    return fontStyleSet.Match(fontStyle);

                var fontFromDefaultSource = SKFontManager.Default.MatchFamily(style.FontFamily, fontStyle);
                
                if (fontFromDefaultSource != null)
                    return fontFromDefaultSource;
                
                throw new ArgumentException(
                    $"The typeface '{style.FontFamily}' could not be found. " +
                    $"Please consider the following options: " +
                    $"1) install the font on your operating system or execution environment. " +
                    $"2) load a font file specifically for QuestPDF usage via the QuestPDF.Drawing.FontManager.RegisterFontType(Stream fileContentStream) static method.");
            }
        }

        internal static SKFontMetrics ToFontMetrics(this TextStyle style)
        {
            return FontMetrics.GetOrAdd(style.FontMetricsKey, key => style.ToPaint().FontMetrics);
        }
    }
}