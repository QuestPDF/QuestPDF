using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Skia.Text;

namespace QuestPDF.Infrastructure
{
    public record TextStyle
    {
        internal int Id { get; set; }
        
        internal string? Color { get; set; }
        internal string? BackgroundColor { get; set; }
        internal string? FontFamily { get; set; }
        internal string? FontFamilyFallback { get; set; }
        internal float? Size { get; set; }
        internal float? LineHeight { get; set; }
        internal float? LetterSpacing { get; set; }
        internal FontWeight? FontWeight { get; set; }
        internal FontPosition? FontPosition { get; set; }
        internal bool? IsItalic { get; set; }
        internal bool? HasStrikethrough { get; set; }
        internal bool? HasUnderline { get; set; }
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
            FontFamily = Fonts.Lato,
            FontFamilyFallback = null,
            Size = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = Infrastructure.FontWeight.Normal,
            FontPosition = Infrastructure.FontPosition.Normal,
            IsItalic = false,
            HasStrikethrough = false,
            HasUnderline = false,
            Direction = TextDirection.Auto
        };

        private SkTextStyle? SkTextStyleCache;
        
        internal SkTextStyle GetSkTextStyle()
        {
            if (SkTextStyleCache != null)
                return SkTextStyleCache;
            
            SkTextStyleCache = new SkTextStyle(new TextStyleConfiguration
            {
                FontSize = CalculateTargetFontSize(),
                FontWeight = (TextStyleConfiguration.FontWeights?)FontWeight ?? TextStyleConfiguration.FontWeights.Normal,
                
                IsItalic = IsItalic ?? false,
                FontFamily = FontFamily,
                FontFamilyFallback = FontFamilyFallback,
                ForegroundColor = Color?.ColorToCode() ?? 0,
                BackgroundColor = BackgroundColor?.ColorToCode() ?? 0,
                DecorationColor = Color?.ColorToCode() ?? 0,
                DecorationType = CreateDecoration(),
                
                // TODO: create public API to support these properties
                DecorationMode = TextStyleConfiguration.TextDecorationMode.Gaps,
                DecorationStyle = TextStyleConfiguration.TextDecorationStyle.Solid,
                WordSpacing = 0,
                
                LineHeight = LineHeight ?? 1,
                LetterSpacing = LetterSpacing ?? 0,
                BaselineOffset = CalculateBaselineOffset(),
            });
            
            return SkTextStyleCache;

            TextStyleConfiguration.TextDecoration CreateDecoration()
            {
                var result = TextStyleConfiguration.TextDecoration.NoDecoration;
                
                if (HasUnderline == true)
                    result |= TextStyleConfiguration.TextDecoration.Underline;
                
                if (HasStrikethrough == true)
                    result |= TextStyleConfiguration.TextDecoration.LineThrough;
                
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