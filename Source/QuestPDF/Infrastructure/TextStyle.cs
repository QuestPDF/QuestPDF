using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

using TextStyleFontFeature = (string Name, bool Enabled);

namespace QuestPDF.Infrastructure
{
    public record TextStyle
    {
        internal const float NormalLineHeightCalculatedFromFontMetrics = 0;
        
        internal int Id { get; set; }
        
        internal Color? Color { get; set; }
        internal Color? BackgroundColor { get; set; }
        internal Color? DecorationColor { get; set; }
        internal string[]? FontFamilies { get; set; }
        internal TextStyleFontFeature[]? FontFeatures { get; set; }
        internal float? Size { get; set; }
        internal float? LineHeight { get; set; }
        internal float? LetterSpacing { get; set; }
        internal float? WordSpacing { get; set; }
        internal FontWeight? FontWeight { get; set; }
        internal FontPosition? FontPosition { get; set; }
        internal bool? IsItalic { get; set; }
        internal bool? HasStrikethrough { get; set; }
        internal bool? HasUnderline { get; set; }
        internal bool? HasOverline { get; set; }
        internal TextStyleConfiguration.TextDecorationStyle? DecorationStyle { get; set; }
        internal float? DecorationThickness { get; set; }
        internal TextDirection? Direction { get; set; }

        ~TextStyle()
        {
            // TextStyle is meant to be an object spanning the entire application lifetime
            // It does not require the IDisposable pattern
            SkTextStyleCache?.Dispose();
        }

        public static TextStyle Default { get; } = new()
        {
            Id = 0
        };
        
        internal static TextStyle LibraryDefault { get; } = new()
        {
            Id = 1,
            Color = Colors.Black,
            BackgroundColor = Colors.Transparent,
            DecorationColor = Colors.Black,
            FontFamilies = [ Fonts.Lato ],
            FontFeatures = [],
            Size = 12,
            LineHeight = NormalLineHeightCalculatedFromFontMetrics,
            LetterSpacing = 0,
            WordSpacing = 0f,
            FontWeight = Infrastructure.FontWeight.Normal,
            FontPosition = Infrastructure.FontPosition.Normal,
            IsItalic = false,
            HasStrikethrough = false,
            HasUnderline = false,
            HasOverline = false,
            DecorationStyle = TextStyleConfiguration.TextDecorationStyle.Solid,
            DecorationThickness = 1f,
            Direction = TextDirection.Auto
        };
        
        internal static TextStyle ParagraphSpacing { get; } = LibraryDefault with
        {
            Id = 2,
            Size = 0,
            LineHeight = 1
        };

        private volatile SkTextStyle? SkTextStyleCache;
        private readonly object SkTextStyleCacheLock = new();
        
        internal SkTextStyle GetSkTextStyle()
        {
            if (SkTextStyleCache != null)
                return SkTextStyleCache;

            lock (SkTextStyleCacheLock)
            {
                if (SkTextStyleCache != null)
                    return SkTextStyleCache;

                var temp = CreateSkTextStyle();
                SkTextStyleCache = temp;
                return temp;
            }
        }

        private SkTextStyle CreateSkTextStyle()
        {
            var fontFamilyTexts = FontFamilies.Select(x => new SkText(x)).ToList();

            var result = new SkTextStyle(new TextStyleConfiguration
            {
                FontSize = CalculateTargetFontSize(),
                FontWeight = (TextStyleConfiguration.FontWeights?)FontWeight ?? TextStyleConfiguration.FontWeights.Normal,
                IsItalic = IsItalic ?? false,
                
                FontFamilies = GetFontFamilyPointers(fontFamilyTexts),
                FontFeatures = GetFontFeatures(),
                
                ForegroundColor = Color ?? Colors.Black,
                BackgroundColor = BackgroundColor ?? Colors.Transparent,
                DecorationColor = DecorationColor ?? Colors.Black,
                DecorationType = CreateDecoration(),
                
                DecorationMode = TextStyleConfiguration.TextDecorationMode.Through,
                DecorationStyle = DecorationStyle ?? TextStyleConfiguration.TextDecorationStyle.Solid,
                DecorationThickness = DecorationThickness ?? 1,
                
                LineHeight = LineHeight ?? NormalLineHeightCalculatedFromFontMetrics,
                LetterSpacing = (LetterSpacing ?? 0) * (Size ?? 1),
                WordSpacing = (WordSpacing ?? 0) * (Size ?? 1),
                BaselineOffset = CalculateBaselineOffset(),
            });
            
            fontFamilyTexts.ForEach(x => x.Dispose());
            return result;

            IntPtr[] GetFontFamilyPointers(IList<SkText> texts)
            {
                var result = new IntPtr[TextStyleConfiguration.FONT_FAMILIES_LENGTH];
                
                for (var i = 0; i < Math.Min(result.Length, texts.Count); i++)
                    result[i] = texts[i].Instance;
                
                return result;
            }
            
            TextStyleConfiguration.FontFeature[] GetFontFeatures(params (string name, int value)[] features)
            {
                var result = new TextStyleConfiguration.FontFeature[TextStyleConfiguration.FONT_FEATURES_LENGTH];

                foreach (var (feature, index) in FontFeatures.Take(TextStyleConfiguration.FONT_FEATURES_LENGTH).Select((x, i) => (x, i)))
                {
                    result[index].Name = feature.Name;
                    result[index].Value = feature.Enabled ? 1 : 0;
                }
                
                return result;
            }

            TextStyleConfiguration.TextDecoration CreateDecoration()
            {
                var result = TextStyleConfiguration.TextDecoration.NoDecoration;
                
                if (HasUnderline == true)
                    result |= TextStyleConfiguration.TextDecoration.Underline;
                
                if (HasStrikethrough == true)
                    result |= TextStyleConfiguration.TextDecoration.LineThrough;
                
                if (HasOverline == true)
                    result |= TextStyleConfiguration.TextDecoration.Overline;
                
                return result;
            }

            float CalculateTargetFontSize()
            {
                var fontSize = Size ?? 0;
                
                if (FontPosition is Infrastructure.FontPosition.Subscript or Infrastructure.FontPosition.Superscript)
                    return fontSize * 0.6f;
   
                return fontSize;
            }
            
            float CalculateBaselineOffset()
            {
                if (FontPosition == Infrastructure.FontPosition.Subscript)
                    return Size.Value * 0.25f;
                
                if (FontPosition == Infrastructure.FontPosition.Superscript)
                    return -Size.Value * 0.35f;

                return 0;
            }
        }
    }
}