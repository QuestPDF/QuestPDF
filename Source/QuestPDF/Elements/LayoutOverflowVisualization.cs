using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class LayoutOverflowVisualization : ContainerElement, IContentDirectionAware
{
    private const float BorderThickness = 1.5f;
    private readonly Color LineColor = Colors.Red.Medium;
    private readonly Color AvailableAreaColor = Colors.Green.Medium;
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