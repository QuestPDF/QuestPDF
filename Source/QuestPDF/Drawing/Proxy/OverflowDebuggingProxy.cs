using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class OverflowDebuggingProxy : ElementProxy
{
    public bool IsMeasuring { get; private set; } = true;
    
    public Size? MeasurementSize { get; private set; }
    public SpacePlan? SpacePlan { get; private set; }

    public OverflowDebuggingProxy(Element child)
    {
        Child = child;
    }

    internal override SpacePlan Measure(Size availableSpace)
    {
        var spacePlan = Child.Measure(availableSpace);

        if (IsMeasuring)
        {
            MeasurementSize = availableSpace;
            SpacePlan = spacePlan;
        }
        
        return spacePlan;
    }
    
    public void StopMeasuring()
    {
        IsMeasuring = false;
    }
}