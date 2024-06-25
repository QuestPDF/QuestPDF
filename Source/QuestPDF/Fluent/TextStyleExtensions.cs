using System;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia.Text;

namespace QuestPDF.Fluent
{
    public static class TextStyleExtensions
    {
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontColor method.")]
        public static TextStyle Color(this TextStyle style, string value)
        {
            return style.FontColor(value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static TextStyle FontColor(this TextStyle style, Color color)
        {
            return style
                .Mutate(TextStyleProperty.Color, color)
                .Mutate(TextStyleProperty.DecorationColor, color);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.backgroundColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static TextStyle BackgroundColor(this TextStyle style, Color color)
        {
            return style.Mutate(TextStyleProperty.BackgroundColor, color);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontFamily method.")]
        public static TextStyle FontType(this TextStyle style, string value)
        {
            return style.FontFamily(value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFamily"]/*' />
        public static TextStyle FontFamily(this TextStyle style, params string[] values)
        {
            return style.Mutate(TextStyleProperty.FontFamilies, values);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the FontSize method.")]
        public static TextStyle Size(this TextStyle style, float value)
        {
            return style.FontSize(value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontSize"]/*' />
        public static TextStyle FontSize(this TextStyle style, float value)
        {
            if (value <= 0)
                throw new ArgumentException("Font size must be greater than 0.");
            
            return style.Mutate(TextStyleProperty.Size, value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.lineHeight"]/*' />
        public static TextStyle LineHeight(this TextStyle style, float? factor)
        {
            if (factor <= 0)
                throw new ArgumentException("Line height must be greater than 0.");
            
            factor ??= TextStyle.NormalLineHeightCalculatedFromFontMetrics;
            return style.Mutate(TextStyleProperty.LineHeight, factor);
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.letterSpacing"]/*' />
        public static TextStyle LetterSpacing(this TextStyle style, float factor = 0)
        {
            return style.Mutate(TextStyleProperty.LetterSpacing, factor);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.wordSpacing"]/*' />
        public static TextStyle WordSpacing(this TextStyle style, float factor = 0)
        {
            return style.Mutate(TextStyleProperty.WordSpacing, factor);
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.italic"]/*' />
        public static TextStyle Italic(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.IsItalic, value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.wrapAnywhere"]/*' />
        [Obsolete("This setting is not supported since the 2024.3 version. This flag should be handled automatically by the layout engine.")]
        public static TextStyle WrapAnywhere(this TextStyle style, bool value = true)
        {
            return style;
        }

        #region Text Effects
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.strikethrough"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.remarks"]/*' />
        public static TextStyle Strikethrough(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.HasStrikethrough, value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.underline"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.remarks"]/*' />
        public static TextStyle Underline(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.HasUnderline, value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.overline"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.remarks"]/*' />
        public static TextStyle Overline(this TextStyle style, bool value = true)
        {
            return style.Mutate(TextStyleProperty.HasOverline, value);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.color"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationColor(this TextStyle style, Color color)
        {
            return style.Mutate(TextStyleProperty.DecorationColor, color);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.thickness"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationThickness(this TextStyle style, float factor)
        {
            return style.Mutate(TextStyleProperty.DecorationThickness, factor);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.solid"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationSolid(this TextStyle style)
        {
            return style.Mutate(TextStyleProperty.DecorationStyle, TextStyleConfiguration.TextDecorationStyle.Solid);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.double"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationDouble(this TextStyle style)
        {
            return style.Mutate(TextStyleProperty.DecorationStyle, TextStyleConfiguration.TextDecorationStyle.Double);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.wavy"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationWavy(this TextStyle style)
        {
            return style.Mutate(TextStyleProperty.DecorationStyle, TextStyleConfiguration.TextDecorationStyle.Wavy);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.dotted"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationDotted(this TextStyle style)
        {
            return style.Mutate(TextStyleProperty.DecorationStyle, TextStyleConfiguration.TextDecorationStyle.Dotted);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.dashed"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static TextStyle DecorationDashed(this TextStyle style)
        {
            return style.Mutate(TextStyleProperty.DecorationStyle, TextStyleConfiguration.TextDecorationStyle.Dashed);
        }
        
        #endregion
        
        #region Weight
        
        public static TextStyle Weight(this TextStyle style, FontWeight weight)
        {
            return style.Mutate(TextStyleProperty.FontWeight, weight);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.thin"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Thin(this TextStyle style)
        {
            return style.Weight(FontWeight.Thin);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraLight"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle ExtraLight(this TextStyle style)
        {
            return style.Weight(FontWeight.ExtraLight);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.light"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Light(this TextStyle style)
        {
            return style.Weight(FontWeight.Light);
        }
       
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.normal"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle NormalWeight(this TextStyle style)
        {
            return style.Weight(FontWeight.Normal);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.medium"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Medium(this TextStyle style)
        {
            return style.Weight(FontWeight.Medium);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.semiBold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle SemiBold(this TextStyle style)
        {
            return style.Weight(FontWeight.SemiBold);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.bold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Bold(this TextStyle style)
        {
            return style.Weight(FontWeight.Bold);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraBold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle ExtraBold(this TextStyle style)
        {
            return style.Weight(FontWeight.ExtraBold);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.black"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle Black(this TextStyle style)
        {
            return style.Weight(FontWeight.Black);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraBlack"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static TextStyle ExtraBlack(this TextStyle style)
        {
            return style.Weight(FontWeight.ExtraBlack);
        }

        #endregion

        #region Position
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.normal"]/*' />
        public static TextStyle NormalPosition(this TextStyle style)
        {
            return style.Position(FontPosition.Normal);
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.subscript"]/*' />
        public static TextStyle Subscript(this TextStyle style)
        {
            return style.Position(FontPosition.Subscript);
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.superscript"]/*' />
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
        
        [Obsolete("This setting is not supported since the 2024.3 version. Please use the FontFamilyFallback method or rely on the new automated fallback mechanism.")]
        public static TextStyle Fallback(this TextStyle style, TextStyle? value = null)
        {
            var currentFontFamilies = style.FontFamilies ?? Array.Empty<string>();
            var fallbackFontFamilies = value?.FontFamilies ?? Array.Empty<string>();
            
            var targetFontFamilies = currentFontFamilies
                .Concat(fallbackFontFamilies)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();
            
            return style.FontFamily(targetFontFamilies);
        }
        
        [Obsolete("This setting is not supported since the 2024.3 version. Please use the FontFamilyFallback method or rely on the new automated fallback mechanism.")]
        public static TextStyle Fallback(this TextStyle style, Func<TextStyle, TextStyle> handler)
        {
            return style.Fallback(handler(TextStyle.Default));
        }

        #endregion

        #region Direction

        private static TextStyle TextDirection(this TextStyle style, TextDirection textDirection)
        {
            return style.Mutate(TextStyleProperty.Direction, textDirection);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.auto"]/*' />
        public static TextStyle DirectionAuto(this TextStyle style)
        {
            return style.TextDirection(Infrastructure.TextDirection.Auto);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.ltr"]/*' />
        public static TextStyle DirectionFromLeftToRight(this TextStyle style)
        {
            return style.TextDirection(Infrastructure.TextDirection.LeftToRight);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.rtl"]/*' />
        public static TextStyle DirectionFromRightToLeft(this TextStyle style)
        {
            return style.TextDirection(Infrastructure.TextDirection.RightToLeft);
        }

        #endregion

        #region Font Features
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFeatures"]/*' />
        public static TextStyle EnableFontFeature(this TextStyle style, string featureName)
        {
            return style.Mutate(TextStyleProperty.FontFeatures, new[] { (featureName, true) });
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFeatures"]/*' />
        public static TextStyle DisableFontFeature(this TextStyle style, string featureName)
        {
            return style.Mutate(TextStyleProperty.FontFeatures, new[] { (featureName, false) });
        }
        
        #endregion
    }
}