using System;
using System.Collections.Concurrent;
using System.Globalization;
using QuestPDF.Infrastructure;

namespace QuestPDF.Helpers;

static class ColorParser
{
    private static readonly ConcurrentDictionary<string, Color?> Cache = new(); 
    
    public static Color ParseColorHex(string hexString)
    {
        if (!TryParseColorHex(hexString, out var color))
        {
            throw new ArgumentException(
                $"The provided value '{color}' is not a valid hex color. " +
                "The following formats are supported: #RGB, #ARGB, #RRGGBB, #AARRGGBB. " +
                "The hash sign is optional so the following formats are also valid: RGB, ARGB, RRGGBB, AARRGGBB. " +
                "For example #FF8800 is a solid orange color, while #20CF is a barely visible aqua color.",
                nameof(color));
        }
        
        return color;
    }
    
    // inspired by: https://github.com/mono/SkiaSharp/blob/9274aeec807fd17eec2a3266ad4c2475c37d8a0c/binding/SkiaSharp/SKColor.cs#L123
    public static bool TryParseColorHex(string hexString, out Color color)
    {
        var result = Cache.GetOrAdd(hexString, ParseColor);
        color = result ?? Colors.Black;
        return result.HasValue;

        static Color? ParseColor(string hexString)
        {
            uint color = 0;
            
            if (string.IsNullOrWhiteSpace(hexString))
                return null;
    
            // clean up string
            var hexSpan = hexString.AsSpan().Trim().TrimStart('#');
    
            var len = hexSpan.Length;
    
            if (len == 3 || len == 4)
            {
                byte a;
                
                // parse [A]
                if (len == 4)
                {
                    if (!byte.TryParse(hexSpan.Slice(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a))
                        return null;
    
                    a = (byte)((a << 4) | a);
                }
                else
                {
                    a = 255;
                }
    
                // parse RGB
                if (!byte.TryParse(hexSpan.Slice(len - 3, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) ||
                    !byte.TryParse(hexSpan.Slice(len - 2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) ||
                    !byte.TryParse(hexSpan.Slice(len - 1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    return null;
                }
    
                r |= (byte)(r << 4);
                g |= (byte)(g << 4);
                b |= (byte)(b << 4);
                
                // success
                color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
                return new Color(color);
            }
    
            if (len == 6 || len == 8)
            {
                // parse [AA]RRGGBB
                if (!uint.TryParse(hexSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number))
                    return null;
    
                // success
                color = number;
    
                // alpha was not provided, so use 255
                if (len == 6)
                    color |= 0xFF000000;
                
                return new Color(color);
            }
    
            return null;
        }
    }
}