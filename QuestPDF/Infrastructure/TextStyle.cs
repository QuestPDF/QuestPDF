using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure
{
    public class TextStyle
    {
        internal string Color { get; set; } = Colors.Black;
        internal string BackgroundColor { get; set; } = Colors.Transparent;
        internal string FontType { get; set; } = "Calibri";
        internal float Size { get; set; } = 12;
        internal float LineHeight { get; set; } = 1f;
        internal HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        internal FontWeight FontWeight { get; set; } = FontWeight.Normal;
        internal bool IsItalic { get; set; } = false;
        internal bool IsDense { get; set; }
        
        public static TextStyle Default => new TextStyle();
        
        public override string ToString()
        {
            return $"{Color}|{BackgroundColor}|{FontType}|{Size}|{LineHeight}|{Alignment}|{FontWeight}|{IsItalic}|{IsDense}";
        }

        internal TextStyle Clone() => (TextStyle)MemberwiseClone();
    }
}