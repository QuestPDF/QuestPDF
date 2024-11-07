using System;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.Proxy;

internal class LayoutOverflowVisualization : ElementProxy, IContentDirectionAware
{
    private const float BorderThickness = 1.5f;
    private readonly Color LineColor = Colors.Red.Medium;
    private readonly Color AvailableAreaColor = Colors.Green.Medium;
    private const byte AreaOpacity = 64;

    public ContentDirection ContentDirection { get; set; }

    internal override SpacePlan Measure(Size availableSpace)
    {
        if (Size.Equal(availableSpace, Size.Zero))
            return SpacePlan.Wrap("There is no available space.");
        
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type == SpacePlanType.FullRender)
            return childSize;

        var minimalSize = Child.TryMeasureWithOverflow(availableSpace);

        if (minimalSize.Type is SpacePlanType.Wrap)
            return minimalSize;
        
        var width = Math.Min(availableSpace.Width, minimalSize.Width);
        var height = Math.Min(availableSpace.Height, minimalSize.Height);
        
        return new SpacePlan(minimalSize.Type, width, height);
    }
        
    internal override void Draw(Size availableSpace)
    {
        // measure content area
        var childSize = base.Measure(availableSpace);
        
        if (childSize.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
        {
            Child?.Draw(availableSpace);
            return;
        }

        Canvas = Child.Canvas;
        
        if (Canvas is SkiaCanvasBase skiaCanvasBase)
            skiaCanvasBase.MarkCurrentPageAsHavingLayoutIssues();
        
        // check overflow area
        var contentArea = Child.TryMeasureWithOverflow(availableSpace);

        var contentSize = contentArea.Type is SpacePlanType.Wrap
            ? Size.Max
            : contentArea;
        
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
        var availableSpaceColor = AvailableAreaColor.WithAlpha(AreaOpacity);
        Canvas.DrawFilledRectangle(Position.Zero, availableSpace, availableSpaceColor);

        Canvas.Save();
        Canvas.ClipOverflowArea(new SkRect(0, 0, availableSpace.Width, availableSpace.Height), new SkRect(0, 0, contentSize.Width, contentSize.Height));
        Canvas.DrawOverflowArea(new SkRect(0, 0, contentSize.Width, contentSize.Height));
        Canvas.Restore();

        Canvas.DrawStrokeRectangle(Position.Zero, contentSize, BorderThickness, LineColor);
    }
}