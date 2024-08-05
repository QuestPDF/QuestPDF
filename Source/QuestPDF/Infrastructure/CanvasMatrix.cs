using System;

namespace QuestPDF.Infrastructure;

internal readonly struct CanvasMatrix
{
    public readonly float ScaleX;
    public readonly float ScaleY;
    
    public readonly float TranslateX;
    public readonly float TranslateY;
    
    public readonly float SkewX;
    public readonly float SkewY;
    
    public CanvasMatrix(float scaleX, float scaleY, float translateX, float translateY, float skewX, float skewY)
    {
        ScaleX = scaleX;
        ScaleY = scaleY;
        TranslateX = translateX;
        TranslateY = translateY;
        SkewX = skewX;
        SkewY = skewY;
    }
    
    public CanvasMatrix Translate(float x, float y)
    {
        return new CanvasMatrix(ScaleX, ScaleY, TranslateX + x, TranslateY + y, SkewX, SkewY);
    }
    
    public CanvasMatrix Scale(float x, float y)
    {
        return new CanvasMatrix(ScaleX * x, ScaleY * y, TranslateX, TranslateY, SkewX, SkewY);
    }
    
    public CanvasMatrix Rotate(float angle)
    {
        var radians = angle * MathF.PI / 180;
        
        var sin = MathF.Sin(radians);
        var cos = MathF.Cos(radians);
        
        return new CanvasMatrix(
            ScaleX * cos - ScaleY * sin,
            ScaleX * sin + ScaleY * cos,
            TranslateX,
            TranslateY,
            SkewX,
            SkewY
        );
    }
}
