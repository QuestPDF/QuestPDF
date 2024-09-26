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
        var radians = angle * Math.PI / 180;
        
        var sin = Math.Sin(radians);
        var cos = Math.Cos(radians);
        
        return new CanvasMatrix(
            (float)(ScaleX * cos - ScaleY * sin),
            (float)(ScaleX * sin + ScaleY * cos),
            TranslateX,
            TranslateY,
            SkewX,
            SkewY
        );
    }
}
