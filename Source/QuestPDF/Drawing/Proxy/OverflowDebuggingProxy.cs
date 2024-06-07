using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class OverflowDebuggingProxy : ElementProxy
{
    public Size? OriginalMeasurementSize { get; private set; }
    public Size? MeasurementSize { get; private set; }
    
    public SpacePlan? OriginalSpacePlan { get; private set; }
    public SpacePlan? SpacePlan { get; private set; }

    public OverflowDebuggingProxy(Element child)
    {
        Child = child;
    }

    internal override SpacePlan Measure(Size availableSpace)
    {
        var spacePlan = Child.Measure(availableSpace);

        MeasurementSize = availableSpace;
        SpacePlan = spacePlan;
        
        return spacePlan;
    }

    internal void CaptureOriginalValues()
    {
        OriginalMeasurementSize = MeasurementSize;
        OriginalSpacePlan = SpacePlan;
    }
}