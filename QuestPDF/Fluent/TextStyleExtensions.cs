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
        
        public static TextStyle Italic(this TextStyle style, bool value = true)
        {
            return style.Mutate(x => x.IsItalic = value);
        }

        #region Alignmnet
        
        public static TextStyle Alignment(this TextStyle style, HorizontalAlignment value)
        {
            return style.Mutate(x => x.Alignment = value);
        }
        
        public static TextStyle AlignLeft(this TextStyle style)
        {
            return style.Alignment(HorizontalAlignment.Left);
        }
        
        public static TextStyle AlignCenter(this TextStyle style)
        {
            return style.Alignment(HorizontalAlignment.Center);
        }
        
        public static TextStyle AlignRight(this TextStyle style)
        {
            return style.Alignment(HorizontalAlignment.Right);
        }
        
        #endregion
        
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