using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace QuestPDF.Infrastructure
{
    internal enum TextStyleProperty
    {
        Color,
        BackgroundColor,
        DecorationColor,
        FontFamily,
        FontFamilyFallback,
        Size,
        LineHeight,
        LetterSpacing,
        WordSpacing,
        FontWeight,
        FontPosition,
        IsItalic,
        HasStrikethrough,
        HasUnderline,
        HasOverline,        
        DecorationStyle,   
        DecorationThickness,
        Direction
    }
    
    internal static class TextStyleManager
    {
        private static readonly List<TextStyle> TextStyles = new()
        {
            TextStyle.Default,
            TextStyle.LibraryDefault
        };

        private static readonly ConcurrentDictionary<(int originId, TextStyleProperty property, object value), TextStyle> TextStyleMutateCache = new();
        private static readonly ConcurrentDictionary<(int originId, int parentId), TextStyle> TextStyleApplyInheritedCache = new();
        private static readonly ConcurrentDictionary<int, TextStyle> TextStyleApplyGlobalCache = new();
        private static readonly ConcurrentDictionary<(int originId, int parentId), TextStyle> TextStyleOverrideCache = new();
        
        private static readonly object MutationLock = new();

        public static TextStyle Mutate(this TextStyle origin, TextStyleProperty property, object value)
        {
            var cacheKey = (origin.Id, property, value);
            return TextStyleMutateCache.GetOrAdd(cacheKey, x => MutateStyle(TextStyles[x.originId], x.property, x.value, overrideValue: true));
        }

        private static TextStyle MutateStyle(this TextStyle origin, TextStyleProperty targetProperty, object? newValue, bool overrideValue)
        {
            lock (MutationLock)
            {
                if (overrideValue && newValue is null)
                    return origin;
                
                var property = typeof(TextStyle).GetProperty(targetProperty.ToString(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Debug.Assert(property != null);
                
                var oldValue = property.GetValue(origin);
                
                if (!overrideValue && oldValue is not null)
                    return origin;

                if (oldValue == newValue) 
                    return origin;
                
                var newIndex = TextStyles.Count;
                var newTextStyle = origin with { Id = newIndex };
                newTextStyle.Id = newIndex;
                property.SetValue(newTextStyle, newValue);

                TextStyles.Add(newTextStyle);
                return newTextStyle;
            }
        }
        
        internal static TextStyle ApplyInheritedStyle(this TextStyle style, TextStyle parent)
        {
            return TextStyleApplyInheritedCache.GetOrAdd((style.Id, parent.Id), key => ApplyStyleProperties(key.originId, key.parentId, overrideStyle: false));
        }
        
        internal static TextStyle ApplyGlobalStyle(this TextStyle style)
        {
            return TextStyleApplyGlobalCache.GetOrAdd(style.Id, key => ApplyStyleProperties(key, TextStyle.LibraryDefault.Id, overrideStyle: false));
        }

        internal static TextStyle OverrideStyle(this TextStyle style, TextStyle parent)
        {
            return TextStyleOverrideCache.GetOrAdd((style.Id, parent.Id), key => ApplyStyleProperties(key.originId, key.parentId, overrideStyle: true));
        }
        
        private static TextStyle ApplyStyleProperties(int styleId, int parentId, bool overrideStyle)
        {
            var style = TextStyles[styleId];
            var parent = TextStyles[parentId];
            
            return Enum
                .GetValues(typeof(TextStyleProperty))
                .Cast<TextStyleProperty>()
                .Aggregate(style, (mutableStyle, nextProperty) =>
                {
                    var getParentProperty = typeof(TextStyle).GetProperty(nextProperty.ToString(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    Debug.Assert(getParentProperty != null);
                    
                    var newValue = getParentProperty.GetValue(parent);

                    return mutableStyle.MutateStyle(nextProperty, newValue, overrideStyle);
                });
        }
    }
}