using System;
using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure
{
    public class TextStyle
    {
        internal bool HasGlobalStyleApplied { get; private set; }
        
        internal string? Color { get; set; }
        internal string? BackgroundColor { get; set; }
        internal string? FontType { get; set; }
        internal float? Size { get; set; }
        internal float? LineHeight { get; set; }
        internal FontWeight? FontWeight { get; set; }
        internal FontVariant? FontVariant { get; set; }
        internal bool? IsItalic { get; set; }
        internal bool? HasStrikethrough { get; set; }
        internal bool? HasUnderline { get; set; }

        internal object PaintKey { get; private set; }
        internal object FontMetricsKey { get; private set; }

        internal object NormalizedFontMetricsKey => (FontType, Size, FontWeight, Infrastructure.FontVariant.Normal, IsItalic);
        
        internal static TextStyle LibraryDefault => new TextStyle
        {
            Color = Colors.Black,
            BackgroundColor = Colors.Transparent,
            FontType = Fonts.Calibri,
            Size = 12,
            LineHeight = 1.2f,
            FontWeight = Infrastructure.FontWeight.Normal,
            FontVariant = Infrastructure.FontVariant.Normal,
            IsItalic = false,
            HasStrikethrough = false,
            HasUnderline = false
        };

        public static TextStyle Default => new TextStyle();
        
        internal void ApplyGlobalStyle(TextStyle globalStyle)
        {
            if (HasGlobalStyleApplied)
                return;
            
            HasGlobalStyleApplied = true;

            ApplyParentStyle(globalStyle);
            PaintKey ??= (FontType, Size, FontWeight, FontVariant, IsItalic, Color);
            FontMetricsKey ??= (FontType, Size, FontWeight, FontVariant, IsItalic);
        }
        
        internal void ApplyParentStyle(TextStyle parentStyle)
        {
            Color ??= parentStyle.Color;
            BackgroundColor ??= parentStyle.BackgroundColor;
            FontType ??= parentStyle.FontType;
            Size ??= parentStyle.Size;
            LineHeight ??= parentStyle.LineHeight;
            FontWeight ??= parentStyle.FontWeight;
            FontVariant ??= parentStyle.FontVariant;
            IsItalic ??= parentStyle.IsItalic;
            HasStrikethrough ??= parentStyle.HasStrikethrough;
            HasUnderline ??= parentStyle.HasUnderline;
        }

        internal TextStyle Clone()
        {
            var clone = (TextStyle)MemberwiseClone();
            clone.HasGlobalStyleApplied = false;
            return clone;
        }
    }
}