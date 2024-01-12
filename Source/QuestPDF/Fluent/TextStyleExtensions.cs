using System;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TextStyleExtensions
    {
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontColor method.")]
        public static TextStyle Color(this TextStyle style, string value)
            => style.FontColor(value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static TextStyle FontColor(this TextStyle style, string color)
        {
            ColorValidator.Validate(color);
            return style.Mutate(TextStyleProperty.Color, color);
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.backgroundColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static TextStyle BackgroundColor(this TextStyle style, string color)
        {
            ColorValidator.Validate(color);
            return style.Mutate(TextStyleProperty.BackgroundColor, color);
        }

        [Obsolete("This element has been renamed since version 2022.3. Please use the FontFamily method.")]
        public static TextStyle FontType(this TextStyle style, string value)
            => style.FontFamily(value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFamily"]/*' />
        public static TextStyle FontFamily(this TextStyle style, string value)
            => style.Mutate(TextStyleProperty.FontFamily, value);

        [Obsolete("This element has been renamed since version 2022.3. Please use the FontSize method.")]
        public static TextStyle Size(this TextStyle style, float value)
            => style.FontSize(value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontSize"]/*' />
        public static TextStyle FontSize(this TextStyle style, float value)
            => value switch
            {
                <= 0 => throw new ArgumentException("Font size must be greater than 0."),
                _ => style.Mutate(TextStyleProperty.Size, value),
            };

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.lineHeight"]/*' />
        public static TextStyle LineHeight(this TextStyle style, float factor = 1)
            => factor switch
            {
                <= 0 => throw new ArgumentException("Line height must be greater than 0."),
                _ => style.Mutate(TextStyleProperty.LineHeight, factor),
            };

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.letterSpacing"]/*' />
        public static TextStyle LetterSpacing(this TextStyle style, float factor = 1)
            => factor switch
            {
                <= 0 => throw new ArgumentException("Letter spacing must be greater than 0."),
                _ => style.Mutate(TextStyleProperty.LetterSpacing, factor),
            };

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.italic"]/*' />
        public static TextStyle Italic(this TextStyle style, bool value = true)
            => style.Mutate(TextStyleProperty.IsItalic, value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.strikethrough"]/*' />
        public static TextStyle Strikethrough(this TextStyle style, bool value = true)
            => style.Mutate(TextStyleProperty.HasStrikethrough, value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.underline"]/*' />
        public static TextStyle Underline(this TextStyle style, bool value = true)
            => style.Mutate(TextStyleProperty.HasUnderline, value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.wrapAnywhere"]/*' />
        public static TextStyle WrapAnywhere(this TextStyle style, bool value = true)
            => style.Mutate(TextStyleProperty.WrapAnywhere, value);

        #region Weight

        public static TextStyle Weight(this TextStyle style, FontWeight weight)
            => style.Mutate(TextStyleProperty.FontWeight, weight);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.thin"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Thin(this TextStyle style)
            => style.Weight(FontWeight.Thin);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraLight"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle ExtraLight(this TextStyle style)
            => style.Weight(FontWeight.ExtraLight);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.light"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Light(this TextStyle style)
            => style.Weight(FontWeight.Light);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.normal"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle NormalWeight(this TextStyle style)
            => style.Weight(FontWeight.Normal);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.medium"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Medium(this TextStyle style)
            => style.Weight(FontWeight.Medium);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.semiBold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle SemiBold(this TextStyle style)
            => style.Weight(FontWeight.SemiBold);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.bold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Bold(this TextStyle style)
            => style.Weight(FontWeight.Bold);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraBold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle ExtraBold(this TextStyle style)
            => style.Weight(FontWeight.ExtraBold);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.black"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Black(this TextStyle style)
            => style.Weight(FontWeight.Black);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraBlack"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle ExtraBlack(this TextStyle style)
            => style.Weight(FontWeight.ExtraBlack);

        #endregion

        #region Position

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.normal"]/*' />
        public static TextStyle NormalPosition(this TextStyle style)
            => style.Position(FontPosition.Normal);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.subscript"]/*' />
        public static TextStyle Subscript(this TextStyle style)
            => style.Position(FontPosition.Subscript);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.superscript"]/*' />
        public static TextStyle Superscript(this TextStyle style)
            => style.Position(FontPosition.Superscript);

        private static TextStyle Position(this TextStyle style, FontPosition fontPosition)
            => style.Mutate(TextStyleProperty.FontPosition, fontPosition);

        #endregion

        #region Fallback

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFallback"]/*' />
        public static TextStyle Fallback(this TextStyle style, TextStyle? value = null)
            => style.Mutate(TextStyleProperty.Fallback, value);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFallback"]/*' />
        public static TextStyle Fallback(this TextStyle style, Func<TextStyle, TextStyle> handler)
            => style.Fallback(handler(TextStyle.Default));

        #endregion

        #region Direction

        private static TextStyle TextDirection(this TextStyle style, TextDirection textDirection)
            => style.Mutate(TextStyleProperty.Direction, textDirection);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.auto"]/*' />
        public static TextStyle DirectionAuto(this TextStyle style)
            => style.TextDirection(Infrastructure.TextDirection.Auto);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.ltr"]/*' />
        public static TextStyle DirectionFromLeftToRight(this TextStyle style)
            => style.TextDirection(Infrastructure.TextDirection.LeftToRight);

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.rtl"]/*' />
        public static TextStyle DirectionFromRightToLeft(this TextStyle style)
            => style.TextDirection(Infrastructure.TextDirection.RightToLeft);

        #endregion
    }
}