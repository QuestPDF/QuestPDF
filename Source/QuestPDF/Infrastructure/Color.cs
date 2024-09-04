using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure;

public readonly struct Color
{
    public uint Hex { get; }
    
    public byte Alpha => (byte) ((Hex & 0xFF000000) >> 24);
    public byte Red => (byte) ((Hex & 0xFF0000) >> 16);
    public byte Green => (byte) ((Hex & 0x00FF00) >> 8);
    public byte Blue => (byte) (Hex & 0x0000FF);

    public Color(uint hex)
    {
        if (hex <= 0x00FFFFFF && hex != 0x00000000) 
            hex |= 0xFF000000;
        
        Hex = hex;
    }
    
    public Color WithAlpha(byte alpha)
    {
        var newHex = (Hex & 0x00FFFFFF) | ((uint)alpha << 24);
        return new Color(newHex);
    }
    
    public static Color FromHex(string hex)
    {
        return ColorParser.ParseColorHex(hex);
    }

    public static Color FromRGB(byte red, byte green, byte blue)
    {
        return FromARGB(255, red, green, blue);
    }
    
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
        return new Color(hex);
    }

    public override string ToString()
    {
        if (Alpha == 0xFF)
            return $"#{Red:X2}{Green:X2}{Blue:X2}";
        
        return $"#{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}";
    }
}