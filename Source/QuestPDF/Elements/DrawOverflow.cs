using System;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

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
        const float checkerboardSize = 8;

        const string lightCellColor = "#44f44336";
        const string darkCellColor = "#88f44336";
        
        var overflowPosition = new Position(0, availableSpace.Height);
        var overflowSize = new Size(contentSize.Width, contentSize.Height - availableSpace.Height);
        
        var boardSizeX = (int)Math.Ceiling(overflowSize.Width / checkerboardSize);
        var boardSizeY = (int)Math.Ceiling(overflowSize.Height / checkerboardSize);
        
        Canvas.Translate(overflowPosition);
        
        foreach (var x in Enumerable.Range(0, boardSizeX))
        {
            foreach (var y in Enumerable.Range(0, boardSizeY))
            {
                var cellColor = (x + y) % 2 == 0 ? lightCellColor : darkCellColor;

                var cellPosition = new Position(
                    x * checkerboardSize, 
                    y * checkerboardSize);

                var isLastCellX = x + 1 == boardSizeX;
                var isLastCellY = y + 1 == boardSizeY;
                
                var cellSize = new Size(
                    isLastCellX ? overflowSize.Width - x * checkerboardSize : checkerboardSize, 
                    isLastCellY ? overflowSize.Height - y * checkerboardSize : checkerboardSize);
                
                Canvas.DrawRectangle(cellPosition, cellSize, cellColor);
            }
        }
        
        Canvas.Translate(overflowPosition.Reverse());
    }
}