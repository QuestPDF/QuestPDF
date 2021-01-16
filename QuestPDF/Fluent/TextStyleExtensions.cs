using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TextStyleExtensions
    {
        private static TextStyle Mutate(this TextStyle style, Action<TextStyle> handler)
        {
            handler(style);
            return style;
        }
        
        public static TextStyle Color(this TextStyle style, string value)
        {
            return style.Mutate(x => x.Color = value);
        }
        
        public static TextStyle FontType(this TextStyle style, string value)
        {
            return style.Mutate(x => x.FontType = value);
        }
        
        public static TextStyle Size(this TextStyle style, float value)
        {
            return style.Mutate(x => x.Size = value);
        }
        
        public static TextStyle LineHeight(this TextStyle style, float value)
        {
            return style.Mutate(x => x.LineHeight = value);
        }
        
        public static TextStyle Alignment(this TextStyle style, HorizontalAlignment value)
        {
            return style.Mutate(x => x.Alignment = value);
        }
        
        public static TextStyle AlignLeft(this TextStyle style)
        {
            return style.Mutate(x => x.Alignment = HorizontalAlignment.Left);
        }
        
        public static TextStyle AlignCenter(this TextStyle style)
        {
            return style.Mutate(x => x.Alignment = HorizontalAlignment.Center);
        }
        
        public static TextStyle AlignRight(this TextStyle style)
        {
            return style.Mutate(x => x.Alignment = HorizontalAlignment.Right);
        }
    }
}