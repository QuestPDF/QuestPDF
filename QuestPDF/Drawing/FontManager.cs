using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace QuestPDF.Drawing
{
    public static class FontManager
    {
        private static ConcurrentDictionary<string, FontStyleSet> StyleSets = new();
        private static ConcurrentDictionary<object, SKFontMetrics> FontMetrics = new();
        private static ConcurrentDictionary<object, SKPaint> Paints = new();
        private static ConcurrentDictionary<object, SKFont> Fonts = new();
        private static ConcurrentDictionary<object, SKShaper> Shapers = new();
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

        [Obsolete("Since version 2022.3, the FontManager class offers better font type matching support. Please use the RegisterFontType(Stream stream) overload.")]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            using var fontData = SKData.Create(stream);
            RegisterFontType(fontData);
            RegisterFontType(fontData, customName: fontName);
        }

        public static void RegisterFontType(Stream stream)
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
                    Color = SKColor.Parse(color)
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
                    TextSize = (style.Size ?? 12) * GetTextScale(style)
                };
            }

            static SKTypeface GetTypeface(TextStyle style)
            {
                var weight = (SKFontStyleWeight)(style.FontWeight ?? FontWeight.Normal);

                //Extra weight for superscript and subscript
                if (style.FontVariant == FontVariant.Superscript || style.FontVariant == FontVariant.Subscript)
                  weight = (SKFontStyleWeight)((int)weight + 100);

                var slant = (style.IsItalic ?? false) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

                var fontStyle = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);

                if (StyleSets.TryGetValue(style.FontType, out var fontStyleSet))
                    return fontStyleSet.Match(fontStyle);

                var fontFromDefaultSource = SKFontManager.Default.MatchFamily(style.FontType, fontStyle);
                
                if (fontFromDefaultSource != null)
                    return fontFromDefaultSource;
                
                throw new ArgumentException(
                    $"The typeface '{style.FontType}' could not be found. " +
                    $"Please consider the following options: " +
                    $"1) install the font on your operating system or execution environment. " +
                    $"2) load a font file specifically for QuestPDF usage via the QuestPDF.Drawing.FontManager.RegisterFontType(Stream fileContentStream) static method.");
            }
        }

        private static float GetTextScale(TextStyle style)
        {
            if (style.FontVariant == FontVariant.Superscript || style.FontVariant == FontVariant.Subscript)
                return 0.65f;

            return 1;
        }

        internal static SKFont ToFont(this TextStyle style)
        {
            return Fonts.GetOrAdd(style.FontMetricsKey, _ => style.ToPaint().Typeface.ToFont());
        }

        internal static SKFontMetrics ToFontMetrics(this TextStyle style)
        {
            return FontMetrics.GetOrAdd(style.FontMetricsKey, key => style.ToPaint().FontMetrics);
        }
        
        internal static SKShaper ToShaper(this TextStyle style)
        {
            return Shapers.GetOrAdd(style.FontMetricsKey, _ => new SKShaper(style.ToFont().Typeface));
        }
    }
}