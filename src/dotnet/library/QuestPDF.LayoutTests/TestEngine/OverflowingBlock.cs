using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Skia;

namespace QuestPDF.LayoutTests.TestEngine;

/// <summary>
/// Works like <see cref="SolidBlock"/>, but always reports its full size, even when it exceeds the available space.
/// Built-in elements never do that, it simulates a child that breaks the measurement contract.
/// </summary>
internal class OverflowingBlock : Element, IStateful
{
    public float TotalWidth { get; set; }
    public float TotalHeight { get; set; }

    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();

        return SpacePlan.FullRender(TotalWidth, TotalHeight);
    }

    internal override void Draw(Size availableSpace)
    {
        using var paint = new SkPaint();
        paint.SetSolidColor(Placeholders.BackgroundColor());
        Canvas.DrawRectangle(Position.Zero, availableSpace, paint);

        IsRendered = true;
    }

    #region IStateful

    private bool IsRendered { get; set; }

    public void ResetState(bool hardReset = false) => IsRendered = false;
    public object GetState() => IsRendered;
    public void SetState(object state) => IsRendered = (bool) state;

    #endregion
}
