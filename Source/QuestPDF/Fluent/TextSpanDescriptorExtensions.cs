using System;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TextSpanDescriptorExtensions
    {
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.style"]/*' />
        public static T Style<T>(this T descriptor, TextStyle style) where T : TextSpanDescriptor
        {
            if (style == null)
                return descriptor;
            
            descriptor.MutateTextStyle(TextStyleManager.OverrideStyle, style);
            return descriptor;
        }
        
        [Obsolete("This setting is not supported since the 2024.3 version. Please use the FontFamilyFallback method or rely on the new automated fallback mechanism.")]
        public static T Fallback<T>(this T descriptor, TextStyle? value = null) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Fallback, value);
            return descriptor;
        }
        
        [Obsolete("This setting is not supported since the 2024.3 version. Please use the FontFamilyFallback method or rely on the new automated fallback mechanism.")]
        public static T Fallback<T>(this T descriptor, Func<TextStyle, TextStyle> handler) where T : TextSpanDescriptor
        {
            return descriptor.Fallback(handler(TextStyle.Default));
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static T FontColor<T>(this T descriptor, Color color) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.FontColor, color);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.backgroundColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static T BackgroundColor<T>(this T descriptor, Color color) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.BackgroundColor, color);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFamily"]/*' />
        public static T FontFamily<T>(this T descriptor, string value) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.FontFamily, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFallback"]/*' />
        public static T FontFamilyFallback<T>(this T descriptor, string value) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.FontFamilyFallback, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontSize"]/*' />
        public static T FontSize<T>(this T descriptor, float value) where T : TextSpanDescriptor
        {
            if (value <= 0)
                throw new ArgumentException("Font size must be greater than 0.");
            
            descriptor.MutateTextStyle(TextStyleExtensions.FontSize, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.lineHeight"]/*' />
        public static T LineHeight<T>(this T descriptor, float factor) where T : TextSpanDescriptor
        {
            if (factor <= 0)
                throw new ArgumentException("Line height must be greater than 0.");
            
            descriptor.MutateTextStyle(TextStyleExtensions.LineHeight, factor);
            return descriptor;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.letterSpacing"]/*' />
        public static T LetterSpacing<T>(this T descriptor, float factor = 0) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.LetterSpacing, factor);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.italic"]/*' />
        public static T Italic<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Italic, value);
            return descriptor;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.wrapAnywhere"]/*' />
        [Obsolete("This setting is not supported since the 2024.3 version. This flag should be handled automatically by the layout engine.")]
        public static T WrapAnywhere<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            return descriptor;
        }
        
        #region Text Effects
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.strikethrough"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.remarks"]/*' />
        public static T Strikethrough<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Strikethrough, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.underline"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.remarks"]/*' />
        public static T Underline<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Underline, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.overline"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.remarks"]/*' />
        public static T Overline<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Overline, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.color"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationColor<T>(this T descriptor, Color color) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationColor, color);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.thickness"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationThickness<T>(this T descriptor, float factor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationThickness, factor);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.solid"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationSolid<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationSolid);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.double"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationDouble<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationDouble);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.wavy"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationWavy<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationWavy);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.dotted"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationDotted<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationDotted);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.dashed"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.decoration.applicability"]/*' />
        public static T DecorationDashed<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DecorationDashed);
            return descriptor;
        }
        
        #endregion

        #region Weight
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.thin"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T Thin<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Thin);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraLight"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T ExtraLight<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.ExtraLight);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.light"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T Light<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Light);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.normal"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T NormalWeight<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.NormalWeight);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.medium"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T Medium<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Medium);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.semiBold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T SemiBold<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.SemiBold);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.bold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T Bold<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Bold);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraBold"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T ExtraBold<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.ExtraBold);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.black"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T Black<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Black);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.extraBlack"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.weight.remarks"]/*' />
        public static T ExtraBlack<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.ExtraBlack);
            return descriptor;
        }

        #endregion

        #region Position
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.normal"]/*' />
        public static T NormalPosition<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.NormalPosition);
            return descriptor;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.subscript"]/*' />
        public static T Subscript<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Subscript);
            return descriptor;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.position.superscript"]/*' />
        public static T Superscript<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Superscript);
            return descriptor;
        }
        
        #endregion
        
        #region Direction

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.auto"]/*' />
        public static T DirectionAuto<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DirectionAuto);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.ltr"]/*' />
        public static T DirectionFromLeftToRight<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DirectionFromLeftToRight);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.direction.rtl"]/*' />
        public static T DirectionFromRightToLeft<T>(this T descriptor) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.DirectionFromRightToLeft);
            return descriptor;
        }

        #endregion
    }
}