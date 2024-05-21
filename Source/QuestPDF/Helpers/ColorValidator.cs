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
                if (string.IsNullOrWhiteSpace(color))
                {
                    return false;
                }

                // clean up string
                color = color.Trim();
                var startIndex = color[0] == '#' ? 1 : 0;

                var len = color.Length - startIndex;
                if (len == 3 || len == 4)
                {
                    // parse [A]
                    if (len == 4 && !byte.TryParse(string.Concat(new string(color[startIndex], 2)),
                            NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
                    {
                        return false;
                    }

                    // parse RGB
                    if (!byte.TryParse(new string(color[len - 3 + startIndex], 2), NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out _) ||
                        !byte.TryParse(new string(color[len - 2 + startIndex], 2), NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out _) ||
                        !byte.TryParse(new string(color[len - 1 + startIndex], 2), NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out _))
                    {
                        return false;
                    }

                    return true;
                }

                if (len == 6 || len == 8)
                {
                    // parse [AA]RRGGBB
                    if (!uint.TryParse(color.Substring(startIndex), NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out _))
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}