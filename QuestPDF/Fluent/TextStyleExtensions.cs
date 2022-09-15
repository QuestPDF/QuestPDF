using System;
using System.ComponentModel;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TextStyleExtensions
    {
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontColor method.")]
        public static TextStyle Color(this TextStyle style, string value)
        {
            return style.FontColor(value);
        }
        
        public static TextStyle FontColor(this TextStyle style, string value)
        {
            return style.Mutate(TextStyleProperty.Color, value);
        }
        
        public static TextStyle BackgroundColor(this TextStyle style, string value)
        {
            return style.Mutate(TextStyleProperty.BackgroundColor, value);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontFamily method.")]
        public static TextStyle FontType(this TextStyle style, string value)
        {
            return style.FontFamily(value);
        }
        
        public static TextStyle FontFamily(this TextStyle style, string value)
        {
            return style.Mutate(TextStyleProperty.FontFamily, value);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontSize method.")]
        public static TextStyle Size(this TextStyle style, float value)
        {
            return style.FontSize(value);
        }
        
        public static TextStyle FontSize(this TextStyle style, float value)
        {
            return style.Mutate(TextStyleProperty.Size, value);
        }
        
        public static TextStyle LineHeight(this TextStyle style, float value)
        {
            return style.Mutate(TextStyleProperty.LineHeight, value);
        }
        
        public static TextStyle Italic(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.IsItalic, value);
        }
        
        public static TextStyle Strikethrough(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.HasStrikethrough, value);
        }
        
        public static TextStyle Underline(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.HasUnderline, value);
        }
        
        public static TextStyle WrapAnywhere(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.WrapAnywhere, value);
        }

        #region Weight
        
        public static TextStyle Weight(this TextStyle style, FontWeight weight)
        {
            return style.Mutate(TextStyleProperty.FontWeight, weight);
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

        #region Position
        
        public static TextStyle NormalPosition(this TextStyle style)
        {
            return style.Position(FontPosition.Normal);
        }

        public static TextStyle Subscript(this TextStyle style)
        {
            return style.Position(FontPosition.Subscript);
        }

        public static TextStyle Superscript(this TextStyle style)
        {
            return style.Position(FontPosition.Superscript);
        }

        private static TextStyle Position(this TextStyle style, FontPosition fontPosition)
        {
            return style.Mutate(TextStyleProperty.FontPosition, fontPosition);
        }
        
        #endregion

        #region Fallback
        
        public static TextStyle Fallback(this TextStyle style, TextStyle? value = null)
        {
            return style.Mutate(TextStyleProperty.Fallback, value);
        }
        
        public static TextStyle Fallback(this TextStyle style, Func<TextStyle, TextStyle> handler)
        {
            return style.Fallback(handler(TextStyle.Default));
        }

        #endregion
    }
}