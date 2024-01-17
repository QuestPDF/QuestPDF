using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class LayoutOverflowVisualization : ContainerElement, IContentDirectionAware
{
    private const float BorderThickness = 1.5f;
    private const float StripeThickness = 1.5f;
    private const float StripeScale = 6f;
    private const string LineColor = Colors.Red.Medium;
    private const string AvailableAreaColor = Colors.Green.Medium;
    private const string OverflowAreaColor = Colors.Red.Medium;
    private const byte AreaOpacity = 64;

    public ContentDirection ContentDirection { get; set; }

    internal override SpacePlan Measure(Size availableSpace)
    {
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type == SpacePlanType.FullRender)
            return childSize;
        
        return SpacePlan.FullRender(availableSpace);
    }
        
    internal override void Draw(Size availableSpace)
    {
        // measure content area
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type == SpacePlanType.FullRender)
        {
            Child?.Draw(availableSpace);
            return;
        }
        
        if (Canvas is SkiaCanvasBase skiaCanvasBase)
            skiaCanvasBase.MarkCurrentPageAsHavingLayoutIssues();
        
        // check overflow area
        var contentSize = Child.TryMeasureWithOverflow(availableSpace) ?? Size.Max;
        
        // draw content
        var translate = ContentDirection == ContentDirection.RightToLeft
            ? new Position(availableSpace.Width - contentSize.Width, 0)
            : Position.Zero;
        
        Canvas.Translate(translate);
        Child?.Draw(contentSize);
        Canvas.Translate(translate.Reverse());
        
        // draw overflow area
        var overflowTranslate = ContentDirection == ContentDirection.RightToLeft ? new Position(availableSpace.Width, 0) : Position.Zero;
        var overflowScale = ContentDirection == ContentDirection.RightToLeft ? -1 : 1;
        
        Canvas.Translate(overflowTranslate);
        Canvas.Scale(overflowScale, 1);
        
        DrawOverflowArea(availableSpace, contentSize);
        
        Canvas.Scale(overflowScale, 1);
        Canvas.Translate(overflowTranslate.Reverse());
    }

    private void DrawOverflowArea(Size availableSpace, Size contentSize)
    {
        if (Canvas is not SkiaCanvasBase canvasBase)
            return;
        
        var skiaCanvas = canvasBase.Canvas;

        DrawAvailableSpaceBackground();

        skiaCanvas.Save();
        ClipOverflowAreaVisibility();
        DrawOverflowArea();
        DrawCheckerboardPattern();
        skiaCanvas.Restore();

        DrawContentAreaBorder();

        void DrawAvailableSpaceBackground()
        {
            using var paint = new SKPaint
            {
                Color = SKColor.Parse(AvailableAreaColor).WithAlpha(AreaOpacity)
            };
        
            skiaCanvas.DrawRect(0, 0, availableSpace.Width, availableSpace.Height, paint);
        }
        
        void DrawContentAreaBorder()
        {
            using var borderPaint = new SKPaint
            {
                Color = SKColor.Parse(LineColor),
                IsStroke = true,
                StrokeWidth = BorderThickness
            };

            skiaCanvas.DrawRect(0, 0, contentSize.Width, contentSize.Height, borderPaint);
        }
        
        void DrawOverflowArea()
        {
            using var areaPaint = new SKPaint
            {
                Color = SKColor.Parse(OverflowAreaColor).WithAlpha(AreaOpacity)
            };

            skiaCanvas.DrawRect(0, 0, contentSize.Width, contentSize.Height, areaPaint);
        }
        
        void DrawCheckerboardPattern()
        {
            var matrix = SKMatrix.CreateScale(StripeScale, StripeScale).PostConcat(SKMatrix.CreateRotation((float)(Math.PI / 4)));

            using var paint = new SKPaint
            {
                Color = SKColor.Parse(LineColor),
                PathEffect = SKPathEffect.Create2DLine(StripeThickness, matrix),
                IsAntialias = true
            };
            
            var targetArea = new SKRect(0,0,contentSize.Width, contentSize.Height);
            targetArea.Inflate(StripeScale * 2, StripeScale * 2);
            
            skiaCanvas.DrawRect(targetArea, paint);
        }

        void ClipOverflowAreaVisibility()
        {
            var path = new SKPath();

            path.AddRect(new SKRect(0, 0, contentSize.Width, contentSize.Height), SKPathDirection.Clockwise);
            path.AddRect(new SKRect(0, 0, Math.Min(availableSpace.Width, contentSize.Width), Math.Min(availableSpace.Height, contentSize.Height)), SKPathDirection.CounterClockwise);

            skiaCanvas.Save();
            skiaCanvas.ClipPath(path);
        }
    }
}