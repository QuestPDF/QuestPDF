using System;
using System.Collections.Concurrent;

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
        Direction,
    }

    internal static class TextStyleManager
    {
        private static readonly ConcurrentDictionary<(TextStyle origin, TextStyleProperty property, object value), TextStyle> TextStyleMutateCache = new();
        private static readonly ConcurrentDictionary<(TextStyle origin, TextStyle parent), TextStyle> TextStyleApplyInheritedCache = new();
        private static readonly ConcurrentDictionary<TextStyle, TextStyle> TextStyleApplyGlobalCache = new();
        private static readonly ConcurrentDictionary<(TextStyle origin, TextStyle parent), TextStyle> TextStyleOverrideCache = new();

        public static TextStyle Mutate(this TextStyle origin, TextStyleProperty property, object value)
        {
            var cacheKey = (origin, property, value);
            return TextStyleMutateCache.GetOrAdd(cacheKey, x => MutateStyle(x.origin, x.property, x.value, overrideValue: true));
        }

        private static TextStyle MutateStyle(this TextStyle origin, TextStyleProperty property, object? value, bool overrideValue)
        {
            if (overrideValue && value == null)
                return origin;

            switch (property)
            {
                case TextStyleProperty.Color:
                    {
                        if (!overrideValue && origin.Color != null)
                            return origin;

                        var castedValue = (string?)value;

                        if (origin.Color == castedValue)
                            return origin;

                        return origin with { Color = castedValue };
                    }

                case TextStyleProperty.BackgroundColor:
                    {
                        if (!overrideValue && origin.BackgroundColor != null)
                            return origin;

                        var castedValue = (string?)value;

                        if (origin.BackgroundColor == castedValue)
                            return origin;

                        return origin with { BackgroundColor = castedValue };
                    }

                case TextStyleProperty.FontFamily:
                    {
                        if (!overrideValue && origin.FontFamily != null)
                            return origin;

                        var castedValue = (string?)value;

                        if (origin.FontFamily == castedValue)
                            return origin;

                        return origin with { FontFamily = castedValue };
                    }

                case TextStyleProperty.Size:
                    {
                        if (!overrideValue && origin.Size != null)
                            return origin;

                        var castedValue = (float?)value;

                        if (origin.Size == castedValue)
                            return origin;

                        return origin with { Size = castedValue };
                    }

                case TextStyleProperty.LineHeight:
                    {
                        if (!overrideValue && origin.LineHeight != null)
                            return origin;

                        var castedValue = (float?)value;

                        if (origin.LineHeight == castedValue)
                            return origin;

                        return origin with { LineHeight = castedValue };
                    }

                case TextStyleProperty.LetterSpacing:
                    {
                        if (!overrideValue && origin.LetterSpacing != null)
                            return origin;

                        var castedValue = (float?)value;

                        if (origin.LetterSpacing == castedValue)
                            return origin;

                        return origin with { LetterSpacing = castedValue };
                    }

                case TextStyleProperty.FontWeight:
                    {
                        if (!overrideValue && origin.FontWeight != null)
                            return origin;

                        var castedValue = (FontWeight?)value;

                        if (origin.FontWeight == castedValue)
                            return origin;

                        return origin with { FontWeight = castedValue };
                    }

                case TextStyleProperty.FontPosition:
                    {
                        if (!overrideValue && origin.FontPosition != null)
                            return origin;

                        var castedValue = (FontPosition?)value;

                        if (origin.FontPosition == castedValue)
                            return origin;

                        return origin with { FontPosition = castedValue };
                    }

                case TextStyleProperty.IsItalic:
                    {
                        if (!overrideValue && origin.IsItalic != null)
                            return origin;

                        var castedValue = (bool?)value;

                        if (origin.IsItalic == castedValue)
                            return origin;

                        return origin with { IsItalic = castedValue };
                    }

                case TextStyleProperty.HasStrikethrough:
                    {
                        if (!overrideValue && origin.HasStrikethrough != null)
                            return origin;

                        var castedValue = (bool?)value;

                        if (origin.HasStrikethrough == castedValue)
                            return origin;

                        return origin with { HasStrikethrough = castedValue };
                    }

                case TextStyleProperty.HasUnderline:
                    {
                        if (!overrideValue && origin.HasUnderline != null)
                            return origin;

                        var castedValue = (bool?)value;

                        if (origin.HasUnderline == castedValue)
                            return origin;

                        return origin with { HasUnderline = castedValue };
                    }

                case TextStyleProperty.WrapAnywhere:
                    {
                        if (!overrideValue && origin.WrapAnywhere != null)
                            return origin;

                        var castedValue = (bool?)value;

                        if (origin.WrapAnywhere == castedValue)
                            return origin;

                        return origin with { WrapAnywhere = castedValue };
                    }

                case TextStyleProperty.Fallback:
                    {
                        if (!overrideValue && origin.Fallback != null)
                            return origin;

                        var castedValue = (TextStyle?)value;

                        if (origin.Fallback == castedValue)
                            return origin;

                        return origin with { Fallback = castedValue };
                    }

                case TextStyleProperty.Direction:
                    {
                        if (!overrideValue && origin.Direction != null)
                            return origin;

                        var castedValue = (TextDirection?)value;

                        if (origin.Direction == castedValue)
                            return origin;

                        return origin with { Direction = castedValue };
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(property), property, "Expected to mutate the TextStyle object. Provided property type is not supported.");
            }
        }

        internal static TextStyle ApplyInheritedStyle(this TextStyle style, TextStyle parent)
        {
            var cacheKey = (style, parent);
            return TextStyleApplyInheritedCache.GetOrAdd(cacheKey, key => key.origin.ApplyStyleProperties(key.parent, overrideStyle: false, overrideFontFamily: false, applyFallback: true).UpdateFontFallback(overrideStyle: true));
        }

        internal static TextStyle ApplyGlobalStyle(this TextStyle style)
        {
            return TextStyleApplyGlobalCache.GetOrAdd(style, key => key.ApplyStyleProperties(TextStyle.LibraryDefault, overrideStyle: false, overrideFontFamily: false, applyFallback: true).UpdateFontFallback(overrideStyle: false));
        }

        private static TextStyle UpdateFontFallback(this TextStyle style, bool overrideStyle)
        {
            var targetFallbackStyle = style
                ?.Fallback
                ?.ApplyStyleProperties(style, overrideStyle: overrideStyle, overrideFontFamily: false, applyFallback: false)
                ?.UpdateFontFallback(overrideStyle);

            return style.MutateStyle(TextStyleProperty.Fallback, targetFallbackStyle, overrideValue: true);
        }

        internal static TextStyle OverrideStyle(this TextStyle style, TextStyle parent)
        {
            var cacheKey = (style, parent);
            return TextStyleOverrideCache.GetOrAdd(cacheKey, key => ApplyStyleProperties(key.origin, key.parent, overrideStyle: true, overrideFontFamily: true, applyFallback: true));
        }

        private static TextStyle ApplyStyleProperties(this TextStyle style, TextStyle parent, bool overrideStyle, bool overrideFontFamily, bool applyFallback)
        {
            var result = style;

            if (string.IsNullOrWhiteSpace(result.FontFamily) || overrideFontFamily)
                result = MutateStyle(result, TextStyleProperty.FontFamily, parent.FontFamily, overrideStyle);

            result = MutateStyle(result, TextStyleProperty.Color, parent.Color, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.BackgroundColor, parent.BackgroundColor, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.Size, parent.Size, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.LineHeight, parent.LineHeight, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.LetterSpacing, parent.LetterSpacing, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.FontWeight, parent.FontWeight, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.FontPosition, parent.FontPosition, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.IsItalic, parent.IsItalic, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.HasStrikethrough, parent.HasStrikethrough, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.HasUnderline, parent.HasUnderline, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.WrapAnywhere, parent.WrapAnywhere, overrideStyle);
            result = MutateStyle(result, TextStyleProperty.Direction, parent.Direction, overrideStyle);

            if (applyFallback)
                result = MutateStyle(result, TextStyleProperty.Fallback, parent.Fallback, overrideStyle);

            return result;
        }
    }
}