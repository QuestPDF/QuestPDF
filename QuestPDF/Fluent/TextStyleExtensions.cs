using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TextStyleExtensions
    {
        private static TextStyle Mutate(this TextStyle style, Action<TextStyle> handler)
        {
            style = style.Clone();
            
            handler(style);
            return style;
        }
        
        public static TextStyle Color(this TextStyle style, string value)
        {
            return style.Mutate(x => x.Color = value);
        }
        
        public static TextStyle BackgroundColor(this TextStyle style, string value)
        {
            return style.Mutate(x => x.BackgroundColor = value);
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
        
        public static TextStyle Italic(this TextStyle style, bool value = true)
        {
            return style.Mutate(x => x.IsItalic = value);
        }
        
        public static TextStyle Strikethrough(this TextStyle style, bool value = true)
        {
            return style.Mutate(x => x.HasStrikethrough = value);
        }
        
        public static TextStyle Underline(this TextStyle style, bool value = true)
        {
            return style.Mutate(x => x.HasUnderline = value);
        }

        #region Weight
        
        public static TextStyle Weight(this TextStyle style, FontWeight weight)
        {
            return style.Mutate(x => x.FontWeight = weight);
        }
        
        public static TextStyle Thin(this TextStyle style)
        {
            return style.Weight(FontWeight.Thin);
        }
        
        public static TextStyle ExtraLight(this TextStyle style)
        {
            return style.Weight(FontWeight.ExtraLight);
        }
        
        public static TextStyle Light(this TextStyle style)
        {
            return style.Weight(FontWeight.Light);
        }
        
        public static TextStyle NormalWeight(this TextStyle style)
        {
            return style.Weight(FontWeight.Normal);
        }
        
        public static TextStyle Medium(this TextStyle style)
        {
            return style.Weight(FontWeight.Medium);
        }
        
        public static TextStyle SemiBold(this TextStyle style)
        {
            return style.Weight(FontWeight.SemiBold);
        }
        
        public static TextStyle Bold(this TextStyle style)
        {
            return style.Weight(FontWeight.Bold);
        }
        
        public static TextStyle ExtraBold(this TextStyle style)
        {
            return style.Weight(FontWeight.ExtraBold);
        }
        
        public static TextStyle Black(this TextStyle style)
        {
            return style.Weight(FontWeight.Black);
        }
        
        public static TextStyle ExtraBlack(this TextStyle style)
        {
            return style.Weight(FontWeight.ExtraBlack);
        }
        
        #endregion
    }
}