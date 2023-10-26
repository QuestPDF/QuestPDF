using System;
using System.Collections.Concurrent;
using System.Linq;

namespace QuestPDF.Infrastructure
{
    internal enum TextStyleProperty
    {
        Color,
        BackgroundColor,
        FontFamily,
        Size,
        LineHeight,
        LetterSpacing,
        FontWeight,
        FontPosition,
        IsItalic,
        HasStrikethrough,
        HasUnderline,
        WrapAnywhere,
        Fallback,
        Direction
    }
    
    internal record StylesApplyingOptions
    {
        public bool OverrideStyle { get; set; }
            
        public bool OverrideFontFamily { get; set; }
            
        public bool AllowFallback { get; set; }
    }
    
    internal static class TextStyleManager
    {
        private static readonly ConcurrentDictionary<(TextStyle Origin, TextStyleProperty Property, object Value), TextStyle> TextStyleMutateCache = new();
        private static readonly ConcurrentDictionary<(TextStyle Origin, TextStyle Parent), TextStyle> TextStyleApplyInheritedCache = new();
        private static readonly ConcurrentDictionary<TextStyle, TextStyle> TextStyleApplyGlobalCache = new();
        private static readonly ConcurrentDictionary<(TextStyle Origin, TextStyle Parent), TextStyle> TextStyleOverrideCache = new();

        private static readonly TextStyleProperty[] AvailableStyleProperties =
            Enum.GetValues(typeof(TextStyleProperty))
                .OfType<TextStyleProperty>()
                .ToArray();

        public static TextStyle Mutate(this TextStyle origin, TextStyleProperty property, object value)
        {
            var cacheKey = (origin, property, value);
            return TextStyleMutateCache.GetOrAdd(cacheKey, tuple => tuple.Origin.MutateStyle(tuple.Property, tuple.Value, overrideValue: true));
        }

        private static TextStyle MutateStyle(this TextStyle origin, TextStyleProperty property, object? newValue, bool overrideValue)
        {
            if (overrideValue && newValue is null)
                return origin;

            var oldValue = origin.GetValue(property);

            if (!overrideValue && oldValue is not null)
                return origin;

            if (oldValue == newValue) 
                return origin;

            var textStyleClone = origin with { };
            textStyleClone.SetValue(property, newValue);
            
            return textStyleClone;
        }

        internal static TextStyle ApplyInheritedStyle(this TextStyle style, TextStyle parent)
        {
            var cacheKey = (style, parent);
            var options = new StylesApplyingOptions
            {
                OverrideStyle = false,
                OverrideFontFamily = false,
                AllowFallback = true
            };
            
            return TextStyleApplyInheritedCache.GetOrAdd(cacheKey, tuple => tuple.Origin.ApplyStyleProperties(tuple.Parent, options).UpdateFontFallback(overrideStyle: true));
        }
        
        internal static TextStyle ApplyGlobalStyle(this TextStyle style)
        {
            var options = new StylesApplyingOptions
            {
                OverrideStyle = false,
                OverrideFontFamily = false,
                AllowFallback = true
            };
            
            return TextStyleApplyGlobalCache.GetOrAdd(style, tuple => tuple.ApplyStyleProperties(TextStyle.LibraryDefault, options).UpdateFontFallback(overrideStyle: false));
        }
        
        private static TextStyle UpdateFontFallback(this TextStyle style, bool overrideStyle)
        {
            var options = new StylesApplyingOptions
            {
                OverrideStyle = overrideStyle,
                OverrideFontFamily = false,
                AllowFallback = false
            };
            
            var targetFallbackStyle = style
                .Fallback?
                .ApplyStyleProperties(style, options)
                .UpdateFontFallback(overrideStyle);
            
            return style.MutateStyle(TextStyleProperty.Fallback, targetFallbackStyle, overrideValue: true);
        }
        
        internal static TextStyle OverrideStyle(this TextStyle style, TextStyle parent)
        {
            var cacheKey = (style, parent);
            var options = new StylesApplyingOptions
            {
                OverrideStyle = true,
                OverrideFontFamily = true,
                AllowFallback = true
            };
            
            return TextStyleOverrideCache.GetOrAdd(cacheKey, tuple => tuple.Origin.ApplyStyleProperties(tuple.Parent, options));
        }

        private static TextStyle ApplyStyleProperties(this TextStyle style, TextStyle parentStyle, StylesApplyingOptions options)
        {
            return AvailableStyleProperties.Aggregate(style, (mutableStyle, nextProperty) =>
                nextProperty switch
                {
                    TextStyleProperty.FontFamily when string.IsNullOrWhiteSpace(mutableStyle.FontFamily) || options.OverrideFontFamily
                        => mutableStyle.MutateStyle(TextStyleProperty.FontFamily, parentStyle.FontFamily, options.OverrideStyle),
                    
                    TextStyleProperty.Fallback when options.AllowFallback
                        => mutableStyle.MutateStyle(TextStyleProperty.Fallback, parentStyle.Fallback, options.OverrideStyle),
                    
                    _ => mutableStyle.MutateStyle(nextProperty, parentStyle.GetValue(nextProperty), options.OverrideStyle)
                });
        }
    }
}