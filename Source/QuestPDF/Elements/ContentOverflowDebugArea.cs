using System;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements;

internal class ContentOverflowDebugArea : ContainerElement
{
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
        
        var overflowSpace = TryVerticalOverflow(availableSpace) 
            ?? TryVerticalOverflow(availableSpace) 
            ?? TryExpandedOverflow(availableSpace) 
            ?? Size.Max;
        
        Child?.Draw(overflowSpace);

        DrawTargetAreaBorder(overflowSpace);
        DrawOverflowArea(availableSpace, overflowSpace);
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
    
    private void DrawOverflowArea(Size availableSpace, Size contentSize)
    {
        if (Canvas is not SkiaCanvasBase canvasBase)
            return;

        var skiaCanvas = canvasBase.Canvas;

        skiaCanvas.Save();
        ClipOverflowAreaVisibility();
        DrawCheckerboardPattern();
        skiaCanvas.Restore();
        
        if (Canvas is SkiaCanvasBase bases2)
        {
            bases2.Canvas.Restore();
        }

        void DrawCheckerboardPattern()
        {
            const float checkerboardSize = 8;

            const string lightCellColor = "#44f44336";
            const string darkCellColor = "#88f44336";
            
            var boardSizeX = (int)Math.Ceiling(contentSize.Width / checkerboardSize);
            var boardSizeY = (int)Math.Ceiling(contentSize.Height / checkerboardSize);

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

        void ClipOverflowAreaVisibility()
        {
            var path = new SKPath();

            var middleWidth = Math.Min(availableSpace.Width, contentSize.Width);
            var middleHeight = Math.Min(availableSpace.Height, contentSize.Height);
            
            path.MoveTo(availableSpace.Width, 0);
            path.LineTo(contentSize.Width, 0);
            path.LineTo(contentSize.Width, contentSize.Height);
            path.LineTo(0, contentSize.Height);
            path.LineTo(0, middleHeight);
            path.LineTo(middleWidth, middleHeight);
            path.LineTo(middleWidth, 0);
            
            skiaCanvas.Save();
            skiaCanvas.ClipPath(path);
        }
    }
}