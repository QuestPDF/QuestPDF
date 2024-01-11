using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.LayoutTests.TestEngine;

internal static class AnnotateInvalidAreaHelper
{
    private const float StripeThickness = 1f;
    private const float StripeScale = 3f;
    private const string LineColor = Colors.Red.Medium;
    
    public static void Annotate(SKCanvas canvas, SKPath area)
    {
        canvas.Save();
        canvas.ClipPath(area);

        DrawCheckerboardPattern();
        
        canvas.Restore();

        void DrawCheckerboardPattern()
        {
            var matrix = SKMatrix.CreateScale(StripeScale, StripeScale).PostConcat(SKMatrix.CreateRotation((float)(Math.PI / 4)));

            using var paint = new SKPaint
            {
                Color = SKColor.Parse(LineColor),
                PathEffect = SKPathEffect.Create2DLine(StripeThickness, matrix),
                IsAntialias = true
            };

            var stripeArea = area.Bounds;
            stripeArea.Inflate(StripeScale * 2, StripeScale * 2);
            canvas.DrawRect(stripeArea, paint);
        }
    }
}