using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Infrastructure
{
    public record TextStyle
    {
        internal int Id { get; set; }
        
        internal Color? Color { get; set; }
        internal Color? BackgroundColor { get; set; }
        internal Color? DecorationColor { get; set; }
        internal string[]? FontFamilies { get; set; }
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
            FontFamilies = new[] { Fonts.Lato },
            Size = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            WordSpacing = 0f,
            FontWeight = Infrastructure.FontWeight.Normal,
            FontPosition = Infrastructure.FontPosition.Normal,
            IsItalic = false,
            HasStrikethrough = false,
            HasUnderline = false,
            HasOverline = false,
            DecorationStyle = TextStyleConfiguration.TextDecorationStyle.Solid,
            DecorationThickness = 2f,
            Direction = TextDirection.Auto
        };
        
        internal static TextStyle ParagraphSpacing { get; } = LibraryDefault with
        {
            Id = 2,
            Size = 0,
            LineHeight = 1
        };

        private SkTextStyle? SkTextStyleCache;
        
        internal SkTextStyle GetSkTextStyle()
        {
            if (SkTextStyleCache != null)
                return SkTextStyleCache;
            
            var fontFamilyTexts = FontFamilies.Select(x => new SkText(x)).ToList();

            SkTextStyleCache = new SkTextStyle(new TextStyleConfiguration
            {
                FontSize = CalculateTargetFontSize(),
                FontWeight = (TextStyleConfiguration.FontWeights?)FontWeight ?? TextStyleConfiguration.FontWeights.Normal,
                
                IsItalic = IsItalic ?? false,
                FontFamilies = GetFontFamilyPointers(fontFamilyTexts),
                ForegroundColor = Color ?? Colors.Black,
                BackgroundColor = BackgroundColor ?? Colors.Transparent,
                DecorationColor = DecorationColor ?? Colors.Black,
                DecorationType = CreateDecoration(),
                
                DecorationMode = TextStyleConfiguration.TextDecorationMode.Through,
                DecorationStyle = DecorationStyle ?? TextStyleConfiguration.TextDecorationStyle.Solid,
                DecorationThickness = DecorationThickness ?? 1,
                
                LineHeight = LineHeight ?? 1,
                LetterSpacing = (LetterSpacing ?? 0) * (Size ?? 1),
                WordSpacing = (WordSpacing ?? 0) * (Size ?? 1),
                BaselineOffset = CalculateBaselineOffset(),
            });
            
            fontFamilyTexts.ForEach(x => x.Dispose());
            return SkTextStyleCache;

            IntPtr[] GetFontFamilyPointers(IList<SkText> texts)
            {
                var result = new IntPtr[TextStyleConfiguration.FONT_FAMILIES_LENGTH];
                
                for (var i = 0; i < Math.Min(result.Length, texts.Count); i++)
                    result[i] = texts[i].Instance;
                
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