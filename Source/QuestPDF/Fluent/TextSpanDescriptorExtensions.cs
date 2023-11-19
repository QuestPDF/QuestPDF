using System;
using QuestPDF.Helpers;
using QuestPDF.Drawing;
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
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFallback"]/*' />
        public static T Fallback<T>(this T descriptor, TextStyle? value = null) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Fallback, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFallback"]/*' />
        public static T Fallback<T>(this T descriptor, Func<TextStyle, TextStyle> handler) where T : TextSpanDescriptor
        {
            return descriptor.Fallback(handler(TextStyle.Default));
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static T FontColor<T>(this T descriptor, string color) where T : TextSpanDescriptor
        {
            ColorValidator.Validate(color);
            descriptor.MutateTextStyle(TextStyleExtensions.FontColor, color);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.backgroundColor"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static T BackgroundColor<T>(this T descriptor, string color) where T : TextSpanDescriptor
        {
            ColorValidator.Validate(color);
            descriptor.MutateTextStyle(TextStyleExtensions.BackgroundColor, color);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.fontFamily"]/*' />
        public static T FontFamily<T>(this T descriptor, string value) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.FontFamily, value);
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
        public static T LetterSpacing<T>(this T descriptor, float factor) where T : TextSpanDescriptor
        {
            if (factor <= 0)
                throw new ArgumentException("Letter spacing must be greater than 0.");
            
            descriptor.MutateTextStyle(TextStyleExtensions.LetterSpacing, factor);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.italic"]/*' />
        public static T Italic<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Italic, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.strikethrough"]/*' />
        public static T Strikethrough<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Strikethrough, value);
            return descriptor;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.underline"]/*' />
        public static T Underline<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.Underline, value);
            return descriptor;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.wrapAnywhere"]/*' />
        public static T WrapAnywhere<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.MutateTextStyle(TextStyleExtensions.WrapAnywhere, value);
            return descriptor;
        }

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