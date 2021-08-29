using QuestPDF.Helpers;
using System.Collections.Concurrent;

namespace QuestPDF.Infrastructure
{
    public class TextStyle
    {
        internal string Color { get; set; } = Colors.Black;
        internal string FontType { get; set; } = "Calibri";
        internal float Size { get; set; } = 12;
        internal float LineHeight { get; set; } = 1.2f;
        internal HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        internal FontWeight FontWeight { get; set; } = FontWeight.Normal;
        internal bool IsItalic { get; set; } = false;

        public static TextStyle Default => new TextStyle();

        internal static ConcurrentDictionary<string, SkiaSharp.SKTypeface> ConfiguredTypefaces = new ConcurrentDictionary<string, SkiaSharp.SKTypeface>();

        public static void ConfigureFontType(string fontType, SkiaSharp.SKTypeface typeFace)
        {
            ConfiguredTypefaces[fontType] = typeFace;
        }
        
        public override string ToString()
        {
            return $"{Color}|{FontType}|{Size}|{LineHeight}|{Alignment}|{FontWeight}|{IsItalic}";
        }

        internal TextStyle Clone() => (TextStyle)MemberwiseClone();
    }
}