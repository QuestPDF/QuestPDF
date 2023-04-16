using System;
using System.Collections.Concurrent;
using SkiaSharp;

namespace QuestPDF.Helpers
{
    internal static class ColorValidator
    {
        private static readonly ConcurrentDictionary<string, bool> Colors = new();
        
        public static void Validate(string? color)
        {
            if (color == null)
                throw new ArgumentException("Color value cannot be null");
            
            var isValid = Colors.GetOrAdd(color, IsColorValid);
            
            if (isValid)
                return;

            throw new ArgumentException(
                $"The provided value '{color}' is not a valid hex color. " +
                "The following formats are supported: #RGB, #ARGB, #RRGGBB, #AARRGGBB. " +
                "The hash sign is optional so the following formats are also valid: RGB, ARGB, RRGGBB, AARRGGBB. " +
                "For example #FF8800 is a solid orange color, while #20CF is a barely visible aqua color.");

            static bool IsColorValid(string color)
            {
                try
                {
                    SKColor.Parse(color);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}