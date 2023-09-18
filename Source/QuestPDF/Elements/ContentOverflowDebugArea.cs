using System;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements;

internal class ContentOverflowDebugArea : ContainerElement, IContentDirectionAware
{
    public ContentDirection ContentDirection { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type == SpacePlanType.Wrap)
            return SpacePlan.FullRender(availableSpace);

        return childSize;
    }
        
    internal override void Draw(Size availableSpace)
    {
        var childSize = base.Measure(availableSpace);

        if (childSize.Type != SpacePlanType.Wrap)
        {
            Child?.Draw(availableSpace);
            return;
        }
        
        var contentSpace = 
            TryVerticalOverflow(availableSpace) 
            ?? TryHorizontalOverflow(availableSpace) 
            ?? TryExpandedOverflow(availableSpace) 
            ?? Size.Max;
        
        var translate = ContentDirection == ContentDirection.RightToLeft
            ? new Position(availableSpace.Width - contentSpace.Width, 0)
            : Position.Zero;
        
        Canvas.Translate(translate);
        Child?.Draw(contentSpace);
        Canvas.Translate(translate.Reverse());
        
        var overflowTranslate = ContentDirection == ContentDirection.RightToLeft ? new Position(availableSpace.Width, 0) : Position.Zero;
        var overflowScale = ContentDirection == ContentDirection.RightToLeft ? -1 : 1;
        
        Canvas.Translate(overflowTranslate);
        Canvas.Scale(overflowScale, 1);
        
        DrawTargetAreaBorder(contentSpace);
        DrawOverflowArea(availableSpace, contentSpace);
        
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
    
    private Size? TryExpandedOverflow(Size availableSpace)
    {
        var overflowSpace = new Size(Size.Infinity, Size.Infinity);
        return TryOverflow(overflowSpace);
    }
    
    private void DrawTargetAreaBorder(Size contentSize)
    {
        const float borderWidth = 2;
        const string borderColor = "#f44336";
        
        Canvas.DrawRectangle(
            new Position(-borderWidth/2, -borderWidth/2), 
            new Size(contentSize.Width + borderWidth/2 + borderWidth/2, borderWidth), 
            borderColor);
            
        Canvas.DrawRectangle(
            new Position(-borderWidth/2, -borderWidth/2), 
            new Size(borderWidth, contentSize.Height + borderWidth/2 + borderWidth/2), 
            borderColor);
            
        Canvas.DrawRectangle(
            new Position(-borderWidth/2, contentSize.Height-borderWidth/2), 
            new Size(contentSize.Width + borderWidth/2 + borderWidth/2, borderWidth), 
            borderColor);
            
        Canvas.DrawRectangle(
            new Position(contentSize.Width-borderWidth/2, -borderWidth/2), 
            new Size(borderWidth, contentSize.Height + borderWidth/2 + borderWidth/2), 
            borderColor);
    }
    
    private void DrawOverflowArea(Size availableSpace, Size contentSpace)
    {
        if (Canvas is not SkiaCanvasBase canvasBase)
            return;

        var skiaCanvas = canvasBase.Canvas;

        skiaCanvas.Save();
        ClipOverflowAreaVisibility();
        DrawCheckerboardPattern();
        skiaCanvas.Restore();

        void DrawCheckerboardPattern()
        {
            const float checkerboardSize = 8;

            const string lightCellColor = "#44f44336";
            const string darkCellColor = "#88f44336";
            
            var boardSizeX = (int)Math.Ceiling(contentSpace.Width / checkerboardSize);
            var boardSizeY = (int)Math.Ceiling(contentSpace.Height / checkerboardSize);

            foreach (var x in Enumerable.Range(0, boardSizeX))
            {
                foreach (var y in Enumerable.Range(0, boardSizeY))
                {
                    var cellColor = (x + y) % 2 == 0 ? lightCellColor : darkCellColor;
                    var cellPosition = new Position(x * checkerboardSize, y * checkerboardSize);
                    var cellSize = new Size(checkerboardSize, checkerboardSize);
                
                    Canvas.DrawRectangle(cellPosition, cellSize, cellColor);
                }
            }
        }

        // creates and applies an L-shaped clipping mask
        void ClipOverflowAreaVisibility()
        {
            var path = new SKPath();

            var middleWidth = Math.Min(availableSpace.Width, contentSpace.Width);
            var middleHeight = Math.Min(availableSpace.Height, contentSpace.Height);
            
            path.MoveTo(availableSpace.Width, 0);
            path.LineTo(contentSpace.Width, 0);
            path.LineTo(contentSpace.Width, contentSpace.Height);
            path.LineTo(0, contentSpace.Height);
            path.LineTo(0, middleHeight);
            path.LineTo(middleWidth, middleHeight);
            path.LineTo(middleWidth, 0);
            
            skiaCanvas.Save();
            skiaCanvas.ClipPath(path);
        }
    }
}