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
    }
}