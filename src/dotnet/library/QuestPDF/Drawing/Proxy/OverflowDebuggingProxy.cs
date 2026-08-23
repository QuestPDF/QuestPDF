using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal sealed class OverflowDebuggingProxy : ElementProxy
{
    private LayoutOverflowDebugger Debugger { get; }

    public Size? AvailableSpace { get; private set; }
    public SpacePlan? SpacePlan { get; private set; }

    public OverflowDebuggingProxy(Element child, LayoutOverflowDebugger debugger)
    {
        Child = child;
        Debugger = debugger;
    }

    internal override SpacePlan Measure(Size availableSpace)
    {
        var spacePlan = Child.Measure(availableSpace);

        if (Debugger.IsRecording && !Size.Equal(availableSpace, Size.Zero))
        {
            AvailableSpace = availableSpace;
            SpacePlan = spacePlan;
        }
        
        return spacePlan;
    }
    
    public void Reset()
    {
        AvailableSpace = null;
        SpacePlan = null;
    }
}