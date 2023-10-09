using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

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
    public ICollection<int> VisibleOnPageNumbers { get; set; } = new List<int>();

    internal override SpacePlan Measure(Size availableSpace)
    {
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type == SpacePlanType.FullRender)
            return childSize;
        
        return SpacePlan.FullRender(availableSpace);
    }
        
    internal override void Draw(Size availableSpace)
    {
        VisibleOnPageNumbers.Add(PageContext.CurrentPage);
        
        // measure content area
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type == SpacePlanType.FullRender)
        {
            Child?.Draw(availableSpace);
            return;
        }
        
        // check overflow area
        var contentSize = 
            TryVerticalOverflow(availableSpace) 
            ?? TryHorizontalOverflow(availableSpace) 
            ?? TryUnconstrainedOverflow() 
            ?? Size.Max;
        
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

    private Size? TryOverflow(Size targetSpace)
    {
        var contentSize = base.Measure(targetSpace);
        return contentSize.Type == SpacePlanType.Wrap ? null : contentSize;
    }
    
    private Size? TryVerticalOverflow(Size availableSpace)
    {
        var overflowSpace = new Size(availableSpace.Width, Size.Infinity);
        return TryOverflow(overflowSpace);
    }
    
    private Size? TryHorizontalOverflow(Size availableSpace)
    {
        var overflowSpace = new Size(Size.Infinity, availableSpace.Height);
        return TryOverflow(overflowSpace);
    }
    
    private Size? TryUnconstrainedOverflow()
    {
        var overflowSpace = new Size(Size.Infinity, Size.Infinity);
        return TryOverflow(overflowSpace);
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