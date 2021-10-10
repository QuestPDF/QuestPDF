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
        internal bool? IsItalic { get; set; }
        internal bool? HasStrikethrough { get; set; }
        internal bool? HasUnderline { get; set; }

        internal string? Key { get; private set; }
        
        internal static TextStyle LibraryDefault => new TextStyle
        {
            Color = Colors.Black,
            BackgroundColor = Colors.Transparent,
            FontType = Fonts.Calibri,
            Size = 12,
            LineHeight = 1.2f,
            FontWeight = Infrastructure.FontWeight.Normal,
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
            Key ??= $"{Color}|{BackgroundColor}|{FontType}|{Size}|{LineHeight}|{FontWeight}|{IsItalic}|{HasStrikethrough}|{HasUnderline}";
        }
        
        internal void ApplyParentStyle(TextStyle parentStyle)
        {
            Color ??= parentStyle.Color;
            BackgroundColor ??= parentStyle.BackgroundColor;
            FontType ??= parentStyle.FontType;
            Size ??= parentStyle.Size;
            LineHeight ??= parentStyle.LineHeight;
            FontWeight ??= parentStyle.FontWeight;
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