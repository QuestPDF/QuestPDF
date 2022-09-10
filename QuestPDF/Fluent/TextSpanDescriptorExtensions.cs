using System;
using System.Runtime.CompilerServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TextSpanDescriptorExtensions
    {
        public static T Style<T>(this T descriptor, TextStyle style) where T : TextSpanDescriptor
        {
            if (style == null)
                return descriptor;
            
            descriptor.TextStyle.OverrideStyle(style);
            return descriptor;
        }
        
        public static T Fallback<T>(this T descriptor, TextStyle? value = null) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.Fallback = value;
            return descriptor;
        }
        
        public static T Fallback<T>(this T descriptor, Func<TextStyle, TextStyle> handler) where T : TextSpanDescriptor
        {
            return descriptor.Fallback(handler(TextStyle.Default));
        }
        
        public static T FontColor<T>(this T descriptor, string value) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.Color = value;
            return descriptor;
        }
        
        public static T BackgroundColor<T>(this T descriptor, string value) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.BackgroundColor = value;
            return descriptor;
        }
        
        public static T FontFamily<T>(this T descriptor, string value) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.FontFamily = value;
            return descriptor;
        }
        
        public static T FontSize<T>(this T descriptor, float value) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.Size = value;
            return descriptor;
        }
        
        public static T LineHeight<T>(this T descriptor, float value) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.LineHeight = value;
            return descriptor;
        }
        
        public static T Italic<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.IsItalic = value;
            return descriptor;
        }
        
        public static T Strikethrough<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.HasStrikethrough = value;
            return descriptor;
        }
        
        public static T Underline<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.HasUnderline = value;
            return descriptor;
        }

        public static T WrapAnywhere<T>(this T descriptor, bool value = true) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.WrapAnywhere = value;
            return descriptor;
        }

        #region Weight
        
        public static T Weight<T>(this T descriptor, FontWeight weight) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.FontWeight = weight;
            return descriptor;
        }
        
        public static T Thin<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.Thin);
        }
        
        public static T ExtraLight<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.ExtraLight);
        }
        
        public static T Light<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.Light);
        }
        
        public static T NormalWeight<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.Normal);
        }
        
        public static T Medium<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.Medium);
        }
        
        public static T SemiBold<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.SemiBold);
        }
        
        public static T Bold<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.Bold);
        }
        
        public static T ExtraBold<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.ExtraBold);
        }
        
        public static T Black<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.Black);
        }
        
        public static T ExtraBlack<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Weight(FontWeight.ExtraBlack);
        }

        #endregion

        #region Position
        public static T NormalPosition<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Position(FontPosition.Normal);
        }

        public static T Subscript<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Position(FontPosition.Subscript);
        }

        public static T Superscript<T>(this T descriptor) where T : TextSpanDescriptor
        {
            return descriptor.Position(FontPosition.Superscript);
        }

        private static T Position<T>(this T descriptor, FontPosition fontPosition) where T : TextSpanDescriptor
        {
            descriptor.TextStyle.FontPosition = fontPosition;
            return descriptor;
        }
        #endregion
    }
}