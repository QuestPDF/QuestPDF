using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure
{
    public class TextStyle
    {
        internal string Color { get; set; } = Colors.Black;
        internal string BackgroundColor { get; set; } = Colors.Transparent;
        internal string FontType { get; set; } = "Calibri";
        internal float Size { get; set; } = 12;
        internal float LineHeight { get; set; } = 1.2f;
        internal FontWeight FontWeight { get; set; } = FontWeight.Normal;
        internal bool IsItalic { get; set; } = false;
        internal bool IsStroked { get; set; } = false;
        internal bool IsUnderlined { get; set; } = false;

        public static TextStyle Default => new TextStyle();

        private string? KeyCache { get; set; }
        
        public override string ToString()
        {
            KeyCache ??= $"{Color}|{BackgroundColor}|{FontType}|{Size}|{LineHeight}|{FontWeight}|{IsItalic}|{IsStroked}|{IsUnderlined}";
            return KeyCache;
        }

        internal TextStyle Clone() => (TextStyle)MemberwiseClone();
    }
}