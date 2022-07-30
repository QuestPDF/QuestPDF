using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarfBuzzSharp;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace QuestPDF.Drawing
{
    public static class FontManager
    {
        private static ConcurrentDictionary<string, FontStyleSet> StyleSets = new();
        private static ConcurrentDictionary<object, SKFontMetrics> FontMetrics = new();
        private static ConcurrentDictionary<object, SKPaint> FontPaints = new();
        private static ConcurrentDictionary<string, SKPaint> ColorPaints = new();
        private static ConcurrentDictionary<object, Font> ShaperFonts = new();
        private static ConcurrentDictionary<object, SKFont> Fonts = new();
        private static ConcurrentDictionary<object, TextShaper> TextShapers = new();

        static FontManager()
        {
            RegisterLibraryDefaultFonts();
        }
        
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

        [Obsolete("Since version 2022.8 this method has been renamed. Please use the RegisterFontWithCustomName method.")]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            RegisterFontWithCustomName(fontName, stream);
        }
        
        public static void RegisterFontWithCustomName(string fontName, Stream stream)
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
        
        public static void RegisterFontFromEmbeddedResource(string pathName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathName);
            RegisterFont(stream);
        }
        
        private static void RegisterLibraryDefaultFonts()
        {
            var fontFileNames = new[]
            {
                "Lato-Black.ttf",
                "Lato-BlackItalic.ttf",
                
                "Lato-Bold.ttf",
                "Lato-BoldItalic.ttf",
                
                "Lato-Regular.ttf",
                "Lato-Italic.ttf",
                
                "Lato-Light.ttf",
                "Lato-LightItalic.ttf",
                
                "Lato-Thin.ttf",
                "Lato-ThinItalic.ttf"
            };
            
            fontFileNames
                .Select(x => $"QuestPDF.Resources.DefaultFont.{x}")
                .ToList()
                .ForEach(RegisterFontFromEmbeddedResource);
        }

        internal static SKPaint ColorToPaint(this string color)
        {
            return ColorPaints.GetOrAdd(color, Convert);

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
            return FontPaints.GetOrAdd(style.PaintKey, key => Convert(style));

            static SKPaint Convert(TextStyle style)
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(style.Color),
                    Typeface = GetTypeface(style),
                    TextSize = (style.Size ?? 12) * GetTextScale(style),
                    IsAntialias = true,
                };
            }

            static SKTypeface GetTypeface(TextStyle style)
            {
                var weight = (SKFontStyleWeight)(style.FontWeight ?? FontWeight.Normal);

                // superscript and subscript use slightly bolder font to match visually line thickness
                if (style.FontPosition is FontPosition.Superscript or FontPosition.Subscript)
                {
                    var weightValue = (int)weight;
                    weightValue = Math.Min(weightValue + 100, 1000);
                    
                    weight = (SKFontStyleWeight) (weightValue);
                }

                var slant = (style.IsItalic ?? false) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

                var fontStyle = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);

                if (StyleSets.TryGetValue(style.FontFamily, out var fontStyleSet))
                    return fontStyleSet.Match(fontStyle);

                var fontFromDefaultSource = SKFontManager.Default.MatchFamily(style.FontFamily, fontStyle);
                
                if (fontFromDefaultSource != null)
                    return fontFromDefaultSource;

                var availableFontNames = string.Join(", ", SKFontManager.Default.GetFontFamilies());
                
                throw new ArgumentException(
                    $"The typeface '{style.FontFamily}' could not be found. " +
                    $"Please consider the following options: " +
                    $"1) install the font on your operating system or execution environment. " +
                    $"2) load a font file specifically for QuestPDF usage via the QuestPDF.Drawing.FontManager.RegisterFontType(Stream fileContentStream) static method. " +
                    $"Available font family names: [{availableFontNames}]");
            }
            
            static float GetTextScale(TextStyle style)
            {
                return style.FontPosition switch
                {
                    FontPosition.Normal => 1f,
                    FontPosition.Subscript => 0.625f,
                    FontPosition.Superscript => 0.625f,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        internal static SKFontMetrics ToFontMetrics(this TextStyle style)
        {
            return FontMetrics.GetOrAdd(style.FontMetricsKey, key => style.NormalPosition().ToPaint().FontMetrics);
        }

        internal static Font ToShaperFont(this TextStyle style)
        {
            return ShaperFonts.GetOrAdd(style.PaintKey, _ =>
            {
                var typeface = style.ToPaint().Typeface;

                using var harfBuzzBlob = typeface.OpenStream(out var ttcIndex).ToHarfBuzzBlob();
                
                using var face = new Face(harfBuzzBlob, ttcIndex)
                {
                    Index = ttcIndex,
                    UnitsPerEm = typeface.UnitsPerEm,
                    GlyphCount = typeface.GlyphCount
                };
                
                var font = new Font(face);
                font.SetScale(TextShaper.FontShapingScale, TextShaper.FontShapingScale);
                font.SetFunctionsOpenType();

                return font;
            });
        }
        
        internal static TextShaper ToTextShaper(this TextStyle style)
        {
            return TextShapers.GetOrAdd(style.PaintKey, _ => new TextShaper(style));
        }
        
        internal static SKFont ToFont(this TextStyle style)
        {
            return Fonts.GetOrAdd(style.PaintKey, _ => style.ToPaint().ToFont());
        }
    }
}