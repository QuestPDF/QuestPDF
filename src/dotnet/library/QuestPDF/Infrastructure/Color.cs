using System;
using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure;

public readonly struct Color
{
    public uint Hex { get; }
    
    public byte Alpha => (byte) ((Hex & 0xFF000000) >> 24);
    public byte Red => (byte) ((Hex & 0xFF0000) >> 16);
    public byte Green => (byte) ((Hex & 0x00FF00) >> 8);
    public byte Blue => (byte) (Hex & 0x0000FF);

    /// <summary>
    /// Creates a new color from a hex value following the ARGB format.
    /// For example 0xFF0000FF represents a fully opaque blue color.
    /// </summary>
    public Color(uint hex)
    {
        Hex = hex;
    }


    /// <summary>
    /// Creates a new color instance with the specified alpha transparency value.
    /// The alpha value should be within the range 0 to 255, where 0 represents fully transparent
    /// and 255 represents fully opaque.
    /// </summary>
    /// <returns>A new color instance with the adjusted alpha transparency.</returns>
    public Color WithAlpha(byte alpha)
    {
        var newHex = (Hex & 0x00FFFFFF) | ((uint)alpha << 24);
        return new Color(newHex);
    }
    
    /// <summary>
    /// Creates a new color instance with the specified alpha transparency value.
    /// The alpha value should be within the range 0 to 1, where 0 represents fully transparent
    /// and 1 represents fully opaque.
    /// </summary>
    /// <returns>A new color instance with the adjusted alpha transparency.</returns>
    public Color WithAlpha(float alpha)
    {
        if (alpha < 0 || alpha > 1)
            throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha value must be between 0 and 1.");
        
        var newAlpha = (byte)(255 * alpha);
        return WithAlpha(newAlpha);
    }

    /// <summary>
    /// Creates a new color instance from a hex string representation.
    /// The string should follow the formats: #RGB, #ARGB, #RRGGBB, or #AARRGGBB.
    /// The hash sign is optional; for example: RGB, ARGB, RRGGBB, or AARRGGBB are also valid.
    /// </summary>
    /// <param name="hex">The hex string representing the color.</param>
    /// <returns>A new color instance created from the hex string.</returns>
    public static Color FromHex(string hex)
    {
        return ColorParser.ParseColorHex(hex);
    }

    /// <summary>
    /// Creates a new color instance given the red, green, and blue component values.
    /// Each component should be within the range 0 to 255.
    /// </summary>
    public static Color FromRGB(byte red, byte green, byte blue)
    {
        return FromARGB(255, red, green, blue);
    }

    /// <summary>
    /// Creates a new color instance from the specified alpha, red, green, and blue component values.
    /// Each component should be within the range 0 to 255.
    /// </summary>
    public static Color FromARGB(byte alpha, byte red, byte green, byte blue)
    {
        return new Color((uint) (alpha << 24 | red << 16 | green << 8 | blue));
    }
    
    public static implicit operator string(Color color)
    {
        return color.ToString();
    }
    
    public static implicit operator Color(string hex)
    {
        return FromHex(hex);
    }
    
    public static implicit operator uint(Color color)
    {
        return color.Hex;
    }
    
    public static implicit operator Color(uint hex)
    {
        if (hex <= 0x00FFFFFF) 
            hex |= 0xFF000000;
        
        return new Color(hex);
    }

    public override string ToString()
    {
        if (Alpha == 0xFF)
            return $"#{Red:X2}{Green:X2}{Blue:X2}";
        
        return $"#{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}";
    }
}