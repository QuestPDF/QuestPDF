using System;
using System.Collections.Concurrent;
using QuestPDF.Skia;

namespace QuestPDF.Helpers
{
    internal static class ColorValidator
    {
        static ColorValidator()
        {
            NativeDependencyCompatibilityChecker.Test();
        }
        
        private static readonly ConcurrentDictionary<string, bool> ColorsValidityCache = new();
        
        public static void Validate(string? color)
        {
            if (color == null)
                throw new ArgumentException("Color value cannot be null");
            
            var isValid = ColorsValidityCache.GetOrAdd(color, x => SkColor.TryParse(x, out var _));
            
            if (isValid)
                return;

            throw new ArgumentException(
                $"The provided value '{color}' is not a valid hex color. " +
                "The following formats are supported: #RGB, #ARGB, #RRGGBB, #AARRGGBB. " +
                "The hash sign is optional so the following formats are also valid: RGB, ARGB, RRGGBB, AARRGGBB. " +
                "For example #FF8800 is a solid orange color, while #20CF is a barely visible aqua color.");
        }
    }
}