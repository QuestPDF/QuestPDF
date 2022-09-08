using System;
using System.Collections.Concurrent;
using QuestPDF.Fluent;

namespace QuestPDF.Infrastructure
{
    internal enum TextStyleProperty
    {
        Color,
        BackgroundColor,
        FontFamily,
        Size,
        LineHeight,
        FontWeight,
        FontPosition,
        IsItalic,
        HasStrikethrough,
        HasUnderline,
        WrapAnywhere
    }
    
    internal static class TextStyleManager
    {
        public static ConcurrentDictionary<(TextStyle origin, TextStyleProperty property, object value), TextStyle> TextStyleMutateCache = new();
        public static ConcurrentDictionary<(TextStyle origin, TextStyle parent, bool overrideValue), TextStyle> TextStyleApplyCache = new();

        public static TextStyle Mutate(this TextStyle origin, TextStyleProperty property, object value)
        {
            var cacheKey = (origin, property, value);
            return TextStyleMutateCache.GetOrAdd(cacheKey, x => MutateStyle(x.origin, x.property, x.value));
        }

        private static TextStyle MutateStyle(TextStyle origin, TextStyleProperty property, object value, bool overrideValue = true)
        {
            if (property == TextStyleProperty.Color)
            {
                if (!overrideValue && origin.Color != null)
                    return origin;
                
                var castedValue = (string?)value;

                if (origin.Color == castedValue)
                    return origin;

                return origin with { Color = castedValue };
            }
            
            if (property == TextStyleProperty.BackgroundColor)
            {
                if (!overrideValue && origin.BackgroundColor != null)
                    return origin;
                
                var castedValue = (string?)value;
                
                if (origin.BackgroundColor == castedValue)
                    return origin;

                return origin with { BackgroundColor = castedValue };
            }
            
            if (property == TextStyleProperty.FontFamily)
            {
                if (!overrideValue && origin.FontFamily != null)
                    return origin;
                
                var castedValue = (string?)value;
                
                if (origin.FontFamily == castedValue)
                    return origin;

                return origin with { FontFamily = castedValue };
            }
            
            if (property == TextStyleProperty.Size)
            {
                if (!overrideValue && origin.Size != null)
                    return origin;
                
                var castedValue = (float?)value;
                
                if (origin.Size == castedValue)
                    return origin;

                return origin with { Size = castedValue };
            }
            
            if (property == TextStyleProperty.LineHeight)
            {
                if (!overrideValue && origin.LineHeight != null)
                    return origin;
                
                var castedValue = (float?)value;
                
                if (origin.LineHeight == castedValue)
                    return origin;

                return origin with { LineHeight = castedValue };
            }
            
            if (property == TextStyleProperty.FontWeight)
            {
                if (!overrideValue && origin.FontWeight != null)
                    return origin;
                
                var castedValue = (FontWeight?)value;
                
                if (origin.FontWeight == castedValue)
                    return origin;

                return origin with { FontWeight = castedValue };
            }
            
            if (property == TextStyleProperty.FontPosition)
            {
                if (!overrideValue && origin.FontPosition != null)
                    return origin;
                
                var castedValue = (FontPosition?)value;
                
                if (origin.FontPosition == castedValue)
                    return origin;

                return origin with { FontPosition = castedValue };
            }
            
            if (property == TextStyleProperty.IsItalic)
            {
                if (!overrideValue && origin.IsItalic != null)
                    return origin;
                
                var castedValue = (bool?)value;
                
                if (origin.IsItalic == castedValue)
                    return origin;

                return origin with { IsItalic = castedValue };
            }
            
            if (property == TextStyleProperty.HasStrikethrough)
            {
                if (!overrideValue && origin.HasStrikethrough != null)
                    return origin;
                
                var castedValue = (bool?)value;
                
                if (origin.HasStrikethrough == castedValue)
                    return origin;

                return origin with { HasStrikethrough = castedValue };
            }
            
            if (property == TextStyleProperty.HasUnderline)
            {
                if (!overrideValue && origin.HasUnderline != null)
                    return origin;
                
                var castedValue = (bool?)value;
                
                if (origin.HasUnderline == castedValue)
                    return origin;

                return origin with { HasUnderline = castedValue };
            }
            
            if (property == TextStyleProperty.WrapAnywhere)
            {
                if (!overrideValue && origin.WrapAnywhere != null)
                    return origin;

                var castedValue = (bool?)value;
                
                if (origin.WrapAnywhere == castedValue)
                    return origin;
                
                return origin with { WrapAnywhere = castedValue };
            }

            throw new ArgumentOutOfRangeException(nameof(property), property, "Expected to mutate the TextStyle object. Provided property type is not supported.");
        }
        
        internal static TextStyle ApplyGlobalStyle(this TextStyle style, TextStyle parent)
        {
            var cacheKey = (style, parent, false);
            return TextStyleApplyCache.GetOrAdd(cacheKey, key => ApplyStyle(key.origin, key.parent, key.overrideValue));
        }
        
        internal static TextStyle OverrideStyle(this TextStyle style, TextStyle parent)
        {
            var cacheKey = (style, parent, true);
            return TextStyleApplyCache.GetOrAdd(cacheKey, key => ApplyStyle(key.origin, key.parent, key.overrideValue));
        }
        
        private static TextStyle ApplyStyle(TextStyle style, TextStyle parent, bool overrideValue)
        {
            var result = style;

            result = MutateStyle(result, TextStyleProperty.Color, parent.Color, overrideValue);
            result = MutateStyle(result, TextStyleProperty.BackgroundColor, parent.BackgroundColor, overrideValue);
            result = MutateStyle(result, TextStyleProperty.FontFamily, parent.FontFamily, overrideValue);
            result = MutateStyle(result, TextStyleProperty.Size, parent.Size, overrideValue);
            result = MutateStyle(result, TextStyleProperty.LineHeight, parent.LineHeight, overrideValue);
            result = MutateStyle(result, TextStyleProperty.FontWeight, parent.FontWeight, overrideValue);
            result = MutateStyle(result, TextStyleProperty.FontPosition, parent.FontPosition, overrideValue);
            result = MutateStyle(result, TextStyleProperty.IsItalic, parent.IsItalic, overrideValue);
            result = MutateStyle(result, TextStyleProperty.HasStrikethrough, parent.HasStrikethrough, overrideValue);
            result = MutateStyle(result, TextStyleProperty.HasUnderline, parent.HasUnderline, overrideValue);
            result = MutateStyle(result, TextStyleProperty.WrapAnywhere, parent.WrapAnywhere, overrideValue);
            
            return result;
        }
    }
}