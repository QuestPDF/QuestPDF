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

        private static TextStyle DefaultTextStyleCache = new TextStyle();
        public static TextStyle Default => DefaultTextStyleCache;

        internal void ApplyGlobalStyle(TextStyle global)
        {
            if (HasGlobalStyleApplied)
                return;
            
            HasGlobalStyleApplied = true;

            Color ??= global.Color;
            BackgroundColor ??= global.BackgroundColor;
            FontType ??= global.FontType;
            Size ??= global.Size;
            LineHeight ??= global.LineHeight;
            FontWeight ??= global.FontWeight;
            IsItalic ??= global.IsItalic;
            HasStrikethrough ??= global.HasStrikethrough;
            HasUnderline ??= global.HasUnderline;
            
            Key ??= $"{Color}|{BackgroundColor}|{FontType}|{Size}|{LineHeight}|{FontWeight}|{IsItalic}|{HasStrikethrough}|{HasUnderline}";
        }

        internal TextStyle Clone()
        {
            var clone = (TextStyle)MemberwiseClone();
            clone.HasGlobalStyleApplied = false;
            return clone;
        }
    }
}